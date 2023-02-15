using Data.Media;
using Data.Tags;
using Models;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        private IMediaData _mediaData;
        private ITagsData _tagsData;

        public async Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            bool validTags = true;
            bool validOrigin = false;
            bool validType = false;

            foreach (var tagName in tagList)
            {
                if (!(await _tagsData.IsValidTag(tagName)))
                {
                    validTags = false;
                }
            }

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