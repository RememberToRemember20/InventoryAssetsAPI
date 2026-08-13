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
    public class FloorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FloorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("PostFloor")]
        public async Task<IActionResult> PostFloor([FromBody] PostFloor postFloor)
        {
            if (postFloor == null)
            {
                return BadRequest();
            }
            var floor=_mapper.Map<Floor>(postFloor);
           await _unitOfWork.Floors.Insert(floor);
            await _unitOfWork.Save();
            return Ok(postFloor);
        }
        [HttpGet("GetFloor")]
        public async Task<IActionResult> GetFloors()
        {
            var floors = await _unitOfWork.Floors.GetAll(include:q=>q.Include(r=>r.Rooms));
            var result = _mapper.Map<List<GetFloor>>(floors);
            return Ok(result);
        }
        [HttpGet("GetFloorById/{id}")]
        public async Task<IActionResult> GetFloor(int id)
        {
            var floor = await _unitOfWork.Floors.Get(q=>q.Id == id,include:q=>q.Include(r=>r.Rooms));
            if (floor == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<GetFloor>(floor);
            return Ok(result);
        }

    }
}
