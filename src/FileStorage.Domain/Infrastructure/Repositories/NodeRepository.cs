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

        public async Task<Node> GetNodeByName(string nodeName)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Name == nodeName);
        }
    }
}
