using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;

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
    }
}