using Data.Media;
using Data.Tags;
using Models.Dtos;
using Models.Exceptions;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        private IMediaData _mediaData;
        private ITagsData _tagsData;

        public MediaLogic(IMediaData mediaData, ITagsData tagsData)
        {
            _mediaData = mediaData;
            _tagsData = tagsData;
        }

        public async Task<List<MediumDto>> GetMediaWithFiltersAsync(
            List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            bool validTags = true;
            bool validOrigin = false;
            bool validType = false;

            foreach (var tagName in tagList)
            {
                if (!(await _tagsData.TagExistsAsync(tagName)))
                {
                    validTags = false;
                }
            }

            if (validTags)
            {
                if (originId != -1)
                {
                    validOrigin = await _mediaData.MediumOriginExistsAsync(originId);
                }
                else
                {
                    validOrigin = true;
                }

                if (validOrigin)
                {
                    if (typeId != -1)
                    {
                        validType = await _mediaData.MediumTypeExistsAsync(typeId);
                    }
                    else 
                    { 
                        validType = true; 
                    }

                    if (validType)
                    {
                        return await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, includeDeprecated, includeNonDeprDissociated, originId, typeId, archived);
                    }
                    else
                    {
                        throw new InvalidMediaTypeException();
                    }
                }
                else
                {
                    throw new InvalidMediaOriginException();
                }
            }
            else
            {
                throw new InvalidTagException();
            }
        }
    }
}