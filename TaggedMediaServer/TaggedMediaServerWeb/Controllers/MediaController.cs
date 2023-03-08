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

        /// <summary>
        ///     Searches for media based on various specifications.
        /// </summary>
        /// <param name="includeDeprecated">Set this flag if you're including deprecated tags in the search. Default: false.</param>
        /// <param name="includeNonDeprDissociated">
        ///     Set this flag if the media you're searching for might not have a (non-deprecated) tag associated to it anymore. Default: false.
        /// </param>
        /// <param name="originId">If you want to filter by medium origin, specify its ID here. Use -1 for any origin. Default: -1.</param>
        /// <param name="typeId">If you want to filter by medium type, specify its ID here. Use -1 for any type. Default: -1.</param>
        /// <param name="archived">Set this flag if you want to only return archived media in the results. Default: false.</param>
        /// <returns>Array of filtered medium objects.</returns>
        /// <exception cref="HttpResponseException">If there is a database issue, a 500 error will be returned.</exception>
        /// <remarks>
        ///     A 422 (Unprocessable Entity) error will be return if:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Any given tag is null.</description>
        ///         </item>
        ///         <item>
        ///             <description>Given Origin and/or Type ID is less than -1.</description>
        ///         </item>
        ///         <item>
        ///             <description>Any given tag does not exist in the system.</description>
        ///         </item>
        ///         <item>
        ///             <description>Given Origin and/or Type ID does not exist in the system.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetMedia(
            [FromQuery(Name = "include-deprecated")] bool includeDeprecated = false, 
            [FromQuery(Name = "include-non-depr-dissociated")] bool includeNonDeprDissociated = false,
            [FromQuery(Name = "origin")] int originId = -1,
            [FromQuery(Name = "type")] int typeId = -1,
            [FromQuery] bool archived = false)
        { //TODO: Find out how (if possible) to add the tag param here.
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
            catch (InvalidTagException)
            {
                return UnprocessableEntity("At least one given tag does not exist.");
            }
            catch (InvalidMediaTypeException)
            {
                return UnprocessableEntity("The given media type does not exist.");
            }
            catch (InvalidMediaOriginException)
            {
                return UnprocessableEntity("The given media origin does not exist.");
            }
        }
    }
}
