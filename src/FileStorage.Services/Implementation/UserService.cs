using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;

namespace FileStorage.Services.Implementation
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
            State = new ServiceState();
        }
        /// <summary>
        /// Model state of the executed actions
        /// </summary>
        public ServiceState State { get; }

        public async Task<ServiceState> CreateAsync(RegistrationModelDto modelDto)
        {
            if (modelDto.Email.Split(' ').Length == 2)
            {
                State.ErrorMessage = "Email should not contain spaces!";
                State.TypeOfError = TypeOfServiceError.BadRequest;
                return State;
            }
            if (string.IsNullOrWhiteSpace(modelDto.Password))
            {
                State.ErrorMessage = "You must type a password.";
                State.TypeOfError = TypeOfServiceError.BadRequest;
                return State;
            }
            var checkIsUserAlreadyExistWithEmail = await _userRepository.GetUserAsync(modelDto.Email);
            if (checkIsUserAlreadyExistWithEmail != null)
            {
                State.ErrorMessage = "User with requested email already exist!";
                State.TypeOfError = TypeOfServiceError.BadRequest;
                return State;
            }
            // Create user
            var user = new ApplicationUser()
            {
                Email = modelDto.Email,
                UserName = modelDto.Email,
                Nodes = new List<Node>()
            };
            // Create base folder for concrete user
            user.Nodes.Add(new Node()
            {
                Name = modelDto.Email + "_RootFolder",
                IsDirectory = true,
                Created = DateTime.Now,
                

            });
            await _userRepository.CreateAsync(user, modelDto.Password);
            return State;
        }
    }
}
