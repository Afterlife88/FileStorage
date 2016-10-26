using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using FileStorage.Domain.Infrastructure.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Domain.Infrastructure.Repositories
{
    public class FileVersionRepository : IFileVersionRepository
    {
        private readonly DataDbContext _dataDbContext;

        public FileVersionRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }
        public async Task<FileVersion> GetFileVersionByMd5HashAsync(string hash)
        {
            return await _dataDbContext.FileVersions.FirstOrDefaultAsync(r => r.MD5Hash == hash);

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
