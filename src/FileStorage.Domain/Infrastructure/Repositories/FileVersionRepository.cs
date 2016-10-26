using System.Linq;
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

        public async Task<int> GetLastVersionOfTheFile(Node file)
        {
            int lastVersionOfFile =  await _dataDbContext.FileVersions.Where(r => r.Node == file).MaxAsync(r => r.VersionOfFile);
            return lastVersionOfFile;
        }
        public void AddFileVersion(FileVersion fileVersion)
        {
            _dataDbContext.FileVersions.Add(fileVersion);
        }
    }
}
