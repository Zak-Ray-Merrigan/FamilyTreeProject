using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        [HttpPost("representation-to-family-element")]
        public IActionResult RepresentationToFamilyElement([FromBody] RepresentationElement representationElement)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representationElement.Representation}");
                Family family = new(representationElement.Representation);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                return Ok(APIUtils.SerializeFamily(family));
            }
            catch (FormatException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientBadRequestException(ex.Message, ex));
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("family-element-to-representation")]
        public IActionResult FamilyElementToRepresenation([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                RepresentationElement representation = new()
                {
                    Representation = family.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("person-element-to-representation")]
        public IActionResult PersonElementToRepresentation([FromBody] PersonElement element)
        {
            try
            {
                Person person = APIUtils.DeserializePersonElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Person: {person}");
                RepresentationElement representation = new()
                {
                    Representation = person.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }
    }
}