using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace FileStorage.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        /// 
        /// </summary>
        /// <param name="fileUniqId"></param>
        /// <param name="versionOfFile"></param>
        /// <returns></returns>
        [Route("{fileUniqId}", Name = "GetFile")]
        [HttpGet]
        public async Task<IActionResult> GetFile(Guid fileUniqId, [FromQuery] int? versionOfFile = null)
        {
            try
            {
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
        /// 
        /// </summary>
        /// <param name="fileUniqId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{fileUniqId}")]
        public async Task<IActionResult> RenameFile(Guid fileUniqId, [FromBody]RenameFileDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _fileService.RenameFileAsync(fileUniqId, model.NewName, callerEmail);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directoryName">Optional directory name of the where file upload to</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery]string directoryName = null)
        {
            try
            {
                // TODO: Validate if content type is not form-data
                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var responseFromService = await _fileService.UploadAsync(file, directoryName, callerEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

                return CreatedAtRoute("GetFile", new { fileUniqId = responseFromService.UniqueFileId }, responseFromService);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}