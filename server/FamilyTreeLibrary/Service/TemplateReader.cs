using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FamilyTreeLibrary.Service
{
    public class TemplateReader(FamilyTreeStaticStorage staticStorage, IExtendedLogger<TemplateReader> logger)
    {
        private readonly IExtendedLogger<TemplateReader> logger = logger;
        private readonly FamilyTreeStaticStorage staticStorage = staticStorage;
        private readonly ISet<Person> people = new HashSet<Person>();
        private const string EN_DASH = "\u2013";

        public Template ReadTemplate(string blobUri)
        {
            logger.LogInformation("Given \"{url}\", we are loading the contents in memory.", blobUri);
            logger.LogDebug("Reading {url}...", blobUri);
            List<Line> family = [];
            using Stream templateStream = staticStorage.GetStream(blobUri);
            using PdfReader reader = new(templateStream);
            using PdfDocument template = new(reader);
            logger.LogDebug("Contents are being populated.");
            int pageCount = template.GetNumberOfPages();
            string containerName = blobUri.Split('/')[^2];
            logger.LogDebug("Inherited Family Name: {familyName}", containerName);
            BridgeInstance instance = new(containerName);
            for (int p = 1; p <= pageCount; p++)
            {
                string pageText = PdfTextExtractor.GetTextFromPage(template.GetPage(p));
                logger.LogDebug("From page number {p}: {pageText}", p, pageText);
                string[] pageLines = GetPageLines(pageText);
                foreach (string pageLine in pageLines)
                {
                    Line line = GetLine(pageLine);
                    ISet<InheritedFamilyName> inheritedFamilyNames = new HashSet<InheritedFamilyName>()
                    {
                        new(instance)
                    };
                    Person member = FamilyTreeUtils.GetPerson(logger, inheritedFamilyNames.First(), people, line.Member);
                    Person? inLaw = line.InLaw is null ? null : FamilyTreeUtils.GetPerson(logger, inheritedFamilyNames.First(), people, line.InLaw);
                    family.Add(new(line.Coordinate, member, inLaw, line.FamilyDynamic));
                }
            }
            return new Template()
            {
                Family = family,
                FamilyName = new(instance)
            };
        }

        private string[] GetPageLines(string pageText)
        {
            logger.LogInformation("Normalization Stage");
            logger.LogDebug("Normalizing the text.");
            string spaceNormalization = Regex.Replace(pageText, @"\s+", " ").Trim();
            logger.LogDebug("SPACE NORMALIZATION:\n {spaceNormalization}", spaceNormalization);
            string enDashCleaningPattern = @"(?<=\d{1,2} (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}(-\d{4})?|(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}(-\d{4})?|\d{4}(-\d{4})?)\s*[-–—]\s*(?=Present|\d{1,2} (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}(-\d{4})?|(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) \d{4}(-\d{4})?|\d{4}(-\d{4})?)";
            string enDashNormalization = Regex.Replace(spaceNormalization, enDashCleaningPattern, m => $" {EN_DASH} ");
            logger.LogDebug("EN_DASH NORMALIZATION:\n{enDashNormalization}", enDashNormalization);
            string emptyLifespanPattern = @"\(\s*[-–—]\s*\)";
            string emptyLifespanNormalization = Regex.Replace(enDashNormalization, emptyLifespanPattern, m => $"({EN_DASH})");
            logger.LogDebug("EMPTY LIFESPAN NORMALIZATION:\n{emptyLifespanNormalization}", emptyLifespanNormalization);
            Queue<int> indices = new();
            for (int i = 0; i < emptyLifespanNormalization.Length; i++)
            {
                if (emptyLifespanNormalization[i] == '[')
                {
                    indices.Enqueue(i);
                }
            }
            int count = indices.Count;
            logger.LogInformation("Found {count} family tree nodes on this page", count);
            string[] results = new string[count];
            int index = 0;
            while (indices.TryDequeue(out int start))
            {
                int end = indices.TryPeek(out int value) ? value : emptyLifespanNormalization.Length;
                results[index] = emptyLifespanNormalization[start..end].Trim();
                logger.LogDebug("{familyTreeNode}", results[index++]);
            }
            return results;
        }

        private Line GetLine(string text)
        {
            logger.LogInformation("Line: \"{text}\" is being analyzed.", text);
            char[] splitArgs = ['[',']','(', ')', EN_DASH[0], '&', ':'];
            BridgeInstance emptyInstance = new();
            HierarchialCoordinate emptyCoordinate = new();
            HierarchialCoordinate coordinate = emptyCoordinate;
            IDictionary<string,BridgeInstance> memberObj = new Dictionary<string,BridgeInstance>();
            IDictionary<string,BridgeInstance> inLawObj = new Dictionary<string,BridgeInstance>();
            IDictionary<string,BridgeInstance> familyDynamicObj = new Dictionary<string,BridgeInstance>();
            logger.LogInformation("Setting up for decomposition...");
            string[] segments = [.. text.Split(splitArgs).Skip(1).Select(segment => segment.Trim())];
            for (int i = 0; i < segments.Length; i++)
            {
                switch (i)
                {
                    case 0: coordinate = new([.. segments[i].Split('.').Select(label => Convert.ToInt32(label))]); logger.LogDebug("Hierarchial Coordinate: {coordinate}", coordinate); break;
                    case 1: memberObj["birthName"] = new(segments[i]); logger.LogDebug("Member Birth Name: {birthName}", memberObj["birthName"]); break;
                    case 2: memberObj["birthDate"] = segments[i].Length > 0 ? new(segments[i]) : emptyInstance;
                        if (memberObj["birthDate"].IsNull)
                        {
                            logger.LogWarning("Birth Date of {name}: {birthDate}", memberObj["birthName"], memberObj["birthDate"]);
                        }
                        else
                        {
                            logger.LogDebug("Birth Date of {name}: {birthDate}", memberObj["birthName"], memberObj["birthDate"]);
                        }
                        break;
                    case 3: memberObj["deceasedDate"] = segments[i].Length > 0 && segments[i] != "Present" ? new(segments[i]) : emptyInstance; logger.LogDebug("Deceased Date of {name}: {deceasedDate}", memberObj["birthName"], memberObj["deceasedDate"]); break;
                    case 5: inLawObj["birthName"] = new(segments[i]); logger.LogDebug("In-Law Birth Name: {birthName}", inLawObj["birthName"]); break;
                    case 6: inLawObj["birthDate"] = segments[i].Length > 0 ? new(segments[i]) : emptyInstance;
                        if (inLawObj["birthDate"].IsNull)
                        {
                            logger.LogWarning("Birth Date of {name}: {birthDate}", inLawObj["birthName"], inLawObj["birthDate"]);
                        }
                        else
                        {
                            logger.LogDebug("Birth Date of {name}: {birthDate}", inLawObj["birthName"], inLawObj["birthDate"]);
                        }
                        break;
                    case 7: inLawObj["deceasedDate"] = segments[i].Length > 0 && segments[i] != "Present" ? new(segments[i]) : new(); logger.LogDebug("Deceased Date of {name}: {deceasedDate}", inLawObj["birthName"], inLawObj["deceasedDate"]); break;
                    case 9: familyDynamicObj["familyDynamicStartDate"] = segments[i].Length > 0 ? new(segments[i]) : emptyInstance; break;
                }
            }
            if (coordinate == emptyCoordinate)
            {
                throw new InvalidDataException("The coordinate is missing.");
            }
            if (!(memberObj.TryGetValue("birthName" , out BridgeInstance memberBirthName) && memberBirthName.IsString))
            {
                throw new InvalidDataException("The Member Birth Name is missing");
            }
            Person member = new(memberObj, true);
            if (inLawObj.TryGetValue("birthName", out BridgeInstance inLawBirthName) && !inLawBirthName.IsString)
            {
                throw new InvalidDataException("The InLaw Birth Name is missing.");
            }
            Person? inLaw = inLawBirthName.IsString ? new(inLawObj, true) : null;
            if (familyDynamicObj.TryGetValue("familyDynamicStartDate", out BridgeInstance familyDynamicStartDate))
            {
                if (!familyDynamicStartDate.IsString)
                {
                    throw new InvalidDataException("The Family Dynamic Start Date is missing.");
                }
                else if (inLaw is null)
                {
                    throw new InvalidOperationException("An InLaw must be exist if a family dynamic is defined.");
                }
                familyDynamicObj["pageTitle"] = new($"This is the family of {member.BirthName} and {inLaw.BirthName}.");
            }
            FamilyDynamic? familyDynamic = familyDynamicStartDate.IsString ? new(familyDynamicObj, true) : null;
            return new(coordinate, member, inLaw, familyDynamic);
        }
    }
}