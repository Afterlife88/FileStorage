using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileStorage.Web.Controllers
{
    /// <summary>
    /// Resource for search file and folders
    /// </summary>
    [Route("api/search")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="searchService"></param>
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Searching files and folders  
        /// </summary>
        /// <remarks>
        /// # Description 
        ///  If query parameters do not passed, returns all not removed files and folders that user have
        /// </remarks>
        /// <param name="query"></param>
        /// <param name="includeRemoved"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SearchResultDto>), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResult), 500)]
        public async Task<IActionResult> Get([FromQuery] string query = null, [FromQuery] bool includeRemoved = false)
        {
            try
            {
                var callerEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await _searchService.SearchFilesAsync(callerEmail, query, includeRemoved);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
