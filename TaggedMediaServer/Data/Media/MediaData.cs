using Models.Dtos;

namespace Data.Media
{
    public class MediaData : IMediaData
    {
        public Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidMediaOrigin(int originId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidMediaType(int typeId)
        {
            throw new NotImplementedException();
        }
    }
}