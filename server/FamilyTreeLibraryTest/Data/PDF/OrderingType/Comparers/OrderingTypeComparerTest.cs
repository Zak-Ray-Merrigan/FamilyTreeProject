using FamilyTreeLibrary.Data.PDF.OrderingType;

namespace FamilyTreeLibraryTest.Data.PDF.OrderingType.Comparers
{
    public class OrderingTypeComparerTest
    {
        [Test]
        public void TestOrderingTypeArrayEquals1()
        {
            AbstractOrderingType[] orderingType1 = {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(5,2), AbstractOrderingType.GetOrderingType(3,3)};
            AbstractOrderingType[] orderingType2 = {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(5,2), AbstractOrderingType.GetOrderingType(3,3)};
            Assert.That(orderingType1, Is.EqualTo(orderingType2));
        }
    }
}