using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileStorage.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace FileStorage.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;

        }


        [HttpGet]
        public async Task<IActionResult> GetSmth()
        {
            //User.Identity.

            var obj = User;
            var value = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok(value);
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