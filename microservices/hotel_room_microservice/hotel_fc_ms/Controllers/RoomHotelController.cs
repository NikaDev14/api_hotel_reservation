using System;
using Microsoft.AspNetCore.Mvc;
using hotel_fc_ms.Data;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using hotel_fc_ms.Services;
namespace hotel_fc_ms.Controllers
{
    [ApiController]
    [Route("api/room_hotels")]
    public class RoomHotelController : ControllerBase
	{
        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;
        public RoomHotelController(ApiDbContext context, IAuthenticationService authenticationService)
		{
            _context = context;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IEnumerable<RoomHotel>> Get()
        {
            return await _context.RoomHotels.ToListAsync();
        }

        [HttpGet("hotels/{hotelId}")]
        public async Task<ActionResult<IEnumerable<RoomHotelDetails>>> GetRoomsByHotel(int hotelId)
        {
            var rooms = await _context.RoomHotels
                .Where(hr => hr.HotelId == hotelId)
                .ToListAsync();
            if (rooms == null)
            {
                return NotFound();
            }
            List<RoomHotelDetails> roomHotelDetails = new List<RoomHotelDetails>();
            foreach (var room in rooms)
            {
                var roomSpecs = await _context.Rooms.FindAsync(room.Id);
                if(roomSpecs != null)
                {
                    var roomHotelDetail = new RoomHotelDetails
                    {
                        Name = roomSpecs.Name,
                        NameShortcut = roomSpecs.NameShortcut,
                        nbPersonsMax = roomSpecs.nbPersonsMax,
                        Price = roomSpecs.Price,
                        RemainingRoom = room.nbItems
                    };

                    roomHotelDetails.Add(roomHotelDetail);
                }
            }
            return roomHotelDetails;
        }

        [HttpGet("{roomHotelId}")]
        public async Task<ActionResult<RoomHotelDetails>> GetRoomHotel(int roomHotelId)
        {
            var roomHotel = await _context.RoomHotels.FindAsync(roomHotelId);
            if (roomHotel == null)
            {
                return NotFound();
            }
            var roomSpecs = await _context.Rooms.FindAsync(roomHotel.Id);
            if (roomSpecs != null)
            {
               var roomHotelDetail = new RoomHotelDetails
               {
                 Name = roomSpecs.Name,
                 NameShortcut = roomSpecs.NameShortcut,
                 nbPersonsMax = roomSpecs.nbPersonsMax,
                 Price = roomSpecs.Price,
                 RemainingRoom = roomHotel.nbItems
               };
                return roomHotelDetail;
            }
            return NotFound("There was no any room found...");
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Create(RoomHotel roomHotel)
        {
            if(roomHotel.nbItems == 0 || roomHotel.HotelId == 0 || roomHotel.RoomId == 0)
            {
                return BadRequest("Missing fields...");
            }
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
                roomHotel.UserUid = responseBody;
                if (_context != null && _context.RoomHotels != null)
                {
                    var isExistingRoomHotel = await RoomHotelExists(roomHotel.HotelId, roomHotel.RoomId);
                    if (isExistingRoomHotel == true)
                    {
                        return Unauthorized("The room already exists for this hotel or check room and hotel ids");
                    }
                    _context.RoomHotels.Add(roomHotel);
                    await _context.SaveChangesAsync();
                    return Ok($"The room Hotel has been added");
                }
                else
                {
                    return BadRequest("Database error: Unable to add room.");
                }

            }
            return BadRequest("L'utilisateur n'est pas authentifié...");
        }

        private async Task<Boolean> RoomHotelExists(int hotelId, int roomId)
        {
            var isValidHotel = await _context.Hotels.FindAsync(hotelId);
            var isValidRoom = await _context.Rooms.FindAsync(roomId);
            if(isValidHotel == null || isValidRoom == null)
            {
                return true;
            }
            var existingRoomHotel = await _context.RoomHotels
                .FirstOrDefaultAsync(rh => rh.RoomId == roomId && rh.HotelId == hotelId);
            if (existingRoomHotel != null)
            {
                return true;
            }
            return false;
        }
    }
}

