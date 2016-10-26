using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.Domain.Infrastructure.Contracts.Repositories
{
    public interface INodeRepository
    {
        void AddNode(Node node);
        Task<Node> GetNodeById(Guid nodeId);
        Task<Node> GetNodeByName(string nodeName);
        Task<IEnumerable<Node>> GetAllNodesForUser(string userId);
        Task<Node> GetRootFolderForUser(string userId);
    }
}
