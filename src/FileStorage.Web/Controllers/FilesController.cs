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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace FileStorage.Web.Controllers
{
    /// <summary>
    /// Resource for manage files in file storage system
    /// </summary>
    [Authorize]
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fileService"></param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // GET api/files
        /// <summary>
        /// Returns all files for user
        /// </summary>
        /// <response code="200">Return array of user files</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileDto>), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResult), 500)]
        public async Task<IActionResult> GetUserFiles()
        {
            try
            {
                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await _fileService.GetUserFilesAsync(callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError,
                        _fileService.State.ErrorMessage);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get file from file storage
        /// </summary>
        /// <remarks>
        /// 
        /// ## Description
        /// Returns data of the file with Content-Type, Content-Lenght and Content-Disposition headers 
        /// 
        /// ## Important
        /// 
        ///    <b>Vendor client documentation (swagger) that you use right now make respond file corrupt and increase size received from server by 2 (you can check it by compare content-length from server and downloaded file from documentation).  
        ///    <br/>This is common bug and it will be solved with next updated I hope
        ///    <br/>
        ///    So use postman, fiddler or client application if you want to test API with responded file</b>
        /// 
        /// </remarks>
        /// <param name="fileUniqId">Unique file ID</param>
        /// <param name="versionOfFile">You can chose version of the requseted file</param>
        /// <response code="200">Return file</response>
        /// <response code="400">Returns if passed value invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed file are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpGet]
        [Route("{fileUniqId}", Name = "GetFile")]
        public async Task<IActionResult> GetFile(Guid fileUniqId, [FromQuery] int? versionOfFile = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var responseFromService = await _fileService.GetFileAsync(fileUniqId, callerEmail, versionOfFile);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError,
                        _fileService.State.ErrorMessage);

                HttpContext.Response.ContentLength = responseFromService.Item1.Length;

                FileStreamResult result = new FileStreamResult(responseFromService.Item1,
                    new MediaTypeHeaderValue(responseFromService.Item2.ContentType))
                {
                    FileDownloadName = responseFromService.Item2.Name,
                    FileStream = responseFromService.Item1,
                };
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Upload file to service
        /// </summary>
        /// <remarks>
        /// ## Description
        /// 
        /// Uploading file from Multipart/form-data, generating hash, checking if this file already exist,
        /// if exsit returning bad request, if file hash is different - then its new version of file and upload it. 
        /// 
        /// </remarks>
        /// <param name="file">Multipart/form-data of file</param>
        /// <param name="directoryUniqId">Directory Id where file upload to</param>
        /// <response code="200">Return file information with Location header where file can be downloaded</response>
        /// <response code="400">Returns if values invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested upload folder</response>
        /// <response code="404">Returns if passed folder are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery]Guid? directoryUniqId = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var responseFromService = await _fileService.UploadAsync(file, directoryUniqId, callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

                return CreatedAtRoute("GetFile", new { fileUniqId = responseFromService.UniqueFileId }, responseFromService);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Rename file
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Renaming a name of the file
        /// </remarks>
        /// <param name="fileUniqId">Unique file Id</param>
        /// <param name="request">Request</param>
        /// <response code="204">Return if file renamed successfully</response>
        /// <response code="400">Returns if request are invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed file are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("rename/{fileUniqId}")]
        public async Task<IActionResult> RenameFile(Guid fileUniqId, [FromBody]RenameRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _fileService.RenameFileAsync(fileUniqId, request.NewName, callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError,
                        _fileService.State.ErrorMessage);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Replace file
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Replacing file under requested folder 
        /// </remarks>
        /// <param name="fileUniqId">Unique file Id</param>
        /// <param name="request">Request</param>
        /// <response code="204">Return if file replaced successfully</response>
        /// <response code="400">Returns if request are invalid</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed file are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPatch]
        [Route("replace/{fileUniqId}")]
        public async Task<IActionResult> Replace(Guid fileUniqId, [FromBody]ReplaceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _fileService.ReplaceFileAsync(callerEmail, fileUniqId, request);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError,
                        _fileService.State.ErrorMessage);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Restoring removed file from recycle bin
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Each removed file placed in recycle bin on 30 days. After that they will be removed forever.
        /// This request can restore removed file 
        /// </remarks>
        /// <param name="fileUniqId">Unique file Id</param>
        /// <response code="200">Return file information about restored file with Location header where file can be downloaded</response>
        /// <response code="400">Returns if file are not removed, or not found, and other similar situations</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed file are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPut]
        [Route("restore/{fileUniqId}")]
        public async Task<IActionResult> ResotreDeletedFile(Guid fileUniqId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var restoredFile = await _fileService.RestoreRemovedFileAsync(fileUniqId, callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

                return CreatedAtRoute("GetFile", new { fileUniqId = restoredFile.UniqueFileId }, restoredFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <remarks>
        /// ## Description 
        /// Removing file and placing it to recycle bin on 30 days.
        /// </remarks>
        /// <param name="fileUniqId">Unique file Id</param>
        /// <response code="204">Return if file removed successfully</response>
        /// <response code="400">Returns if request are wrong</response>
        /// <response code="401">Returns if authorize token are missing in header or token is wrong</response>
        /// <response code="403">Returns if user have not access to requested file</response>
        /// <response code="404">Returns if passed file are not exist</response>
        /// <response code="500">Returns if server error has occurred</response>
        [Route("{fileUniqId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFile(Guid fileUniqId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                await _fileService.RemoveFileAsync(fileUniqId, callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError,
                        _fileService.State.ErrorMessage);

                return StatusCode(204);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}