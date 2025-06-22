using FamilyTreeLibrary.Data.PDF.OrderingType;

namespace FamilyTreeLibraryTest.Data.PDF.OrderingType
{
    public class AbstractOrderingTypeTest
    {
        [Test]
        public void TestGetOrderingType1()
        {
            int key = 1;
            int generation = 1;
            KeyValuePair<int,string> expected = new(1,"I.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType2()
        {
            int key = 2;
            int generation = 1;
            KeyValuePair<int,string> expected = new(2,"II.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType3()
        {
            int key = 1;
            int generation = 2;
            KeyValuePair<int,string> expected = new(1,"A.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType4()
        {
            int key = 2;
            int generation = 2;
            KeyValuePair<int,string> expected = new(2,"B.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType5()
        {
            int key = 1;
            int generation = 3;
            KeyValuePair<int,string> expected = new(1,"1.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType6()
        {
            int key = 2;
            int generation = 3;
            KeyValuePair<int,string> expected = new(2,"2.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType7()
        {
            int key = 1;
            int generation = 4;
            KeyValuePair<int,string> expected = new(1,"a.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType8()
        {
            int key = 2;
            int generation = 4;
            KeyValuePair<int,string> expected = new(2,"b.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType9()
        {
            int key = 1;
            int generation = 5;
            KeyValuePair<int,string> expected = new(1,"(1)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType10()
        {
            int key = 2;
            int generation = 5;
            KeyValuePair<int,string> expected = new(2,"(2)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType11()
        {
            int key = 1;
            int generation = 6;
            KeyValuePair<int,string> expected = new(1,"i)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType12()
        {
            int key = 2;
            int generation = 6;
            KeyValuePair<int,string> expected = new(2,"ii)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType13()
        {
            int key = 3;
            int generation = 2;
            KeyValuePair<int,string> expected = new(3, "C.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestTryGetOrderingType1()
        {
            string value = "I.";
            int generation = 1;
            Assert.That(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation));
        }

        [Test]
        public void TestTryGetOrderingType2()
        {
            string value = "August";
            int generation = 2;
            Assert.That(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation), Is.False);
        }

        [Test]
        public void TestTryGetOrderingType3()
        {
            string value = "A";
            int generation = 2;
            Assert.That(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation), Is.False);
        }

        [Test]
        public void TestGetOrderingTypeByLine1()
        {
            Queue<AbstractOrderingType> expectedOrderingTypePossibilities = new();
            expectedOrderingTypePossibilities.Enqueue(AbstractOrderingType.GetOrderingType(100, 1));
            expectedOrderingTypePossibilities.Enqueue(AbstractOrderingType.GetOrderingType(3,2));
            string line = "C. Evelyn Pfingsten 14 Mar 1926 07 Aug 1947";
            Queue<AbstractOrderingType> actualOrderingTypePossibilities = AbstractOrderingType.GetOrderingTypeByLine(line);
            Assert.That(actualOrderingTypePossibilities, Is.EqualTo(expectedOrderingTypePossibilities));
        }

        [Test]
        public void TestNextOrderingType1()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType[] actual = AbstractOrderingType.NextOrderingType(Array.Empty<AbstractOrderingType>(), AbstractOrderingType.GetOrderingType(1,1));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestNextOrderingType2()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(1,2)};
            AbstractOrderingType[] current = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(1,2);
            Assert.That(AbstractOrderingType.NextOrderingType(current, orderingType), Is.EqualTo(expected));
        }

        [Test]
        public void TestNextOrderingType3()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(3,2)
            };
            AbstractOrderingType[] current = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(2,2),
                AbstractOrderingType.GetOrderingType(2,3),
                AbstractOrderingType.GetOrderingType(1,4)
            };
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(3,2);
            Assert.That(AbstractOrderingType.NextOrderingType(current, orderingType), Is.EqualTo(expected));
        }

        [Test]
        public void TestPreviousOrderingType1()
        {
            AbstractOrderingType[] expected = Array.Empty<AbstractOrderingType>();
            AbstractOrderingType[] current = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType[] actual = AbstractOrderingType.PreviousOrderingType(current);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}