using System.Threading.Tasks;
using FileStorage.DAL.Contracts.Repositories;

namespace FileStorage.DAL.Contracts
{
    /// <summary>
	/// Interface for the unit of work
	/// </summary>
    public interface IUnitOfWork
    {
        INodeRepository NodeRepository { get; }
        IUserRepository UserRepository { get; }
        IFileVersionRepository FileVersionRepository { get; }
        IRemovedNodeRepository RemovedNodeRepository { get; }
        /// <summary>
        /// Save changes in database
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();
    }
}
