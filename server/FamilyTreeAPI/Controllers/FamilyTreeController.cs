using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyTreeController : ControllerBase
    {
        [HttpPost("generation-number-of")]
        public IActionResult GenerationNumberOf([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Figuring out the generation level of {element.Member.Name}.");
                return Ok(APIUtils.Service.GenerationNumberOf(family));
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }
        
        [HttpGet("get-families/{orderOption}/by-member-name")]
        public IActionResult GetFamilies([FromRoute]string orderOption, [FromQuery]string memberName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Getting the {APIUtils.Service.Name} family tree in {orderOption} order filtered by: memberName = {(memberName is null || memberName == "" ? "unknown" : memberName)}.");
                SortingOptions option = orderOption switch 
                {
                    "parent first then children" => SortingOptions.ParentFirstThenChildren,
                    "ascending by name" => SortingOptions.AscendingByName,
                    _ => SortingOptions.Empty
                };
                IEnumerable<Family> families = APIUtils.Service.FilterTree(option, memberName);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {APIUtils.Service.Name} family tree is now being serialized.");
                IEnumerable<FamilyElement> response = families.Select(APIUtils.SerializeFamily);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }
        
        [HttpGet("number-of-generations")]
        public IActionResult NumberOfGenerations()
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Counting the number of generations in the {APIUtils.Service.Name} family tree.");
                int response = APIUtils.Service.NumberOfGenerations;
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,$"The {APIUtils.Service.Name} family tree has {response} generations.");
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("number-of-families")]
        public IActionResult NumberOfFamilies()
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Counting the number of families in the {APIUtils.Service.Name} tree.");
                long response = APIUtils.Service.NumberOfFamilies;
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,$"{response} families are part of the {APIUtils.Service.Name} family tree.");
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("initialize-service/{familyName}")]
        public IActionResult InitializeService([FromRoute] string familyName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The service is being initialized based on the {familyName} Family Tree.");
                APIUtils.Service = new(familyName);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The service has been initialized based on the {familyName} Family Tree.");
                MessageResponse response = new()
                {
                    Message = $"This is the {familyName} Family Tree.",
                    IsSuccess = true
                };
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientBadRequestException("Family name is unknown.", ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("report-children")]
        public IActionResult ReportChildren([FromBody] ReportChildrenRequest request)
        {
            try
            {
                Family parent = APIUtils.DeserializeFamilyElement(request.Parent);
                Family child = APIUtils.DeserializeFamilyElement(request.Child);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The parent-child relationship between {parent.Member.Name} and {child.Member.Name} is being reported.");
                APIUtils.Service.ReportChildren(parent, child);
                MessageResponse response = new()
                {
                    Message = $"The parent-child relationship between {parent.Member.Name} and {child.Member.Name} has been reported successfully.",
                    IsSuccess = true
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, response.Message);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPatch("report-deceased")]
        public IActionResult ReportDeceased([FromBody] ReportDeceasedRequest request)
        {
            try
            {
                Person p = request.Element == APIUtils.PersonDefault ? null : APIUtils.DeserializePersonElement(request.Element);
                FamilyTreeDate deceasedDate = new(request.DeceasedDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"{request.Element.Name} being deceased on {request.DeceasedDate} is being reported.");
                APIUtils.Service.ReportDeceased(p, deceasedDate);
                MessageResponse response = new()
                {
                    Message = $"{request.Element.Name} being deceased on {request.DeceasedDate} has been reported successfully.",
                    IsSuccess = true
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, response.Message);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPatch("report-married")]
        public IActionResult ReportMarried([FromBody] FamilyElement request)
        {
            try
            {
                Person member = APIUtils.DeserializePersonElement(request.Member);
                Person inLaw = request.InLaw == APIUtils.PersonDefault ? null : APIUtils.DeserializePersonElement(request.InLaw);
                FamilyTreeDate marriageDate = new(request.MarriageDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"A marriage between {request.Member.Name} and {request.InLaw.Name} on {request.MarriageDate} is being reported.");
                APIUtils.Service.ReportMarried(member, inLaw, marriageDate);
                MessageResponse response = new()
                {
                    Message = $"The marriage between {request.Member.Name} and {request.InLaw.Name} has been applied to the tree.",
                    IsSuccess = true
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, response.Message);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClinetError(new ClientBadRequestException("Family Element doesn't exist.", ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("retrieve-children")]
        public IActionResult RetrieveChildren([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Retrieving the children of {element.Member.Name}");
                return Ok(APIUtils.Service.RetrieveChildrenOf(family).Select(APIUtils.SerializeFamily));
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("retrieve-parent")]
        public IActionResult RetrieveParent([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"In the {APIUtils.Service.Name} family tree, we are retrieving the parent of {family.Member.Name}");
                Family parent = APIUtils.Service.RetrieveParentOf(family);
                FamilyElement response = APIUtils.SerializeFamily(parent);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPut("revert-tree")]
        public IActionResult RevertTree(IFormFile file)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {APIUtils.Service.Name} family tree is being reverted based on the following file named: {file.FileName}");
                string filePath = Path.Combine(@"C:\FamilyTreeProject\resources\PDFInputs", file.FileName);
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                APIUtils.Service.RevertTree(new(filePath));
                MessageResponse response = new()
                {
                    Message = $"{APIUtils.Service.Name} tree has been reverted successfully.",
                    IsSuccess = true
                };
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("view-subtree/{orderOption}/by-member-name")]
        public IActionResult ViewSubtree([FromRoute] string orderOption, [FromQuery] string memberName, [FromBody] FamilyElement element)
        {
            try
            {
                SortingOptions option = orderOption switch
                {
                    "parent first then children" => SortingOptions.ParentFirstThenChildren,
                    "ascending by name" => SortingOptions.AscendingByName,
                    _ => SortingOptions.Empty
                };
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Retrieving the subtree rooted at {element.Member.Name} and {element.InLaw.Name}.");
                return Ok(APIUtils.Service.GetSubtree(option, memberName, family).Select(APIUtils.SerializeFamily));
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClinetError(ex);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }
    }
}