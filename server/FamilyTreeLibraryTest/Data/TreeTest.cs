using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Data
{
    public class TreeTest
    {
        private ITree familyTree;
        [SetUp]
        public void Setup()
        {
            FamilyTreeUtils.InitializeLogger();
            familyTree = new Tree("Pfingsten");
        }

        [Test]
        public void TestContains()
        {
            Family parent = new(new Person("Brent Alan Merrigan", new FamilyTreeDate("24 Dec 1970"), FamilyTreeDate.DefaultDate),
            new Person("Jolene Joyce Rants", new FamilyTreeDate("12 Jun 1968"), FamilyTreeDate.DefaultDate),
            new FamilyTreeDate("30 Aug 1997"));
            Family child = new(new Person("Cade Alan Merrigan", new FamilyTreeDate("29 Oct 2000"), FamilyTreeDate.DefaultDate), null, FamilyTreeDate.DefaultDate);
            Assert.That(familyTree.Contains(parent, child), Is.True);
        }

        [Test]
        public void TestGetEnumerator()
        {
            Exception problem = null;
            try
            {
                foreach (Family family in familyTree)
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Debug, family is null ? "unknown" : family.ToString());
                }
            }
            catch (Exception ex)
            {
                problem = ex;
                FamilyTreeUtils.LogMessage(LoggingLevels.Fatal,$"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Assert.That(problem, Is.Null);
            }
        }

        [Test]
        public void TestGetParent1()
        {
            Family child = new("Cade Alan Merrigan (29 Oct 2000 - Present)");
            Family expectedParent = new("[Brent Alan Merrigan (24 Dec 1970 - Present)]-[Jolene Joyce Rants (12 Jun 1968 - Present)]: 30 Aug 1997");
            Assert.That(familyTree.GetParent(child), Is.EqualTo(expectedParent));
        }
    }
}