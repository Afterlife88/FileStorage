using System.Threading.Tasks;
using FileStorage.Domain.Infrastructure.Contracts;
using FileStorage.Domain.Infrastructure.Contracts.Repositories;

namespace FileStorage.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Constructors / Destructors

        public UnitOfWork(DataDbContext dataDbContext, INodeRepository nodeRepository, IUserRepository userRepository, IFileVersionRepository fileVersionRepository)
        {
            DataDbContext = dataDbContext;
            NodeRepository = nodeRepository;
            UserRepository = userRepository;
            FileVersionRepository = fileVersionRepository;
        }

        #endregion

        /// <summary>
		/// Db context
		/// </summary>
		public DataDbContext DataDbContext { get; set; }
        public INodeRepository NodeRepository { get; }
        public IUserRepository UserRepository { get; }
        public IFileVersionRepository FileVersionRepository { get; }
        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public async Task<int> CommitAsync()
        {
            return await DataDbContext.SaveChangesAsync();
        }

    }
}
