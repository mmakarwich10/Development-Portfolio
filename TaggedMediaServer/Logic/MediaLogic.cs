using Data;
using Models;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        private IMediaData _mediaData;

        public async Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            bool validTags = false;
            bool validOrigin = false;
            bool validType = false;

            validTags = _mediaData;

            if (validTags)
            {
                if (originId != -1)
                {
                    validOrigin = await _mediaData.IsValidMediaOrigin(originId);
                }
                else
                {
                    validOrigin = true;
                }

                if (validOrigin)
                {
                    if (typeId != -1)
                    {
                        validType = await _mediaData.IsValidMediaType(typeId);
                    }
                    else 
                    { 
                        validType = true; 
                    }
                }
            }
        }
    }
}