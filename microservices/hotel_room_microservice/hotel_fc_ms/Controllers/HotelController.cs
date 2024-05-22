using System;
using Microsoft.AspNetCore.Mvc;
using hotel_fc_ms.Data;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using hotel_fc_ms.Services;
namespace hotel_fc_ms.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    public class HotelController : ControllerBase
	{

        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;
        public HotelController(ApiDbContext context, IAuthenticationService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Hotel>> Get()
        {
            return await _context.Hotels.ToListAsync();
        }
        
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            //return Ok("The hotel Test Hotel has been added");
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
                hotel.UserUid = responseBody;
                if (_context != null && _context.Hotels != null)
                {
                    var isExistingHotel = await HotelExists(hotel.Name); // Call the HotelExists method
                    if (isExistingHotel == true)
                    {
                        return Unauthorized("The hotel already exists...");
                    }
                    _context.Hotels.Add(hotel);
                    await _context.SaveChangesAsync();
                    return Ok($"The hotel {hotel.Name} has been added");
                } else
                {
                    return BadRequest("Database error: Unable to add hotel.");
                }

            }
            return BadRequest("L'utilisateur n'est pas authentifié...");
        }

        [HttpGet("{hotelId}")]
        public async Task<ActionResult<Hotel>> Get(int hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);

            if (hotel == null)
            {
                return NotFound();
            }
            return hotel;
        }

        private async Task<Boolean> HotelExists(string name)
        {
            var isExistingHotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Name == name);
            if(isExistingHotel != null)
            {
                return true;
            }
            return false;
        }
    }
}

