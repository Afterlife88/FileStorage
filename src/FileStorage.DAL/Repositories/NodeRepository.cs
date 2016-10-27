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

        public async Task<Node> GetRootFolderForUserAsync(string userId)
        {
            var node = await _dataDbContext.Nodes.Where(r => r.IsDirectory && r.OwnerId == userId).FirstOrDefaultAsync();
            return node;
        }
        public void AddNode(Node node)
        {
            _dataDbContext.Nodes.Add(node);
        }
        public async Task<Node> GetNodeByIdAsync(Guid nodeId)
        {
            var node = await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Id == nodeId);
            return node;
        }

        public async Task<IEnumerable<Node>> GetAllNodesForUserAsync(string userId)
        {
            var nodes = await _dataDbContext.Nodes.Where(r => r.OwnerId == userId).Include(s => s.FileVersions).ToArrayAsync();
            return nodes;
        }

        public Node RenameNode(Node node, string newName)
        {
            node.Name = newName;
            return node;
        }
        public async Task<Node> GetNodeByNameAsync(string nodeName, string userId)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Name == nodeName && r.OwnerId == userId);
        }
    }
}
