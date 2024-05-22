using System;
using Microsoft.AspNetCore.Mvc;
using options_fc_ms.Data;
using options_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using options_fc_ms.Services;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace options_fc_ms.Controllers
{
    [ApiController]
    [Route("api/hotel_options")]
    public class HotelOptionController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;
        private readonly IHotelService _hotelService;

        public HotelOptionController(ApiDbContext context, IAuthenticationService authenticationService, IHotelService hotelService)
        {
            _context = context;
            _authenticationService = authenticationService;
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IEnumerable<HotelOption>> Get()
        {
            return await _context.HotelOptions.ToListAsync();
        }

        [HttpGet("{HotelOptionId}")]
        public async Task<ActionResult<HotelOption>> Get(int HotelOptionId)
        {
            var HotelOption = await _context.HotelOptions.FindAsync(HotelOptionId);

            if (HotelOption == null)
            {
                return NotFound();
            }

            return HotelOption;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> Create(HotelOption hotelOption)
        {
            if (hotelOption.OptionId == 0 || hotelOption.HotelId == 0 || hotelOption.Quantity == 0)
            {
                return BadRequest("Missing fields...");
            }
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var bearer = headerValue.Parameter;
                var authResponse = await _authenticationService.GetAuthenticationResponseAsync("https://admin-ms/api/authentication/me", bearer!);

                if (!authResponse.IsSuccessStatusCode)
                {
                    return Unauthorized("The User does not exist...");
                }
                var responseBody = await authResponse.Content.ReadAsStringAsync();
                hotelOption.UserUid = responseBody;
                var isValidHotelOption = await IsValidHotelOption(hotelOption.OptionId, hotelOption.HotelId);
                if (isValidHotelOption == true)
                {
                    return Unauthorized("The hotel option already exists or the given hotel does not exist, pls check and try again...");
                }
                _context.HotelOptions.Add(hotelOption);
                await _context.SaveChangesAsync();
                return Ok($"The room option has been added");
            }
            return BadRequest("The user has no authenticated....");
        }

        [HttpPut]
        [Route("{hotelOptionId}")]
        public async Task<IActionResult> Update(int hotelOptionId, [FromBody] HotelOption hotelOption)
        {

            if (hotelOption.OptionId == 0 || hotelOption.HotelId == 0 || hotelOption.Quantity == 0)
            {
                return BadRequest("Missing fields...");
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
                var dbHotelOption = await _context.HotelOptions.FindAsync(hotelOptionId);
                if (dbHotelOption == null)
                {
                    return NotFound("The hotel option was not found...");
                }
                dbHotelOption.Quantity = hotelOption.Quantity;
                _context.HotelOptions.Update(dbHotelOption);
                await _context.SaveChangesAsync();
                return Ok($"The option {dbHotelOption.Id} has been updated");
            }
            return BadRequest("The user has no authenticated...");
        }

        [HttpGet("hotels/{hotelId}")]
        public async Task<ActionResult<IEnumerable<HotelOption>>> GetOptionsByHotel(int hotelId)
        {
            var options = await _context.HotelOptions
                .Where(hr => hr.HotelId == hotelId)
                .ToListAsync();
            if (options == null)
            {
                return NotFound();
            }
            return options;
        }

        private async Task<Boolean> IsValidHotelOption(int optionId, int hotelId)
        {
            var isValidOption = await _context.Options.FindAsync(optionId);

            var hotel = await _hotelService.GetHotelByIdResponseAsync($"https://aspnet-hotel-app/api/hotels/{hotelId}");
            if (isValidOption == null || !hotel.IsSuccessStatusCode)
            {
                return true;
            }
            var existingHotelOption = await _context.HotelOptions
                .FirstOrDefaultAsync(rh => rh.OptionId == optionId && rh.HotelId == hotelId);
            if (existingHotelOption != null)
            {
                return true;
            }
            return false;
        }
    }
}
