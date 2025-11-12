using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Attributies
        private readonly IUserService _userService;
        #endregion

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Methods
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateDTO dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return Ok(user);
        }
        #endregion
    }
}
