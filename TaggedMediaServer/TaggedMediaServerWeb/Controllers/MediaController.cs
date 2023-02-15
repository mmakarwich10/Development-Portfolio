using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaggedMediaServerWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMedia(
            [FromQuery(Name = "include-deprecated")] bool includeDeprecated = false, 
            [FromQuery(Name = "include-non-depr-dissociated")] bool includeNonDeprDissociated = false,
            [FromQuery(Name = "origin")] int originId = 0,
            [FromQuery(Name = "type")] int typeId = 0,
            [FromQuery] bool archived = false)
        {
            string?[] tagList = HttpContext.Request.Query["tag"].ToArray();

            foreach(string? tag in tagList)
            {
                if (tag == null)
                {
                    return BadRequest("One or more tags in the query are null.");
                }
            }

            if (originId < 0)
            {
                return BadRequest("Origin ID cannot be less than zero.");
            }

            if (typeId < 0)
            {
                return BadRequest("Type ID cannot be less than zero.");
            }

            List<MediumDto>
        }
    }
}
