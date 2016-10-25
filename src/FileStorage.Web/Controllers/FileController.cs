using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Web.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.Web.Controllers
{
    //[Authorize]
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
        /// <param name="rootFolderId">Root folder ID</param>
        /// <param name="file"></param>
        [Route("{rootFolderId}")]
        [HttpPost]
        public async Task<IActionResult> Upload(int rootFolderId, IFormFile file)
        {
            try
            {

                return Ok(file);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);

            }
        }
    }
}