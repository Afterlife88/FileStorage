using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.DAL.Contracts.Repositories
{
    public interface IFileVersionRepository
    {
        Task<FileVersion> GetFileVersionByMd5HashForUserAsync(string hash, string userId);
        void AddFileVersion(FileVersion fileVersion);
        Task<int> GetNumberOfLastVersionFile(Node file);
        Task<FileVersion> GetLatestFileVersion(Node file);

        Task<FileVersion> GetFileVersionOfVersionNumber(Node file, int versionNumber);
    }
}
