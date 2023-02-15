using Models;

namespace Data
{
    public class MediaData : IMediaData
    {
        public Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            throw new NotImplementedException();
        }
    }
}