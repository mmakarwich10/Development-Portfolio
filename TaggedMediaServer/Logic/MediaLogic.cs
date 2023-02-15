using Models;

namespace Logic
{
    public class MediaLogic : IMediaLogic
    {
        public Task<List<MediumDto>> GetMediaWithFilters(bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            throw new NotImplementedException();
        }
    }
}