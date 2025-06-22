using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Models
{
    public class PersonTest
    {
        [Test]
        public void TestCompareTo1()
        {
            Person p1 = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Person p2 = new("Margaret Ann Lass Merrigan", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            Assert.That(p1.CompareTo(p2), Is.EqualTo(0));
        }

        [Test]
        public void TestRepresentationConstructor1()
        {
            Person expected = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            string representation = expected.ToString();
            Person actual = new(representation);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestRepresentationConstructor2()
        {
            Person expected = new("Gabriel Jose Thompson-Guzman", new FamilyTreeDate("15 Jan. 2016"), FamilyTreeDate.DefaultDate);
            Person actual = new(expected.ToString());
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestToString1()
        {
            Person p = new("Margaret Ann Lass", new FamilyTreeDate("5 Apr 1947"), FamilyTreeDate.DefaultDate);
            string expected = "Margaret Ann Lass (5 Apr 1947 - Present)";
            Assert.That(p.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void TestToString2()
        {
            Person p = new("Gabriel Jose Thompson-Guzman", new FamilyTreeDate("15 Jan. 2016"), FamilyTreeDate.DefaultDate);
            string expected = "Gabriel Jose Thompson-Guzman (15 Jan. 2016 - Present)";
            Assert.That(p.ToString(), Is.EqualTo(expected));
        }
    }
}