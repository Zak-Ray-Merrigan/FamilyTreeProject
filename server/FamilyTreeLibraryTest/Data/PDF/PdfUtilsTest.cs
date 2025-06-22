using FamilyTreeLibrary.Data.PDF;
using FamilyTreeLibrary.Data.PDF.Models;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Data.PDF
{
    public class PdfUtilsTest
    {

        [Test]
        public void TestGetLines1()
        {
            string[] tokens = "I. August Fred Pfingsten  26 Jun 1896  14 Sep 1921  24 Aug 1980  Frieda nee Schobinger  10 Nov 1902  13 Jul 1938".Split(' ');
            Queue<Line> expected = new();
            Line expectedMember = new()
            {
                Name = "August Fred Pfingsten"
            };
            expectedMember.Dates.Enqueue(new FamilyTreeDate("26 Jun 1896"));
            expectedMember.Dates.Enqueue(new FamilyTreeDate("14 Sep 1921"));
            expectedMember.Dates.Enqueue(new FamilyTreeDate("24 Aug 1980"));
            expected.Enqueue(expectedMember);
            Line expectedInLaw = new()
            {
                Name = "Frieda nee Schobinger"
            };
            expectedInLaw.Dates.Enqueue(new FamilyTreeDate("10 Nov 1902"));
            expectedInLaw.Dates.Enqueue(new FamilyTreeDate("13 Jul 1938"));
            expected.Enqueue(expectedInLaw);
            Queue<Line> actual = PdfUtils.GetLines(tokens);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}