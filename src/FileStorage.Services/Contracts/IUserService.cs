using System.Threading.Tasks;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;

namespace FileStorage.Services.Contracts
{
    public interface IUserService
    {
        ServiceState State { get; }
        Task<ServiceState> CreateAsync(RegistrationModelDto model);
    }
}
