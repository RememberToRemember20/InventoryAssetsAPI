using AutoMapper;
using InventoryAssetsAPI.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAssetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AssetController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("PostAsset")]
        public async Task<IActionResult> PostAsset([FromBody] Shared.DTOs.PostAsset postAsset)
        {
            var exist =_unitOfWork.Assets.Get(q=>q.BarCode == postAsset.BarCode);
            if (exist != null)
            {
                return BadRequest("Asset with this barcode already exists.");
            }
            if (postAsset == null)
            {
                return BadRequest();
            }
            var asset = _mapper.Map<Models.Asset>(postAsset);
            await _unitOfWork.Assets.Insert(asset);
            await _unitOfWork.Save();
            return Ok(postAsset);
        }
        [HttpGet("GetAssets/{roomid}")]
        public async Task<IActionResult> GetAssets(int roomid)
        {
            var assets = await _unitOfWork.Assets.GetAll(q=>q.RoomId == roomid);
            var result = _mapper.Map<List<Shared.DTOs.GetAsset>>(assets);
            return Ok(result);
        }

    }
}
