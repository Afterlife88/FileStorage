using System.Threading.Tasks;
using FileStorage.Web.DTO;
using FileStorage.Web.Models;

namespace FileStorage.Web.Contracts
{
    public interface IUserService
    {
        ModelState State { get; }
        Task<ModelState> CreateAsync(RegistrationModelDto model);
    }
}
