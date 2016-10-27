using System.Threading.Tasks;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FileStorage.DAL.Repositories
{
    public class FileVersionRepository : IFileVersionRepository
    {
        private readonly DataDbContext _dataDbContext;

        public FileVersionRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }
        public async Task<FileVersion> GetFileVersionByMd5HashForUserAsync(string hash, string userId)
        {
            var userNodes = await _dataDbContext.Nodes.Where(r => r.OwnerId == userId && !r.IsDeleted).Include(r => r.FileVersions).ToArrayAsync();
            var node = userNodes.FirstOrDefault(r => r.FileVersions.Any(s => s.MD5Hash == hash));
            return node?.FileVersions.FirstOrDefault(r => r.MD5Hash == hash);

        }

        public async Task<int> GetNumberOfLastVersionFile(Node file)
        {
            int lastVersionOfFile = await _dataDbContext.FileVersions.Where(r => r.Node == file).MaxAsync(r => r.VersionOfFile);
            return lastVersionOfFile;
        }

        public async Task<FileVersion> GetFileVersionOfVersionNumber(Node file, int versionNumber)
        {
            return
              await
                  _dataDbContext.FileVersions.FirstOrDefaultAsync(
                      r => r.Node == file && r.VersionOfFile == versionNumber);
        }
        public async Task<FileVersion> GetLatestFileVersion(Node file)
        {
            int numberOfLastFileVersion = await GetNumberOfLastVersionFile(file);

            return
                await
                    _dataDbContext.FileVersions.FirstOrDefaultAsync(
                        r => r.Node == file && r.VersionOfFile == numberOfLastFileVersion);
        }
        public void AddFileVersion(FileVersion fileVersion)
        {
            _dataDbContext.FileVersions.Add(fileVersion);
        }
    }
}
