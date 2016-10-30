using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.DAL.Contracts.Repositories
{
    public interface IRemovedNodeRepository
    {
        Task<RemovedNode> GetNode(Guid nodeId);
        Task<IEnumerable<RemovedNode>> GetAllRemovedNodes();
        void AddRemovedNode(RemovedNode node);
        Task DeleteRemovedNodeRecord(Node node);
        Task<IEnumerable<RemovedNode>> GetLateFiles();
    }
}
