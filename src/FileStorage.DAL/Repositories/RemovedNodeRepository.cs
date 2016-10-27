using System.Threading.Tasks;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.DAL.Repositories
{
    public class RemovedNodeRepository : IRemovedNodeRepository
    {
        private readonly DataDbContext _dataDbContext;

        public RemovedNodeRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }
        public void AddRemovedNode(RemovedNode node)
        {
            _dataDbContext.RemovedNodes.Add(node);
        }

        public async Task DeleteRemovedNodeRecord(Node node)
        {
            var getNode = await _dataDbContext.RemovedNodes.FirstOrDefaultAsync(r => r.Node == node);
            _dataDbContext.RemovedNodes.Remove(getNode);
        }
    }
}