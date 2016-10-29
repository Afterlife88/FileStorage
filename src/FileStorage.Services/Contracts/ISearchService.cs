using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Services.DTO;

namespace FileStorage.Services.Contracts
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResultDto>> SearchFilesAsync(string user, string query, bool isRemoved = false);
    }
}
