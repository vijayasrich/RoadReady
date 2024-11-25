using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoadReady.Authentication;
using RoadReady.Models;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var users = await _userManager.FindByNameAsync(model.UserName);
            if (users != null && await _userManager.CheckPasswordAsync(users, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(users);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, users.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser users = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(users, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed: " + errors });
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));

            await _userManager.AddToRoleAsync(users, UserRoles.Customer);

            return Ok(new Response { Status = "Success", Message = "Customer created successfully!" });
        }


        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser users = new ApplicationUser()
            {
                UserName = model.UserName,  // Ensure UserName is set if required
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                //UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(users, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed: " + errors });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            // Assign the user to the Admin role
            await _userManager.AddToRoleAsync(users, UserRoles.Admin);

            return Ok(new Response { Status = "Success", Message = "Admin user created successfully!" });
        }
        /*if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role creation failed: " + roleErrors });
            }
        }

        await _userManager.AddToRoleAsync(users, UserRoles.Admin);
        return Ok(new Response { Status = "Success", Message = "Admin user created successfully!" });*/
    


        [HttpPost("register-agent")]
        public async Task<IActionResult> RegisterAgent([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser users = new ApplicationUser()
            {
                UserName = model.UserName,  // Ensure UserName is set if required
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                //UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(users, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed: " + errors });
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Agent))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(UserRoles.Agent));
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role creation failed: " + roleErrors });
                }
            }

            await _userManager.AddToRoleAsync(users, UserRoles.Agent);
            return Ok(new Response { Status = "Success", Message = "Agent user created successfully!" });
        }

    }
}
