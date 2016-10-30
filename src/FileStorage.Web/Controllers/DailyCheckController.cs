using System;
using System.Threading.Tasks;
using FileStorage.Services.Contracts;
using Microsoft.AspNetCore.Mvc;


namespace FileStorage.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DailyCheckController : Controller
    {
        private readonly IBlobService _blobService;
        public DailyCheckController(IBlobService blobService)
        {
            _blobService = blobService;
        }
        // GET: api/dailycheck
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _blobService.CheckLateFilesAsync();
                return Ok("Files checked");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
