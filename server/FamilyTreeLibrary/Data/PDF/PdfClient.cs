using FamilyTreeLibrary.Data.PDF.OrderingType;
using FamilyTreeLibrary.Data.PDF.Models;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.PDF
{
    public class PdfClient
    {
        private readonly ICollection<FamilyNode> nodes;

        public PdfClient(FileInfo file)
        {
            Family root = new(new(null, FamilyTreeDate.DefaultDate, FamilyTreeDate.DefaultDate), null, FamilyTreeDate.DefaultDate);
            FamilyNode rootNode = new(null, root);
            File = file;
            nodes = new SortedSet<FamilyNode>()
            {
                rootNode
            };
            Root = new Section(Array.Empty<AbstractOrderingType>(), rootNode);
            FamilyNodeCollection = new List<Section>();
        }

        public FileInfo File
        {
            get;
        }

        public IEnumerable<FamilyNode> Nodes
        {
            get
            {
                return nodes;
            }
        }

        private ICollection<Section> FamilyNodeCollection
        {
            get;
        }

        private Section Root
        {
            get;
        }

        public void LoadNodes()
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"Reading {File.FullName}.");
            AbstractOrderingType[] currentOrderingType = Array.Empty<AbstractOrderingType>();
            Queue<AbstractOrderingType> previousPossibilities = new();
            string previousLine = "";
            int sectionNumber = 1;
            IReadOnlyCollection<string> pdfLines = PdfUtils.GetLinesFromDocument(File);
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"{pdfLines.Count} lines were detected.");
            foreach (string line in pdfLines)
            {
                try
                {
                    string currentLine = line.Trim();
                    Queue<AbstractOrderingType> currentPossibilities = AbstractOrderingType.GetOrderingTypeByLine(currentLine, pdfLines.Count);
                    if (PdfUtils.IsInLaw(currentPossibilities, previousLine, currentLine))
                    {
                        previousLine += $"  {currentLine}";
                    }
                    else if (PdfUtils.IsMember(currentPossibilities))
                    {
                        CreateNode(previousLine, previousPossibilities, ref currentOrderingType, ref sectionNumber);
                        previousLine = currentLine;
                        previousPossibilities = currentPossibilities;
                    }
                }
                catch (Exception ex)
                {
                    throw new SystemException($"{ex.GetType().Name} on Section #{++sectionNumber}: {ex.Message}\n{ex.StackTrace}", ex);
                }
            }
            CreateNode(previousLine, previousPossibilities, ref currentOrderingType, ref sectionNumber);
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"{FamilyNodeCollection.Count} sections were detected.");
        }

        public void AttachNodes()
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "Nodes are connecting.");
            AttachNodes(FamilyNodeCollection, Root);
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "Nodes are connected.");
        }

        private void AttachNodes(IEnumerable<Section> familyNodeCollection, Section parent)
        {
            ICollection<Section> subFamilyNodeCollection = new List<Section>();
            Section previousFamilyNode = parent;
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"Attaching children of {PdfUtils.GetPersonLogName(parent)}.");
            foreach (Section familyNode in familyNodeCollection)
            {
                if (familyNode.OrderingType.Length == parent.OrderingType.Length + 1)
                {
                    parent.Node.Children.Add(familyNode.Node.Element);
                    FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"{PdfUtils.GetPersonLogName(parent)} has a child named {PdfUtils.GetPersonLogName(familyNode)}:\n{parent}");
                    familyNode.Node.Parent = parent.Node.Element;
                    FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"{PdfUtils.GetPersonLogName(familyNode)} has a parent named {PdfUtils.GetPersonLogName(parent)}:\n{familyNode}");
                    nodes.Add(familyNode.Node);
                    if (previousFamilyNode != parent)
                    {
                        Section tempFamilyNode = previousFamilyNode;
                        AttachSubNodes(ref subFamilyNodeCollection, tempFamilyNode);
                    }
                    previousFamilyNode = familyNode;
                }
                else
                {
                    subFamilyNodeCollection.Add(familyNode);
                }
            }
            AttachSubNodes(ref subFamilyNodeCollection, previousFamilyNode);
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"The children of {PdfUtils.GetPersonLogName(parent)} have been attached.");
        }

        private void AttachSubNodes(ref ICollection<Section> subFamilyNodeCollection, Section parent)
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"Analyzing sub-sections of {PdfUtils.GetPersonLogName(parent)}.");
            if (subFamilyNodeCollection.Any())
            {
                AttachNodes(subFamilyNodeCollection.AsEnumerable(), parent);
                subFamilyNodeCollection.Clear();
            }
            FamilyTreeUtils.LogMessage(LoggingLevels.Debug, $"Sub-section analysis of {PdfUtils.GetPersonLogName(parent)} complete.");
        }

        private void CreateNode(string previousLine, Queue<AbstractOrderingType> previousPossibilities, ref AbstractOrderingType[] currentOrderingType, ref int sectionNumber)
        {
            if (previousLine != "" && previousPossibilities.Count > 0)
            {
                string[] tokens = PdfUtils.ReformatLine(previousLine);
                Queue<Line> lines = PdfUtils.GetLines(tokens);
                Family node = PdfUtils.GetFamily(lines);
                AbstractOrderingType[] temp = currentOrderingType;
                currentOrderingType = PdfUtils.AddSection(FamilyNodeCollection, temp, previousPossibilities, node, ref sectionNumber);
            }
            else if (previousLine != "")
            {
                throw new InvalidOperationException("The ordering type is undefined.");
            }
        }
    }
}