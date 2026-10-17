using VirtualFamilyMuseumLibrary.Documents.Models;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Documents.Models
{
    public class PersonTest
    {
        private static readonly Guid DefaultId = Guid.Parse("6f1c2a7e-3b4d-4e5f-8a9b-0c1d2e3f4a5b");

        private static Dictionary<string,BridgeValue> CreateAttributes(string birthName, string? birthDate = null, string? deceasedDate = null, Guid? id = null, string birthFamilyName = "Kessler")
        {
            return new Dictionary<string,BridgeValue>()
            {
                {"id", (id ?? DefaultId).ToString()},
                {"birthFamilyName", birthFamilyName},
                {"birthName", birthName},
                {"birthDate", birthDate is null ? new BridgeValue() : birthDate},
                {"deceasedDate", deceasedDate is null ? new BridgeValue() : deceasedDate}
            };
        }

        private static Person CreatePerson(string birthName, string? birthDate = null, string? deceasedDate = null, Guid? id = null, string birthFamilyName = "Kessler")
        {
            return new Person(CreateAttributes(birthName, birthDate, deceasedDate, id, birthFamilyName));
        }

        // =====================================================================
        // Constructors
        // =====================================================================

        [Test]
        public void ConstructorShouldExposeRequiredAttributesThroughTypedProperties()
        {
            Person person = CreatePerson("Colby Bryan Kessler", "20 Feb 1888", "Apr 1942");

            Assert.That(person.Id, Is.EqualTo(DefaultId));
            Assert.That(person.BirthFamilyName, Is.EqualTo("Kessler"));
            Assert.That(person.BirthName, Is.EqualTo("Colby Bryan Kessler"));
            Assert.That(person.BirthDate, Is.EqualTo(new FamilyDate("1888", Month.Feb, 20)));
            Assert.That(person.DeceasedDate, Is.EqualTo(new FamilyDate("1942", Month.Apr)));
        }

        [Test]
        public void ConstructorShouldCopyDictionarySoLaterChangesToSourceDoNotAffectPerson()
        {
            Dictionary<string,BridgeValue> attributes = CreateAttributes("Kit Dale Kessler");
            Person person = new(attributes);

            attributes["birthName"] = "Changed Name";

            Assert.That(person.BirthName, Is.EqualTo("Kit Dale Kessler"));
        }

        [Test]
        public void CopyConstructorShouldCreateIndependentCopy()
        {
            Person original = CreatePerson("Todd Solo", "1975");
            Person copy = new(original);

            copy.BirthName = "Changed Name";

            Assert.That(original.BirthName, Is.EqualTo("Todd Solo"));
            Assert.That(copy.BirthName, Is.EqualTo("Changed Name"));
        }

        [Test]
        public void CopyConstructorShouldProduceEqualPerson()
        {
            Person original = CreatePerson("Todd Solo", "1975", "2020");

            Assert.That(new Person(original), Is.EqualTo(original));
        }

        // =====================================================================
        // Id + BirthFamilyName + BirthName
        // =====================================================================

        [Test]
        public void IdSetterShouldStoreGuidAsStringUnderIdAttribute()
        {
            Person person = CreatePerson("Todd Solo");
            Guid id = Guid.NewGuid();

            person.Id = id;

            Assert.That(person.Id, Is.EqualTo(id));
            Assert.That((string)person["id"], Is.EqualTo(id.ToString()));
        }

        [Test]
        public void BirthFamilyNameSetterShouldStoreValueUnderBirthFamilyNameAttribute()
        {
            Person person = CreatePerson("Todd Solo");

            person.BirthFamilyName = "Vasterling";

            Assert.That(person.BirthFamilyName, Is.EqualTo("Vasterling"));
            Assert.That((string)person["birthFamilyName"], Is.EqualTo("Vasterling"));
        }

        [Test]
        public void BirthNameSetterShouldStoreValueUnderBirthNameAttribute()
        {
            Person person = CreatePerson("Todd Solo");

            person.BirthName = "Todd Zachary Vasterling";

            Assert.That(person.BirthName, Is.EqualTo("Todd Zachary Vasterling"));
            Assert.That((string)person["birthName"], Is.EqualTo("Todd Zachary Vasterling"));
        }

        // =====================================================================
        // BirthDate + DeceasedDate
        // =====================================================================

        [Test]
        public void BirthDateShouldReturnNullWhenStoredValueIsNull()
        {
            Person person = CreatePerson("Todd Solo");

            Assert.That(person.BirthDate, Is.Null);
        }

        [Test]
        public void BirthDateShouldReturnNullWhenStoredStringIsNotAFamilyDate()
        {
            Person person = CreatePerson("Todd Solo", "not a date");

            Assert.That(person.BirthDate, Is.Null);
        }

        [Test]
        public void BirthDateShouldParseYearRange()
        {
            Person person = CreatePerson("Todd Solo", "1900-1910");

            Assert.That(person.BirthDate, Is.Not.Null);
            Assert.That(person.BirthDate!.Value.Year, Is.EqualTo("1900-1910"));
        }

        [Test]
        public void BirthDateSetterShouldStoreFamilyDateString()
        {
            Person person = CreatePerson("Todd Solo");

            person.BirthDate = new FamilyDate("1888", Month.Feb, 20);

            Assert.That(person.BirthDate, Is.EqualTo(new FamilyDate("1888", Month.Feb, 20)));
            Assert.That((string)person["birthDate"], Is.EqualTo("20 Feb 1888"));
        }

        [Test]
        public void BirthDateSetterShouldStoreJsonNullWhenSetToNull()
        {
            Person person = CreatePerson("Todd Solo", "1975");

            person.BirthDate = null;

            Assert.That(person.BirthDate, Is.Null);
            Assert.That(person["birthDate"].IsNull, Is.True);
        }

        [Test]
        public void DeceasedDateShouldReturnNullWhenStoredValueIsNull()
        {
            Person person = CreatePerson("Todd Solo", "1975");

            Assert.That(person.DeceasedDate, Is.Null);
        }

        [Test]
        public void DeceasedDateSetterShouldStoreFamilyDateString()
        {
            Person person = CreatePerson("Todd Solo", "1975");

            person.DeceasedDate = new FamilyDate("2020", Month.Dec);

            Assert.That(person.DeceasedDate, Is.EqualTo(new FamilyDate("2020", Month.Dec)));
            Assert.That((string)person["deceasedDate"], Is.EqualTo("Dec 2020"));
        }

        [Test]
        public void DeceasedDateSetterShouldStoreJsonNullWhenSetToNull()
        {
            Person person = CreatePerson("Todd Solo", "1975", "2020");

            person.DeceasedDate = null;

            Assert.That(person.DeceasedDate, Is.Null);
            Assert.That(person["deceasedDate"].IsNull, Is.True);
        }

        // =====================================================================
        // Indexer + Value
        // =====================================================================

        [Test]
        public void IndexerShouldGetAndSetAdditionalAttributes()
        {
            Person person = CreatePerson("Todd Solo");

            person["nickname"] = "TJ";

            Assert.That((string)person["nickname"], Is.EqualTo("TJ"));
        }

        [Test]
        public void IndexerSetOnRequiredAttributeShouldBeVisibleThroughTypedProperty()
        {
            Person person = CreatePerson("Todd Solo");

            person["birthName"] = "Todd Zachary Vasterling";

            Assert.That(person.BirthName, Is.EqualTo("Todd Zachary Vasterling"));
        }

        [Test]
        public void ValueShouldContainEveryAttributeIncludingAdditionalOnes()
        {
            Dictionary<string,BridgeValue> attributes = CreateAttributes("Todd Solo", "1975");
            attributes["nickname"] = "TJ";
            Person person = new(attributes);

            Assert.That(person.Value, Is.EqualTo(new BridgeValue(attributes)));
        }

        [Test]
        public void ValueShouldBeASnapshotThatDoesNotChangeWithLaterEdits()
        {
            Person person = CreatePerson("Todd Solo", "1975");
            BridgeValue snapshot = person.Value;

            person.BirthName = "Changed Name";

            Assert.That((string)snapshot.AsObject["birthName"], Is.EqualTo("Todd Solo"));
        }

        // =====================================================================
        // CompareTo
        // =====================================================================

        [Test]
        public void CompareToShouldReturnPositiveWhenOtherIsNull()
        {
            Person person = CreatePerson("Todd Solo");

            Assert.That(person.CompareTo(null), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldReturnZeroForIdenticalPeople()
        {
            Person a = CreatePerson("Same Name", "1980", "2000");
            Person b = CreatePerson("Same Name", "1980", "2000");

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldOrderByBirthDateFirst()
        {
            Person a = CreatePerson("Zed Name", "1980");
            Person b = CreatePerson("Abe Name", "1990");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderPersonWithoutBirthDateBeforePersonWithBirthDate()
        {
            Person a = CreatePerson("Zed Name");
            Person b = CreatePerson("Abe Name", "1980");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderByBirthNameWhenBirthDatesMatch()
        {
            Person a = CreatePerson("Adams", "1980");
            Person b = CreatePerson("Baker", "1980");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderByDeceasedDateWhenBirthDatesAndNamesMatch()
        {
            Person a = CreatePerson("Same", "1980", "2000");
            Person b = CreatePerson("Same", "1980", "2010");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderLivingPersonBeforeDeceasedPersonWhenOtherFieldsMatch()
        {
            Person a = CreatePerson("Same", "1980");
            Person b = CreatePerson("Same", "1980", "2010");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldIgnoreIdAndBirthFamilyName()
        {
            Person a = CreatePerson("Same", "1980", id: Guid.NewGuid(), birthFamilyName: "Kessler");
            Person b = CreatePerson("Same", "1980", id: Guid.NewGuid(), birthFamilyName: "Vasterling");

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldIgnoreAdditionalAttributes()
        {
            Person a = CreatePerson("Same", "1980");
            Person b = CreatePerson("Same", "1980");
            b["nickname"] = "TJ";

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        // =====================================================================
        // CompareTo — year ranges and partial dates
        // =====================================================================

        [Test]
        public void CompareToShouldTreatYearInsideBirthRangeAsEqual()
        {
            Person a = CreatePerson("Jane Doe", "1905");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
            Assert.That(b.CompareTo(a), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldTreatYearInsideDeceasedRangeAsEqual()
        {
            Person a = CreatePerson("Jane Doe", "1905", "1965");
            Person b = CreatePerson("Jane Doe", "1905", "1960-1970");

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldOrderYearBeforeBirthRangeThatStartsAfterIt()
        {
            Person a = CreatePerson("Jane Doe", "1899");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderYearAfterBirthRangeThatEndsBeforeIt()
        {
            Person a = CreatePerson("Jane Doe", "1911");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.CompareTo(b), Is.GreaterThan(0));
            Assert.That(b.CompareTo(a), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldNotTreatOverlappingBirthRangesAsEqual()
        {
            Person a = CreatePerson("Jane Doe", "1900-1910");
            Person b = CreatePerson("Jane Doe", "1905-1915");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldTreatIdenticalBirthRangesAsEqual()
        {
            Person a = CreatePerson("Jane Doe", "1900-1910");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldOrderUnknownMonthBeforeKnownMonthWhenYearFallsInRange()
        {
            Person a = CreatePerson("Jane Doe", "Mar 1905");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.CompareTo(b), Is.GreaterThan(0));
            Assert.That(b.CompareTo(a), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderUnknownMonthBeforeKnownMonthWhenYearsMatch()
        {
            Person a = CreatePerson("Jane Doe", "1905");
            Person b = CreatePerson("Jane Doe", "Mar 1905");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderUnknownDayBeforeKnownDayWhenYearsAndMonthsMatch()
        {
            Person a = CreatePerson("Jane Doe", "Mar 1905");
            Person b = CreatePerson("Jane Doe", "1 Mar 1905");

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        // =====================================================================
        // Equals (typed and object overload) + GetHashCode
        // =====================================================================

        [Test]
        public void EqualsShouldReturnTrueForPeopleWithIdenticalNameAndDates()
        {
            Person a = CreatePerson("Same Name", "20 Feb 1888", "Apr 1942");
            Person b = CreatePerson("Same Name", "20 Feb 1888", "Apr 1942");

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsShouldReturnTrueForDifferentDocumentsDescribingTheSamePerson()
        {
            Person a = CreatePerson("Same Name", "1980", id: Guid.NewGuid(), birthFamilyName: "Kessler");
            Person b = CreatePerson("Same Name", "1980", id: Guid.NewGuid(), birthFamilyName: "Vasterling");

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsShouldReturnTrueWhenYearFallsInsideOtherPersonsBirthRange()
        {
            Person a = CreatePerson("Jane Doe", "1905");
            Person b = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsShouldReturnFalseWhenBirthNameDiffers()
        {
            Person a = CreatePerson("Adams", "1980");
            Person b = CreatePerson("Baker", "1980");

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsShouldReturnFalseWhenDeceasedDateDiffers()
        {
            Person a = CreatePerson("Same", "1980", "2000");
            Person b = CreatePerson("Same", "1980", "2010");

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsShouldReturnFalseForNull()
        {
            Person a = CreatePerson("Same Name");

            Assert.That(a.Equals((Person?)null), Is.False);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnTrueForEqualPerson()
        {
            Person a = CreatePerson("Same Name", "1980");
            object b = CreatePerson("Same Name", "1980");

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnFalseForNonPersonOrNull()
        {
            Person a = CreatePerson("Same Name");

            Assert.That(a.Equals("Same Name"), Is.False);
            Assert.That(a.Equals((object?)null), Is.False);
        }

        [Test]
        public void GetHashCodeShouldBeConsistentForEqualPeopleBackedByDistinctDictionaries()
        {
            Person a = CreatePerson("Same Name", "20 Feb 1888", "Apr 1942");
            Person b = CreatePerson("Same Name", "20 Feb 1888", "Apr 1942");

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCodeShouldIgnoreIdAndBirthFamilyName()
        {
            Person a = CreatePerson("Same Name", "1980", id: Guid.NewGuid(), birthFamilyName: "Kessler");
            Person b = CreatePerson("Same Name", "1980", id: Guid.NewGuid(), birthFamilyName: "Vasterling");

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCodeShouldDifferForDifferentPeople()
        {
            Person a = CreatePerson("Adams", "1980");
            Person b = CreatePerson("Baker", "1990");

            // Hash collisions are possible in theory but extremely unlikely for these values
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToStringShouldReturnDashWhenNoBirthDate()
        {
            Person person = CreatePerson("Kit Dale Kessler");

            Assert.That(person.ToString(), Is.EqualTo("Kit Dale Kessler (–)"));
        }

        [Test]
        public void ToStringShouldReturnPresentWhenBirthDateButNoDeceasedDate()
        {
            Person person = CreatePerson("Todd Solo", "1975");

            Assert.That(person.ToString(), Is.EqualTo("Todd Solo (1975 – Present)"));
        }

        [Test]
        public void ToStringShouldReturnBirthAndDeceasedDateWhenBothSet()
        {
            Person person = CreatePerson("Colby Bryan Kessler", "20 Feb 1888", "Apr 1942");

            Assert.That(person.ToString(), Is.EqualTo("Colby Bryan Kessler (20 Feb 1888 – Apr 1942)"));
        }

        [Test]
        public void ToStringShouldShowYearRangeAsRecorded()
        {
            Person person = CreatePerson("Jane Doe", "1900-1910");

            Assert.That(person.ToString(), Is.EqualTo("Jane Doe (1900-1910 – Present)"));
        }

        // =====================================================================
        // Comparison operators — both non-null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForIdenticalPeople()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseForDifferentPeople()
        {
            Person? a = CreatePerson("Adams", "1980");
            Person? b = CreatePerson("Baker", "1990");

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForIdenticalPeople()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueForDifferentPeople()
        {
            Person? a = CreatePerson("Adams", "1980");
            Person? b = CreatePerson("Baker", "1990");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenFirstIsEarlier()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1990");

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenFirstIsLater()
        {
            Person? a = CreatePerson("Same Name", "1990");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenPeopleAreEqual()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenFirstIsEarlier()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1990");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenPeopleAreEqual()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenFirstIsLater()
        {
            Person? a = CreatePerson("Same Name", "1990");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenFirstIsLater()
        {
            Person? a = CreatePerson("Same Name", "1990");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenFirstIsEarlier()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1990");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenPeopleAreEqual()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenFirstIsLater()
        {
            Person? a = CreatePerson("Same Name", "1990");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenPeopleAreEqual()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1980");

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenFirstIsEarlier()
        {
            Person? a = CreatePerson("Same Name", "1980");
            Person? b = CreatePerson("Same Name", "1990");

            Assert.That(a >= b, Is.False);
        }

        // =====================================================================
        // Comparison operators — null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyLeftIsNull()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyRightIsNull()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyLeftIsNull()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyRightIsNull()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueForBothNull()
        {
            Person? a = null;
            Person? b = null;

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            Person? a = null;
            Person? b = CreatePerson("Same Name");

            Assert.That(a >= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            Person? a = CreatePerson("Same Name");
            Person? b = null;

            Assert.That(a >= b, Is.True);
        }
    }
}
