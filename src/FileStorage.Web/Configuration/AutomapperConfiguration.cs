using AutoMapper;
using FileStorage.Domain.Entities;
using FileStorage.Web.DTO;

namespace FileStorage.Web.Configuration
{
    public class AutomapperConfiguration
    {
        /// <summary>
        /// Configuration of automapper maps
        /// </summary>
        public static void Load()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<RegistrationModelDto, ApplicationUser>()
                    .ForMember(dest => dest.Email, dto => dto.MapFrom(src => src.Email))
                    .ForMember(dest => dest.UserName, dto => dto.MapFrom(src => src.Email));
            });
        }
    }
}
