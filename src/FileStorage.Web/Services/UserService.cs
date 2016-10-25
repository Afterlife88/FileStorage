using System.Threading.Tasks;
using AutoMapper;
using FileStorage.Domain.Entities;
using FileStorage.Domain.Infrastructure.Contracts.Repositories;
using FileStorage.Web.Contracts;
using FileStorage.Web.DTO;
using FileStorage.Web.Models;

namespace FileStorage.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            State = new ModelState();
        }
        /// <summary>
        /// Model state of the executed actions
        /// </summary>
        public ModelState State { get; }

        public async Task<ModelState> CreateAsync(RegistrationModelDto modelDto)
        {
            if (modelDto.Email.Split(' ').Length == 2)
            {
                State.ErrorMessage = "Email should not contain spaces! ";
                return State;
            }
            if (string.IsNullOrWhiteSpace(modelDto.Password))
            {
                State.ErrorMessage = "You must type a password.";
                return State;
            }
            var checkIsUserAlreadyExistWithEmail = await _userRepository.GetUserAsync(modelDto.Email);
            if (checkIsUserAlreadyExistWithEmail != null)
            {
                State.ErrorMessage = "The user already exists!";
                return State;
            }
            await _userRepository.CreateAsync(Mapper.Map<RegistrationModelDto, ApplicationUser>(modelDto), modelDto.Password);
            return State;
        }
    }
}
