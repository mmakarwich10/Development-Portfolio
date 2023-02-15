using Models;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        public Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            bool validOrigin = false;
            bool validType = false;
            bool validTags = false;
        }
    }
}