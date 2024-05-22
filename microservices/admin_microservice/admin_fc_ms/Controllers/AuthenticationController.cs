using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using admin_fc_ms.Models;
using admin_fc_ms.Data;
using admin_fc_ms.Utils;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace UserFcApi.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly APIDbContext _context;
        private readonly TokenService _tokenService;
        public AuthenticationController(UserManager<IdentityUser> userManager, APIDbContext context, TokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        // call this route from the Hotel serice to be accessed only by auth users
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (request.Username == null || request.Email == null || request.Password == null)
            {
                return BadRequest("Bad request...");
            }

            if (request.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters...");
            }

            var userByEmailInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            Console.WriteLine(userByEmailInDb);
            var userByUsernameInDb = _context.Users.FirstOrDefault(u => u.UserName == request.Username);
            if (userByEmailInDb != null || userByUsernameInDb != null)
            {
                return Unauthorized("User already exists...");
            }

            var result = await _userManager.CreateAsync(
                new IdentityUser { UserName = request.Username, Email = request.Email },
                request.Password
            );
            if (result.Succeeded)
            {
                request.Password = "";
                return CreatedAtAction(nameof(Register), new { email = request.Email }, request);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(request.Email);
            if (managedUser == null)
            {
                return BadRequest("Bad credentials");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (userInDb is null)
                return Unauthorized();
            var accessToken = _tokenService.CreateToken(userInDb);
            await _context.SaveChangesAsync();
            return Ok(new AuthResponse
            {
                Username = userInDb.UserName!,
                Email = userInDb.Email!,
                Token = accessToken,
            });
        }

        [HttpGet("me")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetMe()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var bearer = headerValue.Parameter;
                string key = "!SomethingSecretAd!";

                string email = _tokenService.GetEmailFromToken(bearer!, key);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return Ok(user?.Id);
            }
            return BadRequest("La requête est invalide.");
        }

    }
}

