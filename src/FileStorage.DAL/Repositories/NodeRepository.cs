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


        public async Task<Node> GetFolderByNameForUserAsync(string name, string userId)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.IsDirectory && r.Name == name && r.OwnerId == userId);
        }

        public async Task<Node> GetFolderByIdAsync(Guid id)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.IsDirectory && r.Id == id);
        }

        public async Task<Node> GetFolderByIdForUserAsync(Guid id, string userId)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Id == id && r.IsDirectory && r.OwnerId == userId);
        }
        public async Task<Node> GetRootFolderForUserAsync(string userId)
        {
            _dataDbContext.Nodes.Where(r => r.IsDirectory && r.OwnerId == userId && !r.IsDeleted).Load();
            var node = await
                _dataDbContext.Nodes.Where(r => r.IsDirectory && r.OwnerId == userId && !r.IsDeleted).Include(s => s.Siblings)
                    .FirstOrDefaultAsync();
            return node;
        }
        public void AddNode(Node node)
        {
            _dataDbContext.Nodes.Add(node);
        }
        public async Task<Node> GetNodeByIdAsync(Guid nodeId)
        {
            var node = await _dataDbContext.Nodes.Where(r => r.Id == nodeId && !r.IsDeleted).Include(r => r.Siblings).ThenInclude(r => r.FileVersions).
                FirstOrDefaultAsync();
            return node;
        }

        public async Task<IEnumerable<Node>> GetAllNodesForUserAsync(string userId)
        {
            var nodes = await _dataDbContext.Nodes.Where(r => r.OwnerId == userId && !r.IsDeleted).Include(s => s.FileVersions).ToArrayAsync();
            return nodes;
        }

        public Node RenameNode(Node node, string newName)
        {
            node.Name = newName;
            return node;
        }

        public Node ReplaceNodeFolder(Node node, Node destinationFolder)
        {
            node.Folder = destinationFolder;
            return node;
        }

        public async Task<Node> GetNodeByNameAsync(string nodeName, string userId)
        {
            return await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Name == nodeName && r.OwnerId == userId && !r.IsDeleted);
        }

        public async Task<Node> GetNodeThatWasRemoved(Guid nodeId)
        {
            var node = await _dataDbContext.Nodes.FirstOrDefaultAsync(r => r.Id == nodeId && r.IsDeleted);
            return node;
        }
    }
}
