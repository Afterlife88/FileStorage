using AutoMapper;
using FileStorage.Domain.Entities;
using FileStorage.Services.DTO;
using FileStorage.Services.RequestModels;


namespace FileStorage.Web.Configuration
{
    /// <summary>
    /// Mapping entity to DTO
    /// </summary>
    public class AutomapperConfiguration
    {
        /// <summary>
        /// Configuration of automapper maps
        /// </summary>
        public static void Load()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<RegistrationRequest, ApplicationUser>()
                    .ForMember(dest => dest.Email, dto => dto.MapFrom(src => src.Email))
                    .ForMember(dest => dest.UserName, dto => dto.MapFrom(src => src.Email));

                config.CreateMap<FileVersion, FileVersionDto>()
                    .ForMember(dest => dest.FileId, dto => dto.MapFrom(src => src.NodeId))
                    .ForMember(dest => dest.Created, dto => dto.MapFrom(src => src.Created))
                    .ForMember(dest => dest.UniqueFileName, dto => dto.MapFrom(src => src.PathToFile))
                    .ForMember(dest => dest.VersionOfFile, dto => dto.MapFrom(src => src.VersionOfFile));



                config.CreateMap<Node, FileDto>()
                    .ForMember(dest => dest.UniqueFileId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.FileVersions, opt => opt.MapFrom(src => src.FileVersions))
                    .ForMember(dest => dest.DirectoryId, opt => opt.MapFrom(src => src.FolderId))
                    .ForMember(dest => dest.DirectoryName, opt => opt.MapFrom(src => src.Folder.Name));
                //.AfterMap((src, dest) => src.Siblings.Where(r => r.IsDirectory))

                //.ForMember(dest => dest.IsDirectory, opt => opt.Condition(src => src.IsDirectory));

                config.CreateMap<Node, FolderDto>()
                    .ForMember(dest=>dest.FolderName, opt=>opt.MapFrom(src=>src.Name))
                    .ForMember(dest => dest.ParentFolderName, opt => opt.MapFrom(src => src.Folder.Name))
                    .ForMember(dest => dest.ParentFolderId, opt => opt.MapFrom(src => src.Folder.Id))
                    .ForMember(src => src.UniqueFolderId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(src => src.OwnerId, opt => opt.MapFrom(src => src.OwnerId));
                //config.CreateMap<Node, FolderDto>()
                //    //.ForMember(dest => dest.Files, opt => opt.MapFrom(src => src.Id))
                //    .AfterMap((src, dest) => dest.Files = src.Siblings.Where(r => !r.IsDirectory))
                //    //.ForMember(dest => dest.Folders, opt => opt.MapFrom(src => src.Id))
                //    .AfterMap((src, dest) => dest.Folders = src.Siblings.Where(r => r.IsDirectory));

            });
        }
    }
}
