using BlogSite.Server.Entity;
using BlogSite.Server.Utility;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogSite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const string ADMIN_PASS = "Pa$$w0rd";

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<AppUserViewModel>> Login([FromBody] LoginViewModel loginVm)
        {
            var jwtManager = new JwtAuthenticationManager(_signInManager, _userManager);
            var userSession = await jwtManager.GenerateJwtToken(loginVm.Username, loginVm.Password);
            if (userSession == null)
            {
                return Unauthorized();
            }
            else
            {
                return userSession;
            }
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel registerVm)
        {
            try
            {
                if (registerVm.Password != registerVm.ConfirmPassword)
                {
                    return BadRequest("Passwords don't match");
                }
                var user = await _userManager.FindByEmailAsync(registerVm.Email);
                if (user != null)
                {
                    return BadRequest("Email already exists");
                }

                user = new AppUser
                {
                    UserName = registerVm.Email,
                    Email = registerVm.Email,
                    FirstName = registerVm.FirstName,
                    LastName = registerVm.LastName
                };

                var result = await _userManager.CreateAsync(user, registerVm.Password);
                if (result.Succeeded)
                {
                    if (registerVm.AdminPassword == ADMIN_PASS)
                    {
                        var adminRole = await _roleManager.RoleExistsAsync("admin");
                        if (adminRole == false)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("admin"));
                        }
                        await _userManager.AddToRoleAsync(user, "admin");
                    }
                    return Ok(true);
                }
                else
                {
                    return BadRequest("Something went wrong");
                }
            }
            catch (Exception ex)
            {
                var message = "Exception:" + ex.Message;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                }
                return BadRequest(message);
            }

        }
    }
}
