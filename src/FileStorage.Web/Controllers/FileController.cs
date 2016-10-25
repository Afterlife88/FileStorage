using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Web.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private IBlobService blob;

        public FileController(IBlobService blobService)
        {
            blob = blobService;

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
        /// <param name="file"></param>
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> PostFile(IFormFile file)
        {
            try
            {
                await blob.UploadFileAsync(file);
                return Ok();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
           
            }
        }
    }
}