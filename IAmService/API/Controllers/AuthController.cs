using Application.Interfaces;
using Application.Models;
using Domain.Aggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Attributies
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        #endregion

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO userCreateDTO)
        {
            var response = await _userService.CreateUserAsync(userCreateDTO);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDTO loginDTO)
        {
            var authResponse = await _authService.Login(loginDTO);
            return Ok(authResponse);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            var response = await _authService.RefreshAccessTokenAsync(refreshTokenDTO);
            return Ok(response);
        }


    }
}
