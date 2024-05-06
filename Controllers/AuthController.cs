using CatalogAPI.DTOs;
using CatalogAPI.Models;
using CatalogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace CatalogAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdminOnly")]
        [Route("CreateRole")]
        public async Task<ActionResult> CreateRole(string roleName) 
        {
            // Verifica se o role já existe
            var roleAlreadyExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleAlreadyExists) 
            {
                // Cria nova role
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                // Verifica se o role foi criado com sucesso e faz log
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation(1, "Roles Added");
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { 
                            Status = "Success", 
                            Message = $"Role {roleName} added successfully"
                        });
                }
                else 
                {
                    _logger.LogInformation(2, "Error");
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response
                        {
                            Status = "Error",
                            Message = $"We had an issue adding the {roleName} role"
                        });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = $"We had an issue adding the {roleName} role"
                });
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdminOnly")]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            // Busca o usuário
            var user = await _userManager.FindByEmailAsync(email);

            // Verifica se usuário existe
            if (user != null)
            {
                // Adiciona role ao usuário
                var result = await _userManager.AddToRoleAsync(user, roleName);
                
                if (result.Succeeded) 
                {
                    _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                    return StatusCode(StatusCodes.Status200OK,
                        new Response
                        {
                            Status = "Success",
                            Message = $"User {user.Email} added to the {roleName} role"
                        });
                }
                else
                {
                    _logger.LogInformation(1, $"User {user.Email} could not be added to the {roleName} role");
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response
                        {
                            Status = "Error",
                            Message = $"User {user.Email} could not be added to the {roleName} role"
                        });
                }
            }
            return BadRequest(new { error = "Unable to find user" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName!);

            // Check if user exists and if password is valid
            if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                // List of claims to build the access token
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("id", user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Generates a GUID for the token

                };

                // Adds claims to token
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Generating access token
                var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

                // Generating refresh token
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Get refresh token validity
                // _ is a disposable variable, after all, the value will go to a new variable, refreshTokenValidityInMinutes
                _ = int.TryParse(
                    _configuration["JWT:RefreshTokenValidityInMinutes"], 
                    out int refreshTokenValidityInMinutes
                );

                // Sets refreshToken
                user.RefreshToken = refreshToken;

                // Sets expiration time
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

                // Updating user info
                await _userManager.UpdateAsync(user);

                // Return object with tokens and expiration date.
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
        {
            // Check if user exists
            var userExists = await _userManager.FindByNameAsync(model.Username!);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

            // Creates the user
            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            // If creation fails
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed!" });
            }

            // If it succeeds
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
        {
            // Checks if tokenModel is null
            if (tokenModel is null)
                return BadRequest("Invalid client request");

            // Sets access token or throw exception
            string? accessToken = tokenModel.AccessToken
                ?? throw new ArgumentNullException(nameof(tokenModel));

            // Sets refresh token or throw exception
            string? refreshToken = tokenModel.RefreshToken
                ?? throw new ArgumentNullException(nameof(tokenModel));

            // Get claims
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

            if (principal == null)
            {
                return BadRequest("Invalid access or refresh token");
            }

            // Get username
            string username = principal.Identity.Name;

            // Get user
            var user = await _userManager.FindByNameAsync(username!);

            // Checks user tokens
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid access or refresh token");

            // Generate new access token
            var newAccessToken = _tokenService.GenerateAccessToken(
                principal.Claims.ToList(), _configuration);

            // Generate new refresh token
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Updates user info
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        [Authorize(Policy = "ExclusiveOnly")]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            // Find user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("Invalid user name");

            // Revokes refresh token
            user.RefreshToken = null;

            // Update user info
            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}
