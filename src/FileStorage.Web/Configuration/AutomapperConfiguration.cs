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

                config.CreateMap<FileVersion, FileVersionDto>()
                    .ForMember(dest => dest.FileId, dto => dto.MapFrom(src => src.NodeId))
                    .ForMember(dest => dest.Created, dto => dto.MapFrom(src => src.Created))
                    .ForMember(dest => dest.UniqueFileName, dto => dto.MapFrom(src => src.PathToFile))
                    .ForMember(dest => dest.VersionOfFile, dto => dto.MapFrom(src => src.VersionOfFile));



                config.CreateMap<Node, NodeDto>()
                    .ForMember(dest => dest.FileVersions, opt => opt.MapFrom(src => src.FileVersions))
                    .ForMember(dest => dest.DirectoryId, opt => opt.MapFrom(src => src.FolderId))
                    .ForMember(dest => dest.DirectoryName, opt => opt.MapFrom(src => src.Folder.Name));
                //.ForMember(dest => dest.IsDirectory, opt => opt.Condition(src => src.IsDirectory));
            });
        }
    }
}
