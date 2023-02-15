using Data;
using Models;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        private IMediaData _mediaData;

        public Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            bool validTags = false;
            bool validOrigin = false;
            bool validType = false;

            validTags = _mediaData;

            if (originId != -1)
            {
                validOrigin = _mediaData
            }
        }
    }
}