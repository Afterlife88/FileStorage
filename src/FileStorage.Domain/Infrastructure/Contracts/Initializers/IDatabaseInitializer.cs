using System.Threading.Tasks;

namespace FileStorage.Domain.Infrastructure.Contracts.Initializers
{
    public interface IDatabaseInitializer
    {
        Task Seed();
    }
}
