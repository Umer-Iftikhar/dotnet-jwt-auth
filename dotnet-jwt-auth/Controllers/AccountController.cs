using dotnet_jwt_auth.Data;
using dotnet_jwt_auth.DTOs;
using dotnet_jwt_auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_jwt_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController ( UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
           _userManager = userManager;
            _roleManager = roleManager;
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

            return Ok(new { id = user.Id, email = user.Email, roles = roles });
        }
    }
}
