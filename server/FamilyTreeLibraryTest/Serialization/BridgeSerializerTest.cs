using FamilyTreeLibrary.Serialization;
using FamilyTreeLibrary.Serialization.Models;
using System.Text.Json;

namespace FamilyTreeLibraryTest.Serialization
{
    [TestFixture]
    public class BridgeSerializerTest
    {
        private BridgeSerializer serializer;
        private JsonSerializerOptions options;
        [SetUp]
        public void Setup()
        {
            serializer = new();
            options = new()
            {
                Converters = {
                    serializer
                }
            };
        }

        [Test]
        public void TestDeserializeNull()
        {
            string json = "null";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.IsNull, Is.True);
        }

        [Test]
        public void TestSerializeNull()
        {
            string expected = "null";
            string actual = JsonSerializer.Serialize<IBridge>(new Bridge(), options);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestDeserializeString()
        {
            string json = "\"Zak Ray Merrigan\"";
            IBridge expectedBridge = new Bridge("Zak Ray Merrigan");
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsString, Is.EqualTo(expectedBridge.Instance.AsString));
        }

        [Test]
        public void TestSerializeString()
        {
            string expected = "\"Zak Ray Merrigan\"";
            string actual = JsonSerializer.Serialize<IBridge>(new Bridge("Zak Ray Merrigan"), options);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestDeserializeBooleanTrue()
        {
            string json = "true";
            Assert.That(JsonSerializer.Deserialize<IBridge>(json, options)!.Instance.AsBoolean, Is.True);
        }

        [Test]
        public void TestDeserializeBooleanFalse()
        {
            string json = "false";
            Assert.That(JsonSerializer.Deserialize<IBridge>(json, options)!.Instance.AsBoolean, Is.False);
        }

        [Test]
        public void TestSerializeBooleanTrue()
        {
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(true), options), Is.EqualTo("true"));
        }

        [Test]
        public void TestSerializeBooleanFalse()
        {
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(false), options), Is.EqualTo("false"));
        }

        [Test]
        public void TestDeserializeInt1()
        {
            string json = "2";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsInt, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeInt2()
        {
            string json = "2.0";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsInt, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeInt3()
        {
            long value = (long)int.MaxValue + 1;
            string json = $"{value}";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.Throws<InvalidCastException>(() => {
                int i = actualBridge.Instance.AsNumber.AsInt;
            });
        }

        [Test]
        public void TestDeserializeInt4()
        {
            string json = "2.5";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.Throws<InvalidCastException>(() =>  {int i = actualBridge.Instance.AsNumber.AsInt; });
        }

        [Test]
        public void TestDeserializeLong1()
        {
            string json = "2";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsLong, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeLong2()
        {
            string json = "2.0";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsLong, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeLong3()
        {
            long value = (long)int.MaxValue + 1;
            string json = $"{value}";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsLong, Is.EqualTo(value));
        }

        [Test]
        public void TestDeserializeLong4()
        {
            string json = "2.5";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.Throws<InvalidCastException>(() =>  {long l = actualBridge.Instance.AsNumber.AsLong; });
        }

        [Test]
        public void TestDeserializeDouble1()
        {
            string json = "2";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsDouble, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeDouble2()
        {
            string json = "2.0";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsDouble, Is.EqualTo(2));
        }

        [Test]
        public void TestDeserializeDouble3()
        {
            long value = (long)int.MaxValue + 1;
            string json = $"{value}";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsDouble, Is.EqualTo(value));
        }

        [Test]
        public void TestDeserializeDouble4()
        {
            string json = "2.5";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsNumber.AsDouble, Is.EqualTo(2.5));
        }
        
        [Test]
        public void TestSerializeNumber1()
        {
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(new Number(2)), options), Is.EqualTo("2"));
        }

        [Test]
        public void TestSerializeNumber2()
        {
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(new Number(2.0)), options), Is.EqualTo("2"));
        }

        [Test]
        public void TestSerializeNumber3()
        {
            long value = (long)int.MaxValue + 1;
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(new Number(value)), options), Is.EqualTo($"{value}"));
        }

        [Test]
        public void TestSerializeNumber4()
        {
            Assert.That(JsonSerializer.Serialize<IBridge>(new Bridge(new Number(2.5)), options), Is.EqualTo("2.5"));
        }

        [Test]
        public void TestDeserializeEmptyArray()
        {
            string json = "[]";
            IBridge actualBridge = JsonSerializer.Deserialize<IBridge>(json, options)!;
            Assert.That(actualBridge.Instance.AsArray, Is.EqualTo(Array.Empty<BridgeInstance>()));
        }

        [Test]
        public void TestDeserializeArrayWithContents()
        {
            string json = "[\"Merrigan\", \"Drazenovic\", \"Pfingsten\"]";
            IEnumerable<BridgeInstance> expected = [new BridgeInstance("Merrigan"), new("Drazenovic"), new("Pfingsten")];
            IEnumerable<BridgeInstance> actual = JsonSerializer.Deserialize<IBridge>(json, options)!.Instance.AsArray;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestDeserializeObject1()
        {
            string json = "{\"attributeA\": \"valueA\", \"attributeB\": \"valueB\"}";
            IDictionary<string,BridgeInstance> expected = new Dictionary<string,BridgeInstance>()
            {
                ["attributeA"] = new("valueA"),
                ["attributeB"] = new("valueB")
            };
            IDictionary<string,BridgeInstance> actual = JsonSerializer.Deserialize<IBridge>(json, options)!.Instance.AsObject;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}