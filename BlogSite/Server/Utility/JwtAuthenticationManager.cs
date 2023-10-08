using BlogSite.Server.Entity;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogSite.Server.Utility
{
    public class JwtAuthenticationManager
    {
        public const string JWT_SECURITY_KEY = "MySuperStrongSecretStrongKey";
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public JwtAuthenticationManager(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<AppUserViewModel?> GenerateJwtToken(string username, string password)
        {
            //Validating User Credentials
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(username);
                if (user == null)
                {
                    return null;
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

                if (!result.Succeeded)
                {
                    return null;
                }

                //Generating token
                var tokenExpiryTimeStamp = DateTime.Now.AddDays(1);
                var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
                var roles = await _userManager.GetRolesAsync(user);
                var role = "";
                if (roles.Count > 0)
                {
                    role = roles[0];
                }
                var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, user.Email??"")
            });

                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);

                var securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
                    Expires = tokenExpiryTimeStamp,
                    SigningCredentials = signingCredentials,
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(securityTokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                //Return User Session
                var userSession = new AppUserViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = role,
                    Token = token,
                    ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
                };
                return userSession;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
