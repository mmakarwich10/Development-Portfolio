using Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Media
{
    public interface IMediaData
    {
        Task<List<MediumDto>> GetMediaWithFilters(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived);
        Task<bool> IsValidMediaOrigin(int originId);
        Task<bool> IsValidMediaType(int typeId);
    }
}
