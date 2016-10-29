using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;

namespace FileStorage.Services.Implementation
{
    public class SearchService : ISearchService
    {
        private IUnitOfWork _unitOfWork;
        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SearchResultDto>> SearchFiles(string user, string query, bool isRemoved = false)
        {
            var caller = await _unitOfWork.UserRepository.GetUserByNameAsync(user);
            var res = await _unitOfWork.NodeRepository.GetAllFolUserWithQuery(query, caller.Id, isRemoved);

            var dtoList = new List<SearchResultDto>();

            var searchFiles = res as Node[] ?? res.ToArray();
            foreach (var item in searchFiles.ToArray())
            {
                if (isRemoved)
                {
                    var removedNodeInfo = await _unitOfWork.RemovedNodeRepository.GetNode(item.Id);
                    dtoList.Add(new SearchResultDto()
                    {
                        IsDirectory = item.IsDirectory,
                        Id = item.Id,
                        ContentType = item.ContentType,
                        Created = item.Created,
                        Name = item.Name,
                        IsDeleted = item.IsDeleted,
                        WillBeRemovedAt = removedNodeInfo.DateOfRemoval
                    });
                }
                else
                {
                    dtoList.Add(new SearchResultDto()
                    {
                        IsDirectory = item.IsDirectory,
                        Id = item.Id,
                        ContentType = item.ContentType,
                        Created = item.Created,
                        Name = item.Name,
                        IsDeleted = item.IsDeleted
                    });
                }
               
            }


            return dtoList;


        }
    }
}
