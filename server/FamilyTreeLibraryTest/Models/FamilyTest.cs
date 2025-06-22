using FamilyTreeLibrary.Models;
namespace FamilyTreeLibraryTest.Models
{
    public class FamilyTest
    {

        [Test]
        public void TestToString1()
        {
            Person member = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Person inLaw = new("Ronald Merrigan", new FamilyTreeDate("29 Mar 1945"), new FamilyTreeDate("6 Aug 1972"));
            FamilyTreeDate marriageDate = new("25 Jun 1965");
            Family family = new(member, inLaw, marriageDate);
            string expected = "[Margaret Ann Lass (5 Apr 1947 - Present)]-[Ronald Merrigan (29 Mar 1945 - 6 Aug 1972)]: 25 Jun 1965";
            Assert.That(family.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void TestRepresentationConstructor1()
        {
            Person member = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Person inLaw = new("Ronald Merrigan", new FamilyTreeDate("29 Mar 1945"), new FamilyTreeDate("6 Aug 1972"));
            FamilyTreeDate marriageDate = new("25 Jun 1965");
            Family expected = new(member, inLaw, marriageDate);
            Family actual = new(expected.ToString());
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}