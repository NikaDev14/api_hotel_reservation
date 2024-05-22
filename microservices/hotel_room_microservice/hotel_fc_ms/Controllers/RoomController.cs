using System;
using Microsoft.AspNetCore.Mvc;
using hotel_fc_ms.Data;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Net.Http.Headers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Http.Headers;
using hotel_fc_ms.Services;

namespace hotel_fc_ms.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
	{
        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;
        public RoomController(ApiDbContext context, IAuthenticationService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IEnumerable<Room>> Get()
        {
            return await _context.Rooms.ToListAsync();
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Create(Room room)
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
                room.UserUid = responseBody;
                if (_context != null && _context.Rooms != null)
                {
                    var isExistingRoom = await RoomExists(room.Name);
                    if (isExistingRoom == true)
                    {
                        return Unauthorized("The room already exists...");
                    }
                    _context.Rooms.Add(room);
                    await _context.SaveChangesAsync();
                    return Ok($"The room {room.Name} has been added");
                }
                else
                {
                    return BadRequest("Database error: Unable to add room.");
                }

            }
            return BadRequest("L'utilisateur n'est pas authentifié...");
        }

        private async Task<Boolean> RoomExists(string name)
        {
            var isExistingRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Name == name);
            if (isExistingRoom != null)
            {
                return true;
            }
            return false;
        }

        [HttpGet("{roomId}")]
        public async Task<ActionResult<Room>> Get(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }
    }
}

