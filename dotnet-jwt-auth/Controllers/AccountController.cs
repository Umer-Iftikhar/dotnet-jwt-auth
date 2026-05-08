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

        public AccountController ( UserManager<ApplicationUser> userManager)
        {
           _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register ([FromBody] RegisterDto registerDto)
        {

            var user = new ApplicationUser();
            user.Email = registerDto.Email;
            user.UserName = registerDto.Name;
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }


            return Created("", new { id = user.Id, message = "User registered successfully" });
        }

    }
}
