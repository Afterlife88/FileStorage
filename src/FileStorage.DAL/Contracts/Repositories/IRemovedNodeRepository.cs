using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.DAL.Contracts.Repositories
{
    public interface IRemovedNodeRepository
    {
        void AddRemovedNode(RemovedNode node);
        Task DeleteRemovedNodeRecord(Node node);
    }
}
