using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using FileStorage.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;


namespace FileStorage.Web.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserFiles()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var response = await _fileService.GetUserFiles(userEmail);
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
            var obj = new {a = fileName, b = versionOfFile};
            return Ok(obj);
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