using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.Domain.Infrastructure.Contracts.Repositories
{
    public interface IFileVersionRepository
    {
        Task<FileVersion> GetFileVersionByMd5HashAsync(string hash);
        void AddFileVersion(FileVersion fileVersion);
        Task<int> GetNumberOfLastVersionFile(Node file);
        Task<FileVersion> GetLatestFileVersion(Node file);

        Task<FileVersion> GetFileVersionOfVersionNumber(Node file, int versionNumber);
    }
}
