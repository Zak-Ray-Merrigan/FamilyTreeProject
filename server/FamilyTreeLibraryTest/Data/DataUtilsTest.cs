using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Driver;
namespace FamilyTreeLibraryTest.Data
{
    public class DataUtilsTest
    {
        [SetUp]
        public void SetUp()
        {
            FamilyTreeUtils.InitializeLogger();
        }

        [Test]
        public void TestPfingstenCollectionConnection()
        {
            Exception problem;
            try
            {
                string familyName = "Pfingsten";
                DataUtils.GetCollection(familyName);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,"Connection Successful.");
                problem = null;
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Fatal,$"Unable to Connect.\n{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                problem = ex;
            }
            Assert.That(problem, Is.Null);
        }

        [Test]
        public void TestGetNodesToBeUpdated1()
        {
            string familyName = "Pfingsten";
            IMongoCollection<BsonDocument> mongoCollection = DataUtils.GetCollection(familyName);
            Family element = new(new Person("Cade Alan Merrigan", new FamilyTreeDate("29 Oct 2000"), FamilyTreeDate.DefaultDate), null, FamilyTreeDate.DefaultDate);
            // Building expected parent match of Cade Alan Merrigan
            ObjectId parentId = new("657f208e782f52784bef9880");
            Person parentMember = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Person parentInlaw = new("Ronald Merrigan", new FamilyTreeDate("29 Mar 1945"), new FamilyTreeDate("6 Aug 1972"));
            FamilyTreeDate parentMarriageDate = new("25 Jun 1965");
            Family parent = new(parentMember, parentInlaw, parentMarriageDate);
            Family parentElement = new(new Person("Brent Alan Merrigan", new FamilyTreeDate("24 Dec 1970"), FamilyTreeDate.DefaultDate),
            new Person("Jolene Joyce Rants", new FamilyTreeDate("12 Jun 1968"), FamilyTreeDate.DefaultDate),
            new FamilyTreeDate("30 Aug 1997"));
            FamilyNode parentNode = new(parentId, parent, parentElement);
            parentNode.Children.Add(element);
            parentNode.Children.Add(new Family(new Person("Kennedy Jo Merrigan", new FamilyTreeDate("6 Nov 2003"), FamilyTreeDate.DefaultDate),
            null, FamilyTreeDate.DefaultDate));
            // Building Expected Element
            ObjectId elementId = new("659428fae024ed23d924f1a5");
            FamilyNode elementNode = new(elementId, parentElement, element);
            IEnumerable<FamilyNode> expected = new List<FamilyNode>()
            {
                parentNode,
                elementNode
            };
            IEnumerable<FamilyNode> actual = DataUtils.GetNodesToBeUpdated(element, mongoCollection);
            Assert.That(actual, Is.EqualTo(expected));
        }

    }
}