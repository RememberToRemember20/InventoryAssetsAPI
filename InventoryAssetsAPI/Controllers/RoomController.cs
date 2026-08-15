using AutoMapper;
using InventoryAssetsAPI.IRepository;
using InventoryAssetsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public RoomController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("PostRoom")]
        public async Task<IActionResult> PostRoom([FromBody] PostRoom postRoom)
        {
            if (postRoom  == null)
            {
                return BadRequest();
            }
            var room = _mapper.Map<Room>(postRoom);
            await _unitOfWork.Rooms.Insert(room);
            await _unitOfWork.Save();
            return Ok(postRoom);
        }
        [HttpGet("GetRoom/{floorid}")]
        public async Task<IActionResult> GetRooms(int floorid)
        {
            var rooms = await _unitOfWork.Rooms.GetAll(q=>q.FloorId == floorid);
            var result = _mapper.Map<List<GetRoom>>(rooms);
            return Ok(result);
        }
        [HttpGet("GetRoomById/{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _unitOfWork.Rooms.Get(q => q.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<GetRoom>(room);
            return Ok(result);
        }
        [HttpGet("GetRoom")]
        public async Task<IActionResult> GetRoom()
        {
            var rooms = await _unitOfWork.Rooms.GetAll();
            var result = _mapper.Map<List<GetRoom>>(rooms);
            return Ok(result);
        }

    }
}
