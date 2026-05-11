using dotnet_jwt_auth.Data;
using dotnet_jwt_auth.DTOs;
using dotnet_jwt_auth.Models;
using dotnet_jwt_auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_jwt_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController ( UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
           _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register ([FromBody] RegisterDto registerDto)
        {

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Name
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Created("", new { id = user.Id, message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid Email or Password");
            }

            var password = await _userManager.CheckPasswordAsync(user,loginDto.Password);
            if(!password)
            {
                return Unauthorized("Invalid Email or Password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(token);
        }
    }
}
