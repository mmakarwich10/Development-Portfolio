using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace TaggedMediaServerWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private IMediaLogic _mediaLogic;

        public MediaController(IMediaLogic mediaLogic)
        {
            _mediaLogic = mediaLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetMedia(
            [FromQuery(Name = "include-deprecated")] bool includeDeprecated = false, 
            [FromQuery(Name = "include-non-depr-dissociated")] bool includeNonDeprDissociated = false,
            [FromQuery(Name = "origin")] int originId = -1,
            [FromQuery(Name = "type")] int typeId = -1,
            [FromQuery] bool archived = false)
        {
            string?[] tagList = HttpContext.Request.Query["tag"].ToArray();
            List<string> cleanTagList = new List<string>();

            foreach(string? tag in tagList)
            {
                if (tag == null)
                {
                    return BadRequest("One or more tags in the query are null.");
                }
                else
                {
                    cleanTagList.Add(tag);
                }
            }

            if (originId < -1)
            {
                return BadRequest("Origin ID cannot be less than -1.");
            }

            if (typeId < -1)
            {
                return BadRequest("Type ID cannot be less than -1.");
            }

            try
            {
                List<MediumDto> returnedMedia = await _mediaLogic.GetMediaWithFilters(cleanTagList, includeDeprecated, includeNonDeprDissociated, originId, typeId, archived);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
