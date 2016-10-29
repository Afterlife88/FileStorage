using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorage.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileStorage.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/search")]
    [Authorize]
    public class SearchController : Controller
    {
        private ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        /// <summary>
        /// Searching files and folders
        /// If query parameters do not passed, returns all not removed 
        /// files and folders that user have
        /// </summary>
        /// <remarks>
        /// # Description 
        ///  If query parameters do not passed, returns all not removed files and folders that user have
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="includeRemoved"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string name = null, [FromQuery]bool includeRemoved = false)
        {
            var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var response = await _searchService.SearchFiles(callerEmail, name, includeRemoved);
            return Ok(response);
        }
    }
}
