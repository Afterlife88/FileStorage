using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.Domain.Infrastructure.Contracts.Repositories
{
    public interface INodeRepository
    {
        void AddNode(Node node);
        Task<Node> GetNodeById(int nodeId);
        Task<Node> GetNodeByName(string nodeName);
        Task<IEnumerable<Node>> GetAllNodesForUser(string userId);
    }
}
