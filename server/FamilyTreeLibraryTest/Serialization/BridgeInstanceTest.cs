using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibraryTest.Serialization
{
    public class BridgeInstanceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAsNumber()
        {
            BridgeInstance instance = new("1950");
            Assert.That(instance.AsNumber.AsInt, Is.EqualTo(1950));
        }
    }
}