using System.Threading.Tasks;
using FileStorage.DAL.Contracts;
using FileStorage.DAL.Contracts.Repositories;

namespace FileStorage.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Constructors / Destructors

        public UnitOfWork(DataDbContext dataDbContext, INodeRepository nodeRepository, 
            IUserRepository userRepository, 
            IFileVersionRepository fileVersionRepository, IRemovedNodeRepository removedNodeRepository)
        {
            DataDbContext = dataDbContext;
            NodeRepository = nodeRepository;
            UserRepository = userRepository;
            FileVersionRepository = fileVersionRepository;
            RemovedNodeRepository = removedNodeRepository;
        }

        #endregion

        /// <summary>
		/// Db context
		/// </summary>
        private DataDbContext DataDbContext { get; }
        public INodeRepository NodeRepository { get; }
        public IUserRepository UserRepository { get; }
        public IFileVersionRepository FileVersionRepository { get; }
        public IRemovedNodeRepository RemovedNodeRepository { get; }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public async Task<int> CommitAsync()
        {
            return await DataDbContext.SaveChangesAsync();
        }

    }
}
