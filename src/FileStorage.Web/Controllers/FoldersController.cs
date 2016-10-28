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
    [Authorize]
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
        /// Return all folders that exist in root user folder, with releated files and folders that exist on user file storage
        /// 
        /// </summary>
        /// <remarks>
        /// # Description
        /// 
        /// Response of the request contain all information about folders and files on in that exist in root user folder, include recursively files and folder that contains in other folders
        /// 
        /// </remarks>
        /// <response code="200">Return Root user folder with all existed data that user have on his workplace</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet]
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
        /// 
        /// </summary>
        /// <param name="uniqFolderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{uniqFolderId}", Name = "GetFolder")]
        public async Task<IActionResult> GetConcreteFolder(Guid uniqFolderId)
        {
            try
            {
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
        /// 
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody]CreateFolderRequest requst)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var response = await _folderService.AddFolderAsync(callerEmail, requst);
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

        //// PUT api/values/5
        //[HttpPatch("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
