using FamilyTreeLibrary.Enumerators;
using FamilyTreeLibrary.Serialization.Models;
using iText.StyledXmlParser.Css.Selector.Item;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Serialization
{
    public readonly partial struct BridgeInstance: IEquatable<BridgeInstance>
    {
        private readonly object? instance;

        public BridgeInstance()
        {
            instance = null;
        }

        public BridgeInstance(string text)
        {
            instance = text;
        }

        public BridgeInstance(Number num)
        {
            instance = num;
        }

        public BridgeInstance(bool value)
        {
            instance = value;
        }

        public BridgeInstance(IEnumerable<BridgeInstance> array)
        {
            instance = array;
        }

        public BridgeInstance(IDictionary<string,BridgeInstance> obj)
        {
            instance = obj;
        }

        public IEnumerable<BridgeInstance> AsArray
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidCastException("The instance doesn't exist.");
                }
                return (IEnumerable<BridgeInstance>)instance;
            }
        }
        
        public bool AsBoolean
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidCastException("The instance doesn't exist.");
                }
                string? representation = instance.ToString() ?? throw new InvalidCastException("The instance doesn't exist.");
                if (representation.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                else if (representation.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
                throw new InvalidCastException($"{representation} isn't a boolean.");
            }
        }

        public Number AsNumber
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidCastException("The instance doesn't exist.");
                }
                string? representation = instance.ToString() ?? throw new InvalidCastException("The instance doesn't exist.");
                if (Digits().Matches(representation).Count < 1 && DigitsWithDecimal().Matches(representation).Count < 1 && 
                    NegativeDigits().Matches(representation).Count < 1 && NegativeDigitsWithDecimal().Matches(representation).Count < 1)
                {
                    throw new InvalidCastException($"{representation} isn't a number.");
                }
                return new(Convert.ToDouble(representation));
            }
        }

        public IDictionary<string, BridgeInstance> AsObject
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidCastException("This instance doesn't exist.");
                }
                return (IDictionary<string, BridgeInstance>)instance;
            }
        }

        public string AsString
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidCastException("The instance doesn't exist.");
                }
                string representation = instance.ToString() ?? throw new InvalidCastException("The instance doesn't exist.");
                return representation;
            }
        }

        public bool IsArray
        {
            get
            {
                return instance is IEnumerable<BridgeInstance>;
            }
        }

        public bool IsBoolean
        {
            get
            {
                if (instance is null)
                {
                    return false;
                }
                string? representation = instance.ToString();
                if (representation is null)
                {
                    return false;
                }
                return representation.Equals("true", StringComparison.CurrentCultureIgnoreCase) || representation.Equals("false", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public bool IsNumber
        {
            get
            {
                if (instance is null)
                {
                    return false;
                }
                string? representation = instance.ToString();
                if (representation is null)
                {
                    return false;
                }
                return Digits().Matches(representation).Count > 0 || DigitsWithDecimal().Matches(representation).Count > 0 || 
                    NegativeDigits().Matches(representation).Count > 0 || NegativeDigitsWithDecimal().Matches(representation).Count > 0;
            }
        }

        public bool IsNull
        {
            get
            {
                return instance is null;
            }
        }

        public bool IsObject
        {
            get
            {
                return instance is IDictionary<string, BridgeInstance>;
            }
        }

        public bool IsString
        {
            get
            {
                return instance is string;
            }
        }

        public bool Equals(BridgeInstance other)
        {
            if (IsNull && other.IsNull)
            {
                return true;
            }
            else if (IsBoolean && other.IsBoolean)
            {
                return (AsBoolean && other.AsBoolean) || !(AsBoolean || other.AsBoolean);
            }
            else if (IsNumber && other.IsNumber)
            {
                return AsNumber.AsDouble == other.AsNumber.AsDouble;
            }
            else if (IsString && other.IsString)
            {
                return AsString == other.AsString;
            }
            else if (IsArray && other.IsArray)
            {
                if (AsArray.Count() != other.AsArray.Count())
                {
                    return false;
                }
                foreach (BridgeInstance element in AsArray)
                {
                    if (!other.AsArray.Contains(element))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (IsObject && other.IsObject)
            {
                if (AsObject.Count != other.AsObject.Count)
                {
                    return false;
                }
                foreach (string attribute in AsObject.Keys)
                {
                    if (!(other.AsObject.ContainsKey(attribute) && AsObject[attribute].Equals(other.AsObject[attribute])))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return obj is BridgeInstance other && Equals(other);
        }

        public override int GetHashCode()
        {
            IEnumerator<int> primeEnumerator = new PrimeNumberEnumerator();
            int sum = 0;
            if (TryGetArray(out IEnumerable<BridgeInstance> array))
            {
                foreach (BridgeInstance element in array)
                {
                    if (!primeEnumerator.MoveNext())
                    {
                        throw new NotSupportedException("Out of primes.");
                    }
                    sum += primeEnumerator.Current * element.GetHashCode();
                    primeEnumerator.Dispose();
                }
                return sum;
            }
            else if (TryGetBoolean(out bool b))
            {
                return b.GetHashCode();
            }
            else if (TryGetNumber(out Number num))
            {
                return num.AsDouble.GetHashCode();
            }
            else if (TryGetObject(out IDictionary<string,BridgeInstance> obj))
            {
                foreach (KeyValuePair<string,BridgeInstance> kv in obj)
                {
                    if (!primeEnumerator.MoveNext())
                    {
                        throw new NotSupportedException("Out of primes.");
                    }
                    sum += primeEnumerator.Current * kv.GetHashCode();
                }
                return sum;
            }
            else if (TryGetString(out string text))
            {
                return text.GetHashCode();
            }
            return 0;
        }

        public override string ToString()
        {
            JsonSerializerOptions options = new()
            {
                Converters = {
                    new BridgeSerializer()
                },
                WriteIndented = true
            };
            if (TryGetArray(out IEnumerable<BridgeInstance> array))
            {
                return JsonSerializer.Serialize<IBridge>(new Bridge(array), options);
            }
            else if (TryGetBoolean(out bool b))
            {
                return JsonSerializer.Serialize<IBridge>(new Bridge(b), options);
            }
            else if (TryGetNumber(out Number num))
            {
                return JsonSerializer.Serialize<IBridge>(new Bridge(num), options);
            }
            else if (TryGetObject(out IDictionary<string,BridgeInstance> obj))
            {
                return JsonSerializer.Serialize<IBridge>(new Bridge(obj), options);
            }
            else if (TryGetString(out string text))
            {
                return JsonSerializer.Serialize<IBridge>(new Bridge(text), options);
            }
            return JsonSerializer.Serialize<IBridge>(new Bridge(), options);
        }

        public bool TryGetArray(out IEnumerable<BridgeInstance> value)
        {
            bool result = IsArray;
            value = result ? AsArray : [];
            return result;
        }

        public bool TryGetBoolean(out bool value)
        {
            value = IsBoolean && AsBoolean;
            return IsBoolean;
        }

        public bool TryGetNumber(out Number value)
        {
            bool result = IsNumber;
            value = result ? AsNumber : new();
            return result;
        }
        
        public bool TryGetObject(out IDictionary<string, BridgeInstance> value)
        {
            bool result = IsObject;
            value = result ? AsObject : new Dictionary<string, BridgeInstance>();
            return result;
        }

        public bool TryGetString(out string value)
        {
            bool result = IsString;
            value = result ? AsString : "";
            return result;
        }

        public static bool operator==(BridgeInstance left, BridgeInstance right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(BridgeInstance left, BridgeInstance right)
        {
            return !left.Equals(right);
        }

        [GeneratedRegex(@"^\d+\.\d+$", RegexOptions.Compiled)]
        private static partial Regex DigitsWithDecimal();
        [GeneratedRegex(@"^\d+$", RegexOptions.Compiled)]
        private static partial Regex Digits();
        [GeneratedRegex(@"^\-\d+\.\d+$", RegexOptions.Compiled)]
        private static partial Regex NegativeDigitsWithDecimal();
        [GeneratedRegex(@"^\-\d+$", RegexOptions.Compiled)]
        private static partial Regex NegativeDigits();
    }
}