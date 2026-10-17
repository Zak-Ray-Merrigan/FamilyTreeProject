using VirtualFamilyMuseumLibrary.Documents.Models;

namespace VirtualFamilyMuseumLibrary.Documents
{
    public static class DocumentExtensions
    {
        public static IReadOnlyDictionary<PersonRequiredAttributes,string> PersonRequiredAttributeNames
        {
            get
            {
                return new Dictionary<PersonRequiredAttributes,string>()
                {
                    {PersonRequiredAttributes.ID, "id"},
                    {PersonRequiredAttributes.BIRTH_FAMILY_NAME, "birthFamilyName"},
                    {PersonRequiredAttributes.BIRTH_NAME, "birthName"},
                    {PersonRequiredAttributes.BIRTH_DATE, "birthDate"},
                    {PersonRequiredAttributes.DECEASED_DATE, "deceasedDate"}
                };
            }
        }
    }
}