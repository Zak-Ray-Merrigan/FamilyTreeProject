using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Models
{
    public class FamilyTreeDateTest
    {

        [Test]
        public void TestOperatorEquals1()
        {
            Exception problem = null;
            bool result = default;
            try
            {
                FamilyTreeDate dateA = new("26 Jun 1896");
                result = dateA == default;
            }
            catch (Exception ex)
            {
                problem = ex;
            }
            finally
            {
                if (problem is null)
                {
                    Assert.That(result, Is.False);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TestIsDefault1()
        {
            bool result = FamilyTreeDate.IsDefault(default);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestIsDefault2()
        {
            FamilyTreeDate dateA = new("26 Jun 1896");
            bool result = FamilyTreeDate.IsDefault(dateA);
            Assert.That(result, Is.False);
        }
    }
}