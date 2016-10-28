using System.Threading.Tasks;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;
using FileStorage.Services.RequestModels;

namespace FileStorage.Services.Contracts
{
    public interface IUserService
    {
        ServiceState State { get; }
        Task<ServiceState> CreateAsync(RegistrationRequest model);
    }
}
