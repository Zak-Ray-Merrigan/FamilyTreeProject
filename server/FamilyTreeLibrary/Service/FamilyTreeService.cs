using FamilyTreeLibrary.Data.Databases;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Service
{
    public class FamilyTreeService(FamilyTreeGraph familyTreeGraph, PersonCollection personCollection, FamilyDynamicCollection familyDynamicCollection, 
        FamilyTreeStaticStorage staticStorage, IExtendedLogger<FamilyTreeService> logger, IExtendedLogger<TemplateReader> readerLogger)
    {
        private readonly FamilyTreeGraph familyTreeGraph = familyTreeGraph;
        private readonly PersonCollection personCollection = personCollection;
        private readonly FamilyDynamicCollection familyDynamicCollection = familyDynamicCollection;
        private readonly FamilyTreeStaticStorage staticStorage = staticStorage;
        private readonly IExtendedLogger<FamilyTreeService> logger = logger;
        private readonly IExtendedLogger<TemplateReader> readerLogger = readerLogger;

        public void RevertTree(string blobUri)
        {
            TemplateReader reader = new(staticStorage, readerLogger);
            Template template = reader.ReadTemplate(blobUri);
            RemoveAllNodes(template.FamilyName);
        }

        private void RemoveAllNodes(InheritedFamilyName familyName)
        {
            IEnumerable<FamilyTreeNode> nodes;
            do
            {
                nodes = familyTreeGraph.GetRootNodesByFamilyName(familyName);
                if (nodes.Any())
                {
                    break;
                }
                foreach (FamilyTreeNode node in nodes)
                {
                    Person member = personCollection[node.MemberId];
                    personCollection.Remove(member);
                    Person? inLaw = node.InLawId.HasValue ? personCollection[node.InLawId.Value] : null;
                    if (inLaw is not null)
                    {
                        personCollection.Remove(inLaw);
                    }
                    FamilyDynamic familyDynamic = familyDynamicCollection[node.DynamicId];
                    familyDynamicCollection.Remove(familyDynamic);
                    familyTreeGraph.RemoveNode(node);
                }
            } while(true);
        }

        private class FamilyTreeNodeComparer(PersonCollection personCollection, FamilyDynamicCollection familyDynamicCollection) : IComparer<FamilyTreeNode>
        {
            private readonly PersonCollection personCollection = personCollection;
            private readonly FamilyDynamicCollection familyDynamicCollection = familyDynamicCollection;
            public int Compare(FamilyTreeNode? a, FamilyTreeNode? b)
            {
                if (a is null && b is null)
                {
                    return 0;
                }
                else if (a is null)
                {
                    return -1;
                }
                else if (b is null)
                {
                    return 1;
                }
                Person memberA = personCollection[a.MemberId];
                Person memberB = personCollection[b.MemberId];
                if (memberA < memberB)
                {
                    return -1;
                }
                else if (memberA > memberB)
                {
                    return 1;
                }
                FamilyTreeDate? familyDynamicStartDateA = familyDynamicCollection[a.DynamicId].FamilyDynamicStartDate;
                FamilyTreeDate? familyDynamicStartDateB = familyDynamicCollection[b.DynamicId].FamilyDynamicStartDate;
                if (familyDynamicStartDateA < familyDynamicStartDateB)
                {
                    return -1;
                }
                else if (familyDynamicStartDateA > familyDynamicStartDateB)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}