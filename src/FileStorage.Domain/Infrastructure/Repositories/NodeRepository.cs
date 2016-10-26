using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using FileStorage.Domain.Infrastructure.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Domain.Infrastructure.Repositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly DataDbContext _dataDbContext;

        public NodeRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }

        public void AddNode(Node node)
        {
            _dataDbContext.Nodes.Add(node);
        }
        public async Task<Node> GetNodeById(int nodeId)
        {
            var node = await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Id == nodeId);
            return node;
        }

        public async Task<IEnumerable<Node>> GetAllNodesForUser(string userId)
        {
            var nodes = await _dataDbContext.Nodes.Where(r => r.OwnerId == userId).Include(s=>s.FileVersions).ToArrayAsync();
            return nodes;
        }
        public async Task<Node> GetNodeByName(string nodeName)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Name == nodeName);
        }
    }
}
