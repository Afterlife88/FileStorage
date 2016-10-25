using System.Threading.Tasks;
using FileStorage.Domain.Infrastructure.Contracts.Repositories;

namespace FileStorage.Domain.Infrastructure.Contracts
{
    /// <summary>
	/// Interface for the unit of work
	/// </summary>
    public interface IUnitOfWork
    {
        INodeRepository NodeRepository { get; }
        IUserRepository UserRepository { get; }
        IFileVersionRepository FileVersionRepository { get; }
        /// <summary>
        /// Save changes in database
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();
    }
}
