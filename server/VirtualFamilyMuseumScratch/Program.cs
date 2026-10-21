using VirtualFamilyMuseumLibrary.Documents.Models;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

IDictionary<string,BridgeValue> obj = new Dictionary<string,BridgeValue>();
Guid id = Guid.NewGuid();
obj["id"] = id.ToString();
obj["birthName"] = "Colby Bryan Kessler";
obj["birthFamilyName"] = "Kessler";
obj["birthDate"] = new FamilyDate("1888", Month.Feb, 20).Value;
obj["deceasedDate"] = new FamilyDate("1942", Month.Apr).Value;
Person p = new(obj);
Console.WriteLine(p is IBridge);