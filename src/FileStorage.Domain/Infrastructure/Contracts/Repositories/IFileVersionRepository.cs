using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.Domain.Infrastructure.Contracts.Repositories
{
    public interface IFileVersionRepository
    {
        Task<FileVersion> GetFileVersionByMd5HashAsync(string hash);
        void AddFileVersion(FileVersion fileVersion);
        Task<int> GetLastVersionOfTheFile(Node file);
    }
}
