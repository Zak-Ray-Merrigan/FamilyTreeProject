using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.Comparers;
using FamilyTreeLibrary.Data.PDF;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Service
{
    public class FamilyTreeService
    {
        public FamilyTreeService(string familyName)
        {
            try
            {
                FamilyTree = new Tree(familyName);
            }
            catch (ArgumentNullException ex)
            {
                throw new ClientNotFoundException("The family doesn't exist.", ex);
            }
        }

        public string Name
        {
            get
            {
                return FamilyTree.Name;
            }
        }

        public int NumberOfGenerations
        {
            get
            {
                return FamilyTree.Height - 1;
            }
        }

        public long NumberOfFamilies
        {
            get
            {
                return FamilyTree.Count - 1;
            }
        }

        public void AppendTree(FileInfo templateFile)
        {
            try
            {
                PdfClient client = new(templateFile);
                client.LoadNodes();
                client.AttachNodes();
                if (client.Nodes.Any() && client.Nodes.First().Parent is not null)
                {
                    throw new InvalidOperationException("The root must be the first node to be analyzed.");
                }
                foreach (FamilyNode node in client.Nodes)
                {
                    string parentPortion;
                    if (node.Parent is null)
                    {
                        parentPortion = "null";
                    }
                    else if (node.Parent.Member.Name is null)
                    {
                        parentPortion = "root";
                    }
                    else
                    {
                        parentPortion = node.Parent.InLaw is null ? node.Parent.Member.Name : $"{node.Parent.Member.Name}/{node.Parent.InLaw.Name}";
                    }
                    string childPortion;
                    if (node.Element.Member.Name is null)
                    {
                        childPortion = "root";
                    }
                    else
                    {
                        childPortion = node.Element.InLaw is null ? node.Element.Member.Name : $"{node.Element.Member.Name}/{node.Element.InLaw.Name}";
                    }
                    if ((node.Parent is null && !FamilyTree.Any()) || !FamilyTree.Contains(node.Parent, node.Element))
                    {
                        FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"Adding the parent-child relationship between {parentPortion} and {childPortion}.");
                        FamilyTree.Add(node.Id, node.Parent, node.Element);
                        FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"The parent-child relationship between {parentPortion} and {childPortion} has been added.");
                    }
                    FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The parent-child relationship between {parentPortion} and {childPortion} exists in the tree.");
                }
                long pdfNodesCount = client.Nodes.LongCount();
                long treeNodesCount = FamilyTree.Count;
                if (pdfNodesCount != treeNodesCount)
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Warning, $"Only {treeNodesCount} of {pdfNodesCount} were analyzed.");
                }
                else
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"{treeNodesCount} nodes have been analyzed.");
                }
            }
            catch (IOException ex)
            {
                throw new ClientNotFoundException("The provided file isn't found in your directory. Try to upload again.", ex);
            }
        }

        public IEnumerable<Family> FilterTree(SortingOptions option, string name)
        {
            IEnumerable<Family> families = option switch
            {
                SortingOptions.ParentFirstThenChildren => FamilyTree,
                SortingOptions.AscendingByName => FamilyTree.Order(new NameAscendingComparer()),
                _ => throw new ClientBadRequestException("There is nothing to show if you don't select an ordering option."),
            };
            if (name is not null && name != "")
            {
                return families.Where((family) =>
                {
                    return family.Member.Name is not null && family.Member.Name.Contains(name, StringComparison.OrdinalIgnoreCase);
                });
            }
            return families;
        }

        public int GenerationNumberOf(Family family)
        {
            return FamilyTree.Depth(family);
        }

        public IEnumerable<Family> GetSubtree(SortingOptions orderOption, string memberName, Family family)
        {
            IEnumerable<Family> initial = orderOption switch
            {
                SortingOptions.ParentFirstThenChildren => FamilyTree.Subtree(family),
                SortingOptions.AscendingByName => FamilyTree.Subtree(family).Order(new NameAscendingComparer()),
                _ => throw new ClientBadRequestException("There is nothing to show if you don't select an ordering option.")
            };
            if (memberName is not null && memberName != "")
            {
                return initial.Where((fam) => 
                {
                    return fam.Member.Name is not null && fam.Member.Name.Contains(memberName, StringComparison.OrdinalIgnoreCase);
                });
            }
            return initial;
        }

        public void ReportChildren(Family parent, Family child)
        {
            if (parent is null)
            {
                throw new ClientBadRequestException($"The child has to be a parent of somebody in the {FamilyTree.Name} family tree.");
            }
            else if (child is null)
            {
                throw new ClientBadRequestException("The child you're trying to report doesn't exist.");
            }
            else if (!FamilyTree.Any(node => node == parent) && !FamilyTree.Any(node => node == child))
            {
                throw new ClientBadRequestException($"Either {parent.Member.Name} or {child.Member.Name} must be genetically part of the {FamilyTree.Name} Family Tree.");
            }
            else if (FamilyTree.Contains(parent, child))
            {
                throw new ClientBadRequestException($"{parent.Member.Name} already has a child named {child.Member.Name}");
            }
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Adding the parent-child relationship between {parent.Member.Name} and {child.Member.Name}");
            FamilyTree.Add(default, parent, child);
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"{parent.Member.Name} has a child named {child.Member.Name}");
        }

        public void ReportDeceased(Person p, FamilyTreeDate deceasedDate)
        {
            if (p is null)
            {
                throw new ClientBadRequestException("The person you're trying to report doesn't exist.");
            }
            else if (deceasedDate == default || deceasedDate == FamilyTreeDate.DefaultDate)
            {
                throw new ClientBadRequestException("The deceased date provided doesn't exist.");
            }
            IEnumerable<Family> families = FamilyTree.Where((node) => node.Member == p || node.InLaw == p);
            if (!families.Any())
            {
                throw new ClientBadRequestException($"{p.Name} isn't found in the tree.");
            }
            foreach (Family fam in families)
            {
                Family updatedFam = (Family)fam.Clone();
                if (updatedFam.Member == p)
                {
                    updatedFam.Member.DeceasedDate = deceasedDate;
                }
                else if (updatedFam.InLaw == p)
                {
                    updatedFam.InLaw.DeceasedDate = deceasedDate;
                }
                FamilyTree.Update(fam, updatedFam);
            }
        }

        public void ReportMarried(Person member, Person inLaw, FamilyTreeDate marriageDate)
        {
            if (member is null)
            {
                throw new ClientBadRequestException("The biological member you're trying to report doesn't exist.");
            }
            else if (inLaw is null)
            {
                throw new ClientBadRequestException($"{member.Name} can't married to someone who doesn't exist.");
            }
            else if (marriageDate == default || marriageDate == FamilyTreeDate.DefaultDate)
            {
                throw new ClientBadRequestException("An unknown marriage date was entered.");
            }
            IList<Family> familyCollection = FamilyTree.ToList();
            IList<Family> families = familyCollection.Where(node => node.Member == member).ToList();
            if (!families.Any())
            {
                throw new ClientBadRequestException($"{member.Name} isn't a member of the tree.");
            }
            Family family = families.Where(fam => fam.InLaw == inLaw).FirstOrDefault();
            if (family is null && (families.Count > 1 || families[0].InLaw is not null))
            {
                Family additionalFamily = new(member, inLaw, marriageDate);
                Family node = (Family)families[0].Clone();
                Family parent = FamilyTree.GetParent(node);
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "An additional partnership is being added.");
                FamilyTree.Add(default, parent, additionalFamily);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, "The additional partnership has been applied.");
            }
            else if (family is null)
            {
                Family initial = familyCollection.Where(node => node == families[0]).First();
                Family final = new(initial.Member, inLaw, marriageDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"The node: {initial} is being updated.");
                FamilyTree.Update(initial, final);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The node has been updated to {final}");
            }
            else
            {
                Family initial = familyCollection.Where(node => node == family).First();
                Family final = new(initial.Member, initial.InLaw, marriageDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"The marriage date is being changed from {initial.MarriageDate} to {final.MarriageDate}.");
                FamilyTree.Update(initial, final);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, "The marriage date update was successful");
            }
        }

        public IEnumerable<Family> RetrieveChildrenOf(Family element)
        {
            return FamilyTree.GetChildren(element);
        }

        public Family RetrieveParentOf(Family element)
        {
            Family parent = FamilyTree.GetParent(element);
            if (parent is null || parent == Family.EmptyFamily)
            {
                throw new ClientNotFoundException($"{element.Member.Name} has an unknown parent.");
            }
            return parent;
        }

        public void RevertTree(FileInfo templateFile)
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {FamilyTree.Name} family tree is being emptied.");
            FamilyTree.Clear();
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"The {FamilyTree.Name} is now empty.");
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {FamilyTree.Name} family tree is now being reverted based on the template file path: \"{templateFile.FullName}\".");
            AppendTree(templateFile);
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {FamilyTree.Name} family tree has been reverted successfully.");
        }

        private ITree FamilyTree
        {
            get;
        }
    }
}