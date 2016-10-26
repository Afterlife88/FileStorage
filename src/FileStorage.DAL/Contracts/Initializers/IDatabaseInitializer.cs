using System.Threading.Tasks;

namespace FileStorage.DAL.Contracts.Initializers
{
    public interface IDatabaseInitializer
    {
        Task Seed();
    }
}
