using System;
using Microsoft.AspNetCore.Mvc;
using options_fc_ms.Data;
using options_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using options_fc_ms.Services;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace options_fc_ms.Controllers
{
    [ApiController]
    [Route("api/options")]
    public class OptionController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;
        public OptionController(ApiDbContext context, IAuthenticationService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Option>> Get()
        {
            return await _context.Options.ToListAsync();
        }
        
        [HttpGet("{optionId}")]
        public async Task<ActionResult<Option>> Get(int optionId)
        {
            var option =  await _context.Options.FindAsync(optionId);

            if (option == null)
            {
                return NotFound();
            }

            return option;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Create(Option option)
        {
            if (option.Name == null)
            {
                return BadRequest("The Name field is mandatory...");
            }
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var bearer = headerValue.Parameter;
                var response = await _authenticationService.GetAuthenticationResponseAsync("https://admin-ms/api/authentication/me", bearer!);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized("The User does not exist...");
                }
                var responseBody = await response.Content.ReadAsStringAsync();
                option.UserUid = responseBody;
                var isExistingOption = await OptionExists(option.Name);
                if (isExistingOption == true)
                {
                    return Unauthorized("The option already exists...");
                }
                _context.Options.Add(option);
                await _context.SaveChangesAsync();
                return Ok($"The option {option.Name} has been added");
            }
            return BadRequest("The user has no authenticated...");
        }

        [HttpPut]
        [Route("{optionId}")]
        public async Task<IActionResult> Update(int optionId, [FromBody] Option option)
        {
            var isExistingOption = await OptionExists(option.Name);
            if (isExistingOption == true)
            {
                return Unauthorized("The option's name already exists...");
            }
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var bearer = headerValue.Parameter;
                var response = await _authenticationService.GetAuthenticationResponseAsync("https://admin-ms/api/authentication/me", bearer!);

                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized("The User does not exist...");
                }
                var dbOption = await _context.Options.FindAsync(optionId);
                if (dbOption == null)
                {
                    return NotFound("The option was not found...");
                }
                dbOption.Name = option.Name;
                dbOption.Price = option.Price;
                _context.Options.Update(dbOption);
                await _context.SaveChangesAsync();
                return Ok($"The option {dbOption.Id} has been updated");
            }
            return BadRequest("The user has no authenticated...");
        }

        private async Task<Boolean> OptionExists(string name)
        {
            var isExistingOption = await _context.Options.FirstOrDefaultAsync(o => o.Name == name);
            if (isExistingOption != null)
            {
                return true;
            }
            return false;
        }
    }
}

