using Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tags
{
    public interface ITagsData
    {
        Task<List<TagDto>> GetCurrentTagsByMediumIdAsync(int mediaId);
        Task<bool> TagExistsAsync(string tagName);
    }
}
