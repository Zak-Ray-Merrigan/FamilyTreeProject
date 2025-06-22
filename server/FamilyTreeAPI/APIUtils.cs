using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FamilyTreeAPI
{
    public static class APIUtils
    {
        private const string STRING_DEFAULT = "unknown";
        public static FamilyTreeService Service
        {
            get;
            set;
        }

        public static PersonElement PersonDefault
        {
            get
            {
                return new PersonElement()
                {
                    Name = STRING_DEFAULT,
                    BirthDate = STRING_DEFAULT,
                    DeceasedDate = STRING_DEFAULT
                };
            }
        }

        public static FamilyElement FamilyDefault
        {
            get
            {
                return new FamilyElement()
                {
                    Member = PersonDefault,
                    InLaw = PersonDefault,
                    MarriageDate = STRING_DEFAULT
                };
            }
        }

        public static Family DeserializeFamilyElement(FamilyElement element)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"This family element: \"{element}\" is being deserialized.");
                Person member = DeserializePersonElement(element.Member);
                Person inLaw = element.InLaw == PersonDefault ? null : DeserializePersonElement(element.InLaw);
                FamilyTreeDate marriageDate = element.MarriageDate == FamilyDefault.MarriageDate ? new(null) : new(element.MarriageDate);
                return new(member, inLaw, marriageDate);
            }
            catch (FormatException ex)
            {
                throw new ClientBadRequestException(ex.Message, ex);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("Family element can't be null.", ex);
            }
        }

        public static Person DeserializePersonElement(PersonElement element)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"This person element: \"{element}\" is being deserialized.");
                string name = element.Name == PersonDefault.Name ? null : element.Name;
                FamilyTreeDate birthDate = element.BirthDate == PersonDefault.BirthDate ? new(null) : new(element.BirthDate);
                FamilyTreeDate deceasedDate = element.DeceasedDate == PersonDefault.DeceasedDate ? new(null) : new(element.DeceasedDate);
                return new(name, birthDate, deceasedDate);
            }
            catch (FormatException ex)
            {
                throw new ClientBadRequestException(ex.Message, ex);
            }
            catch (NullReferenceException ex)
            {
                throw new ClientBadRequestException("Person element can't be null.", ex);
            }
        }

        public static IActionResult SerializeAsClinetError(ClientException ex)
        {
            MessageResponse response = new()
            {
                Message = ex.Message,
                IsSuccess = false
            };
            FamilyTreeUtils.LogMessage(LoggingLevels.Error, response.Message);
            return new ObjectResult(response)
            {
                StatusCode = (int)ex.StatusCode
            };
        }
        
        public static IActionResult SerializeAsServerError(Exception ex)
        {
            MessageResponse response = new()
            {
                Message = $"{ex.GetType().Name}: {ex.Message}",
                IsSuccess = false
            };
            FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, response.Message);
            return new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        public static FamilyElement SerializeFamily(Family family)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"{family} is being serialized.");
                return new()
                {
                    Member = new PersonElement
                    {
                        Name = family.Member.Name is null ? STRING_DEFAULT : family.Member.Name,
                        BirthDate = family.Member.BirthDate.ToString() is null ? STRING_DEFAULT : family.Member.BirthDate.ToString(),
                        DeceasedDate = family.Member.DeceasedDate.ToString() is null ? STRING_DEFAULT : family.Member.DeceasedDate.ToString()
                    },
                    InLaw = family.InLaw is null ? PersonDefault : new PersonElement
                    {
                        Name = family.InLaw.Name is null ? STRING_DEFAULT : family.InLaw.Name,
                        BirthDate = family.InLaw.BirthDate.ToString() is null ? STRING_DEFAULT : family.InLaw.BirthDate.ToString(),
                        DeceasedDate = family.InLaw.DeceasedDate.ToString() is null ? STRING_DEFAULT : family.InLaw.DeceasedDate.ToString()
                    },
                    MarriageDate = family.MarriageDate.ToString() is null ? STRING_DEFAULT : family.MarriageDate.ToString()
                };
            }
            catch (NullReferenceException ex)
            {
                throw new ClientNotFoundException("The family member is unknown.", ex);
            }
        }
    }
}