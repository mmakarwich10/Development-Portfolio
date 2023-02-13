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
            [FromQuery(Name = "include-deprecated")] bool includeDeprecated, 
            [FromQuery(Name = "include-non-depr-dissociated")] bool includeNonDeprDissociated,
            [FromQuery(Name = "origin")] int originId,
            [FromQuery(Name = "type")] int typeId,
            [FromQuery] bool archived)
        {
            List<MediumDto>
        }
    }
}
