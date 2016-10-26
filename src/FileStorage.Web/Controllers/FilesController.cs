using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Web.Contracts;
using FileStorage.Web.Utils;
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
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await _fileService.GetUserFiles(userEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

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
        [Route("{fileUniqId}")]
        [HttpGet]
        public async Task<IActionResult> GetFile(Guid fileUniqId, [FromQuery]int? versionOfFile = null)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (versionOfFile == null)
                {
                    var responseFromService = await _fileService.GetLastVersionOfFile(fileUniqId, userEmail);
                    if (!_fileService.State.IsValid)
                        return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

                    HttpContext.Response.ContentLength = responseFromService.Item1.Length;

                    FileStreamResult result = new FileStreamResult(responseFromService.Item1,
                        new MediaTypeHeaderValue(responseFromService.Item2.ContentType))
                    {
                        FileDownloadName = responseFromService.Item2.Name,
                        FileStream = responseFromService.Item1,
                    };
                    return result;
                }
                return Ok();

            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
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
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _fileService.UploadAsync(file, directoryName, userEmail);
                if (!_fileService.State.IsValid)
                    return ServiceResponseDispatcher.ExecuteServiceResponse(this, _fileService.State.TypeOfError, _fileService.State.ErrorMessage);

                return StatusCode(201);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }
        }
    }
}