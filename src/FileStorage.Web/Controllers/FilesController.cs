using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;


namespace FileStorage.Web.Controllers
{
    //[Authorize]
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
                    return StatusCode(500, _fileService.State.ErrorMessage);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Route("{fileName}")]
        [HttpGet]
        public async Task<IActionResult> GetFile(string fileName, [FromQuery]int? versionOfFile = null)
        {
            try
            {
                if (versionOfFile == null)
                {
                    var responseFromService = await _fileService.GetLastVersionOfFile(fileName);
                    if (!_fileService.State.IsValid)
                        return StatusCode(500, _fileService.State.ErrorMessage);

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
        /// Action to upload file
        /// </summary>
        /// <param name="directoryId">Root folder ID</param>
        /// <param name="file"></param>
        [Route("{directoryId}")]
        [HttpPost]
        public async Task<IActionResult> Upload(int directoryId, IFormFile file)
        {
            try
            {
                // TODO: Validate if content type is not form-data
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var serviceResponse = await _fileService.UploadAsync(file, directoryId, userEmail);

                if (serviceResponse.IsValid)
                {
                    return StatusCode(201);
                }
                return BadRequest(serviceResponse.ErrorMessage);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }
        }
    }
}