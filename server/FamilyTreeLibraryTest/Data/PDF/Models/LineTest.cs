using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Data.PDF.Models;

namespace FamilyTreeLibraryTest.Data.PDF.Models
{
    public class LineTest
    {

        [Test]
        public void TestClone()
        {
            Line expected = new()
            {
                Name = "August Fred Pfingsten"
            };
            expected.Dates.Enqueue(new FamilyTreeDate("26 Jun 1896"));
            expected.Dates.Enqueue(new FamilyTreeDate("14 Sep 1921"));
            expected.Dates.Enqueue(new FamilyTreeDate("24 Aug 1980"));
            Assert.That(expected.Clone(), Is.EqualTo(expected));
        }
    }
}