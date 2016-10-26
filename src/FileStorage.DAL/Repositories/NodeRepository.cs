using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.DAL.Repositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly DataDbContext _dataDbContext;

        public NodeRepository(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }

        public async Task<Node> GetRootFolderForUser(string userId)
        {
            var node = await _dataDbContext.Nodes.Where(r => r.IsDirectory && r.OwnerId == userId).FirstOrDefaultAsync();
            return node;
        }
        public void AddNode(Node node)
        {
            _dataDbContext.Nodes.Add(node);
        }
        public async Task<Node> GetNodeById(Guid nodeId)
        {
            var node = await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Id == nodeId);
            return node;
        }

        public async Task<IEnumerable<Node>> GetAllNodesForUser(string userId)
        {
            var nodes = await _dataDbContext.Nodes.Where(r => r.OwnerId == userId).Include(s => s.FileVersions).ToArrayAsync();
            return nodes;
        }
        public async Task<Node> GetNodeByName(string nodeName)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Name == nodeName);
        }
    }
}
