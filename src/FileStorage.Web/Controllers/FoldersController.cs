using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.RequestModels;
using FileStorage.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileStorage.Web.Controllers
{
    /// <summary>
    /// Resource for manage folders in file storage system
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/folders")]
    public class FoldersController : Controller
    {
        private readonly IFolderService _folderService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="folderService"></param>
        public FoldersController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        /// <summary>
        /// Tree view on user folders 
        /// </summary>
        /// <remarks>
        /// # Description
        /// 
        /// Return all folders that exist in root user folder, with releated files and folders that exist on user file storage <br/>
        /// Response of the request contain all information about folders and files on in that exist in root user folder in tree-view, include recursively files and folder that contains in other folders
        /// 
        /// </remarks>
        /// <response code="200">Return Root user folder with all existed data that user have on his workplace</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet("tree-view")]
        [ProducesResponseType(typeof(FolderDto), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResult), 500)]
        public async Task<IActionResult> GetFoldersForUser()
        {
            try
            {
                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var folders = await _folderService.GetFoldersForUserAsync(callerEmail);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return Ok(folders);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Returns list of user folders
        /// </summary>
        /// <response code="200">Return array of user folders</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FolderDto>), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResult), 500)]
        public async Task<IActionResult> GetListFolders()
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var folders = await _folderService.GetListFolder(callerEmail);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return Ok(folders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get folder
        /// </summary>
        /// <remarks>
        /// ## Description
        /// 
        /// Return folder information with folders and files that under requested folder
        /// 
        /// </remarks>
        /// <param name="uniqFolderId">Directory Id</param>
        /// <response code="200">Return information of folder</response>
        /// <response code="400">Returns if values invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested folder</response>
        /// <response code="404">Returns if passed folder are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet]
        [Route("{uniqFolderId}", Name = "GetFolder")]
        public async Task<IActionResult> GetConcreteFolder(Guid uniqFolderId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var folder = await _folderService.GetFolderForUserAsync(callerEmail, uniqFolderId);

                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return Ok(folder);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <remarks>
        /// ## Description
        /// 
        /// Creating folder in the file storage, if parent id not passed, folder will be created on root user folder
        /// 
        /// </remarks>
        /// <response code="200">Return folder information with Location header where information of folder can be requested</response>
        /// <response code="400">Returns if requested values invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested upload folder</response>
        /// <response code="404">Returns if passed folder are not exist (source or dest folder are not found)</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var response = await _folderService.AddFolderAsync(callerEmail, request);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return CreatedAtRoute("GetFolder", new { uniqFolderId = response.UniqueFolderId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Replace folder
        /// </summary>
        /// <remarks>
        /// ## Description
        /// 
        /// Replacing folder to destanation folder folder with all child files and folders on parent folder.
        /// 
        /// </remarks>
        /// <response code="204">Return if folder replaced successfully</response>
        /// <response code="400">Returns if values invalid (Destination folder is same as requested folder) </response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested upload folder or destanation folder</response>
        /// <response code="404">Returns if passed folder are not exist (source or dest folder are not found)</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPatch]
        [Route("replace/{uniqFolderId}")]
        public async Task<IActionResult> Replace(Guid uniqFolderId, [FromBody] ReplaceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _folderService.ReplaceFolderAsync(callerEmail, uniqFolderId, request);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Rename folder
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Renaming a name of the folder
        /// </remarks>
        /// <param name="uniqFolderId">Unique folder Id</param>
        /// <param name="request">Request</param>
        /// <response code="204">Return if folder renamed successfully</response>
        /// <response code="400">Returns if request are invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested folder</response>
        /// <response code="404">Returns if passed folder are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPatch]
        [Route("rename/{uniqFolderId}")]
        public async Task<IActionResult> RenameFolder(Guid uniqFolderId, [FromBody] RenameRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _folderService.RenameFolderAsync(uniqFolderId, request.NewName, callerEmail);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Removing folder with all releated childs in folder to recycle bin and placing on 30 days.
        /// </remarks>
        /// <param name="uniqFolderId">Unique folder Id</param>
        /// <response code="204">Return if folder removed successfully</response>
        /// <response code="400">Returns if request are wrong</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested folder</response>
        /// <response code="404">Returns if passed folder are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [Route("{uniqFolderId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFolder(Guid uniqFolderId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                await _folderService.RemoveFolderAsync(uniqFolderId, callerEmail);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return StatusCode(204);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Restoring removed folder from recycle bin
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Each removed folders placed in recycle bin on 30 days. After that they will be removed forever.
        /// This request can restore removed folder 
        /// </remarks>
        /// <param name="uniqFolderId">Unique folder Id</param>
        /// <response code="200">Return folder information about restored folder with Location header where information of folder can be requested</response>
        /// <response code="400">Returns if folder are not removed, and other similar situations</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed folder are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPut]
        [Route("restore/{uniqFolderId}")]

        public async Task<IActionResult> RestoreFolder(Guid uniqFolderId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var result = await _folderService.RestoreRemovedFolderAsync(uniqFolderId, callerEmail);
                if (!_folderService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _folderService.State.TypeOfError,
                        _folderService.State.ErrorMessage);

                return CreatedAtRoute("GetFolder", new { uniqFolderId = result.UniqueFolderId }, result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
