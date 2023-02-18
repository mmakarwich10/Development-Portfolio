using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Models.Exceptions;
using System.Net;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

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
                    return UnprocessableEntity("One or more tags in the query are null.");
                }
                else
                {
                    cleanTagList.Add(tag);
                }
            }

            if (originId < -1)
            {
                return UnprocessableEntity("Origin ID cannot be less than -1.");
            }

            if (typeId < -1)
            {
                return UnprocessableEntity("Type ID cannot be less than -1.");
            }

            try
            {
                List<MediumDto> returnedMedia = await _mediaLogic.GetMediaWithFiltersAsync(cleanTagList, includeDeprecated, includeNonDeprDissociated, originId, typeId, archived);
                return Ok(returnedMedia);
            }
            catch (DatabaseException)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        "There was an issue during database operations. Please try the request again. If you continue to get this message, notify your administrator."
                    )
                };
                throw new HttpResponseException(response);
            }
        }
    }
}
