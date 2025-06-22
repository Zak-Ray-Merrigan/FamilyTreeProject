using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace FamilyTreeLibrary.Service
{
    public class TemplateGenerator
    {
        private readonly InheritedFamilyName inheritedFamilyName;
        private readonly IEnumerable<TemplateLine> lines;
        private readonly ISet<Person> people;
        private readonly IExtendedLogger<TemplateGenerator> logger;

        public TemplateGenerator(IExtendedLogger<TemplateGenerator> logger, string name)
        {
            this.logger = logger;
            inheritedFamilyName = new(name);
            people = new HashSet<Person>();
            lines = GetFamily();
        }

        public TemplateGenerator(IExtendedLogger<TemplateGenerator> logger, string name, int id)
        {
            this.logger = logger;
            inheritedFamilyName = new(name, id);
            people = new SortedSet<Person>();
            lines = GetFamily();
        }

        private static IDictionary<string,BridgeInstance> DefaultPerson
        {
            get
            {
                return new Dictionary<string,BridgeInstance>()
                {
                    ["birthName"] = new(),
                    ["birthDate"] = new(),
                    ["deceasedDate"] = new()
                };
            }
        }
        private static string FilePath
        {
            get
            {
                DirectoryInfo directory = new(Directory.GetCurrentDirectory());
                while(directory.Name != "FamilyTreeProject")
                {
                    directory = directory.Parent!;
                }
                return System.IO.Path.Combine(directory.FullName, "resources\\PfingstenFamilyAlternative.txt");
            }
        }

        private static string WritePath
        {
            get
            {
                DirectoryInfo directory = new(Directory.GetCurrentDirectory());
                while(directory.Name != "FamilyTreeProject")
                {
                    directory = directory.Parent!;
                }
                return System.IO.Path.Combine(directory.FullName, "resources\\2023PfingstenBookAlternate.pdf");
            }
        }

        public FileStream WriteTemplate()
        {
            logger.LogInformation("Writing partnerships to the path: \"{filePath}\"", WritePath);
            using FileStream initialStream = new(WritePath, FileMode.Create);
            using PdfWriter templateWriter = new(initialStream);
            using PdfDocument templateDocument = new(templateWriter);
            using Document template = new(templateDocument);
            const float representationHeight = 36f;
            float pageHeight = template.GetPageEffectiveArea(PageSize.A4).GetHeight();
            float currentHeight = 0f;
            foreach (TemplateLine line in lines)
            {
                if (currentHeight + representationHeight > pageHeight)
                {
                    logger.LogDebug("Moving to a new page.");
                    template.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    currentHeight = 0f;
                }
                template.Add(new Paragraph(line.ToString()));
                logger.LogDebug("{line} has been written.", line);
                currentHeight += representationHeight;
            }
            template.Close();
            templateDocument.Close();
            templateWriter.Close();
            logger.LogInformation("Go ahead and look at: \"{filePath}\"", WritePath);
            return new(initialStream.Name, FileMode.Open, FileAccess.Read);
        }

        private TemplateLine BuildHeader(Match header, HierarchialCoordinate coordinate)
        {
            logger.LogInformation("The header: \"{headerText}\" is being decomposed and will be located at hierarchial coordinate {coordinate} relative to the family tree.", header, coordinate);
            IReadOnlyDictionary<string,string> headerDecomposition = GetHeaderDecomposition(header);
            logger.LogInformation("Final Decomposed Result:\n{result}", string.Join('\n', headerDecomposition.Select(pair => $"{pair.Key} -> {pair.Value}")));
            IDictionary<string,BridgeInstance> memberObj = DefaultPerson;
            IDictionary<string,BridgeInstance> inLawObj = DefaultPerson;
            IDictionary<string,BridgeInstance> familyDynamicObj = new Dictionary<string,BridgeInstance>();
            logger.LogDebug("Expressing decomposed result as member, in-law, and family dynamic instances.");
            foreach (string key in headerDecomposition.Keys)
            {
                switch(key)
                {
                    case "memberBirthName": memberObj["birthName"] = new(headerDecomposition[key]); break;
                    case "memberBirthDate": memberObj["birthDate"] = new(headerDecomposition[key]); break;
                    case "familyDynamicStartDate": familyDynamicObj["familyDynamicStartDate"] = new(headerDecomposition[key]); break;
                    case "memberDeceasedDate": memberObj["deceasedDate"] = new(headerDecomposition[key]); break;
                    case "inLawBirthName": inLawObj["birthName"] = new(headerDecomposition[key]); break;
                    case "inLawBirthDate": inLawObj["birthDate"] = new(headerDecomposition[key]); break;
                    case "inLawDeceasedDate": inLawObj["deceasedDate"] = new(headerDecomposition[key]); break;
                }
            }
            if (!memberObj["birthName"].IsString)
            {
                throw new MissingRequiredAttributeException("birthName");
            }
            Person member = FamilyTreeUtils.GetPerson(logger, inheritedFamilyName, people, new(memberObj, true));
            logger.LogInformation("Member -> {member}", member);
            if (member.BirthDate is null)
            {
                logger.LogWarning("No birth date recorded for \"{name}\"", member.BirthName);
            }
            Person? inLaw = inLawObj["birthName"].IsString ? FamilyTreeUtils.GetPerson(logger, inheritedFamilyName, people, new(inLawObj, true)) : null;
            if (inLaw is null)
            {
                familyDynamicObj["pageTitle"] = new($"This is the family of {member.BirthName}.");
            }
            else
            {
                logger.LogInformation("InLaw -> {inLaw}", inLaw);
                if (inLaw.BirthDate is null)
                {
                    logger.LogWarning("No birth date recorded for \"{name}\"", inLaw.BirthName);
                }
                familyDynamicObj["pageTitle"] = new($"This is the family of {member.BirthName} and {inLaw.BirthName}.");
            }
            FamilyDynamic familyDynamic = new(familyDynamicObj, true);
            logger.LogInformation("FamilyDynamic -> {familyDynamic}", familyDynamic);
            return new(coordinate, member, familyDynamic, inLaw);
        }

        private Queue<Content> GetContents(string whole, int generation, HierarchialCoordinate start)
        {
            logger.LogInformation("Given:\n{templateText}\nThe nodes belonging to generation {generation} are being parsed into headers and their existing descendants are being parsed later.", whole, generation);
            Queue<Content> contents = new();
            HierarchialCoordinate currentCoordinate = start;
            logger.LogDebug("Starting coordinate: {coordinate}", start);
            Regex headerRegex = GetRegexOfGeneration(generation);
            logger.LogDebug("Finding matches with the following regular expression: {pattern}", headerRegex);
            Queue<Match> headers = new(headerRegex.Matches(whole));
            logger.LogDebug("Here are the headers that are going to be parsed.\n{headersText}", string.Join('\n', headers));
            while (headers.TryDequeue(out Match? h1) && h1 is not null)
            {
                logger.LogDebug("Building {headerText}", h1);
                TemplateLine header = BuildHeader(h1, currentCoordinate.Copy());
                logger.LogInformation("Header: {header}", header);
                int startIndex = h1.Index + h1.Length + 1;
                logger.LogDebug("Begin Index of sub content: {index} (Relative to initial text)", startIndex);
                int endIndex = headers.TryPeek(out Match? h2) && h2 is not null ? h2.Index - 1 : whole.Length;
                logger.LogDebug("End Index of sub content: {index} (Relative to initial text)", endIndex);
                string subContent = startIndex < endIndex ? whole[startIndex .. endIndex] : "";
                logger.LogInformation("SubContent: {subContent}", subContent);
                contents.Enqueue(new(header, subContent));
                currentCoordinate = currentCoordinate.Sibling!.Value;
            }
            return contents;
        }

        private IEnumerable<TemplateLine> GetFamily()
        {
            List<TemplateLine> lines = [];
            Stack<Queue<Content>> contents = new();
            logger.LogInformation("Retrieving the family dynamics of {inheritedFamilyName}.", inheritedFamilyName);
            string templateText = ReadTextFile();
            logger.LogDebug("Starting with the 1st generation.");
            contents.Push(GetContents(templateText, contents.Count + 1, new([1])));
            while (contents.TryPop(out Queue<Content>? collection) && collection is not null)
            {
                logger.LogDebug("Remaining generation {number} contents:\n{contents}", contents.Count + 1, string.Join('\n', collection));
                if (collection.TryDequeue(out Content current))
                {
                    Content content = current.Copy();
                    logger.LogDebug("\"{header}\" is ready for writing.", content.Header);
                    lines.Add(content.Header);
                    logger.LogDebug("Constructing a node associating: \"{header}\"", content.Header);
                    logger.LogDebug("Since this is a pre-order traversal, we consider the children first. This means we have to save the remaining for later.");
                    contents.Push(collection);
                    logger.LogDebug("We are considering the sub content as children.");
                    contents.Push(GetContents(content.SubContent, contents.Count + 1, content.Header.Coordinate.Child));
                }
            }
            return lines;
        }
        private IReadOnlyDictionary<string,string> GetHeaderDecomposition(Match m)
        {
            logger.LogDebug("Decomposing: \"{header}\"", m);
            IReadOnlyDictionary<string,string> initial = m.Groups
            .Cast<Group>()
            .Where(g => g.Name == "memberBirthName" || 
                g.Name == "memberBirthDate" ||
                g.Name == "familyDynamicStartDate" ||
                g.Name == "memberDeceasedDate" ||
                g.Name == "inLawBirthName" ||
                g.Name == "inLawBirthDate" ||
                g.Name == "inLawDeceasedDate")
            .ToDictionary(
                g => g.Name,
                g => g.Value
            );
            logger.LogInformation("Initial Decomposed Result:\n{initial}", string.Join('\n', initial.Select(pair => $"{pair.Key} -> {pair.Value}")));
            Dictionary<string,string> final = new(initial);
            logger.LogDebug("Trimming out attributes with empty values.");
            foreach (string name in initial.Keys)
            {
                if (initial[name].Length == 0)
                {
                    final.Remove(name);
                    logger.LogDebug("{name} temporarily has an unknown value.", name);
                }
            }
            bool[] hasDateByPosition = [final.ContainsKey("memberBirthDate"), final.ContainsKey("familyDynamicStartDate"),final.ContainsKey("memberDeceasedDate")];
            bool hasInLaw = final.TryGetValue("inLawBirthName", out string? value) && value.Any(char.IsAsciiLetterUpper) && value.Any(char.IsAsciiLetterLower);
            logger.LogDebug("{memberBirthName} has in-law: {result}", final["memberBirthName"], hasInLaw);
            Person? p = people.FirstOrDefault((person) => person.BirthName == final["memberBirthName"]);
            bool missingFamilyDynamicStartDate;
            if (hasDateByPosition[0] && hasDateByPosition[1] && !hasDateByPosition[2])
            {
                logger.LogDebug("2 Dates are found.");
                missingFamilyDynamicStartDate = p is not null && p.DeceasedDate == new FamilyTreeDate(final["familyDynamicStartDate"]) && hasInLaw;
                if (missingFamilyDynamicStartDate || !hasInLaw)
                {
                    final["memberDeceasedDate"] = final["familyDynamicStartDate"];
                    final.Remove("familyDynamicStartDate");
                    final.Remove("inLawBirthName");
                    final.Remove("inLawBirthDate");
                    final.Remove("inLawDeceasedDate");
                    if (missingFamilyDynamicStartDate)
                    {
                        logger.LogWarning("Missing Family Dynamic Start Date!!!");
                    }
                    else
                    {
                        logger.LogInformation("Since the member doesn't have an in-law, the family dynamic start date is actually the deceased date of the member.");
                    }
                }
            }
            else if (hasDateByPosition[0] && !hasDateByPosition[1] && !hasDateByPosition[2])
            {
                logger.LogDebug("1 Date is found.");
                missingFamilyDynamicStartDate = p is not null && p.BirthDate == new FamilyTreeDate(final["memberBirthDate"]) && hasInLaw;
                if (!missingFamilyDynamicStartDate && hasInLaw)
                {
                    final["familyDynamicStartDate"] = final["memberBirthDate"];
                    final.Remove("memberBirthDate");
                    logger.LogInformation("Since the member has an in-law, this date must be family dynamic start date.");
                }
                else if (missingFamilyDynamicStartDate)
                {
                    logger.LogWarning("Missing Family Dynamic Start Date!!!");
                }
            }
            else if (!hasDateByPosition[0] && !hasDateByPosition[1] && !hasDateByPosition[2])
            {
                logger.LogWarning("No dates are provided.");
            }
            else if (hasDateByPosition[0] && hasDateByPosition[1] && hasDateByPosition[2])
            {
                logger.LogDebug("All dates are provided.");
            }
            else
            {
                throw new InvalidDataException("Invalid Date Arrangement!!!");
            }
            return final;
        }

        private static Regex GetRegexOfGeneration(int generation)
        {
            const string LOWER = @"[a-z]+\.\s";
            const string NUMERICAL = @"\d+\.\s";
            const string PARENTHESIZED_NUMERICAL = @"\(\d+\)\s";
            const string ROMAN_LOWER = @"[ivxlcdm]+\)\s";
            const string ROMAN_UPPER = @"[IVXLCDM]+:\s";
            const string UPPER = @"[A-Z]+\.\s";
            const string DATE_PATTERN = @"(\d{1,2}\s(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s\d{4}(-\d{4})?|(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s\d{4}(-\d{4})?|\d{4}(-\d{4})?)";
            const string NAME_PATTERN = @"[A-Z][A-Za-z\-\,]*(?:\s[A-Z][A-Za-z\-\,]*)*";
            const string LOOKAHEAD = $@"(?=$|\s{LOWER}|\s{NUMERICAL}|\s{PARENTHESIZED_NUMERICAL}|\s{ROMAN_LOWER}|\s{ROMAN_UPPER}|\s{UPPER})";
            return generation switch
            {
                1 => new(@$"{ROMAN_UPPER}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                2 => new(@$"{UPPER}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                3 => new(@$"{NUMERICAL}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                4 => new(@$"{LOWER}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                5 => new(@$"{PARENTHESIZED_NUMERICAL}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                6 => new(@$"{ROMAN_LOWER}(?<memberBirthName>{NAME_PATTERN})(\s(?<memberBirthDate>{DATE_PATTERN}))?(\s(?<familyDynamicStartDate>{DATE_PATTERN}))?(\s(?<memberDeceasedDate>{DATE_PATTERN}))?(\s(?<inLawBirthName>{NAME_PATTERN})(\s(?<inLawBirthDate>{DATE_PATTERN}))?(\s(?<inLawDeceasedDate>{DATE_PATTERN}))?)?{LOOKAHEAD}", RegexOptions.Compiled),
                _ => new(@"^.+$", RegexOptions.Compiled),
            };
        }

        public string ReadTextFile()
        {
            logger.LogInformation("Reading from {FilePath}.", FilePath);
            string temp = File.ReadAllText(FilePath);
            logger.LogDebug("Original Text:\n{templateText}", temp);
            string templateText = Regex.Replace(temp, @"\s+", " ");
            logger.LogDebug("Normalized Test:\n{templateText}", templateText);
            return templateText;
        }
    }
}