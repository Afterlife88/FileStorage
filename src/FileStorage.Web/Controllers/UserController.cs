using System;
using System.Threading.Tasks;
using System.Web.Http;
using FileStorage.Web.Contracts;
using FileStorage.Web.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Web.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a user in file storage service
        /// </summary>
        /// <remarks>
        /// 
        /// <b>Creating the user in service</b>
        /// 
        /// </remarks>
        /// <param name="model"></param>
        /// <response code="201">Returns if user created successfully</response>
        /// <response code="400">Returns if some required fields are missing in request</response>
        /// <response code="500">Returns if server error has occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(StatusCodeResult), 201)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(typeof(InternalServerErrorResult), 500)]
        public async Task<IActionResult> Register([FromBody] RegistrationModelDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var serviceResponse = await _userService.CreateAsync(model);

                if (serviceResponse.IsValid)
                {
                    return StatusCode(201);
                }
                return BadRequest(serviceResponse.ErrorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
