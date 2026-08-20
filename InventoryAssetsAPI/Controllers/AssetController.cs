using AutoMapper;
using InventoryAssetsAPI.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

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
            var exist =await _unitOfWork.Assets.Get(q=>q.BarCode == postAsset.BarCode);
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
        public async Task<IActionResult> GetAssets(
     int roomid,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string searchTerm = "")
        {
            var requestParams = new RequestParams { PageNumber = pageNumber < 1 ? 1 : pageNumber, PageSize = pageSize < 1 ? 10 : pageSize };

            // جلب البيانات مع الفلترة (حسب رقم الغرفة وكلمة البحث) والتقطيع
            var pagedAssets = await _unitOfWork.Assets.GetPagingAll(
                expression: a => a.RoomId == roomid &&
                                 (string.IsNullOrEmpty(searchTerm) ||
                                  a.Name.Contains(searchTerm) ||
                                  a.BarCode.ToString().Contains(searchTerm) ||
                                  a.Description.Contains(searchTerm)),
                request: requestParams
            );

            // تحويل قائمة Items فقط إلى DTO
            var mappedItems = _mapper.Map<List<Shared.DTOs.GetAsset>>(pagedAssets.Items);

            // إعادة التغليف بـ PagedResult مع الحفاظ على MetaData
            var result = new PagedResult<Shared.DTOs.GetAsset>
            {
                Items = mappedItems,
                MetaData = pagedAssets.MetaData
            };

            return Ok(result);
        }
        [HttpDelete("DeleteAsset/{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var asset = await _unitOfWork.Assets.Get(q => q.Id == id);
            if (asset == null)
            {
                return NotFound();
            }
            await _unitOfWork.Assets.Delete(id);
            await _unitOfWork.Save();
            return Ok();
        }
        [HttpPut("UpdateAsset/{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] Shared.DTOs.PostAsset postAsset)
        {
            var asset = await _unitOfWork.Assets.Get(q => q.Id == id);
            if (asset == null)
            {
                return NotFound();
            }
            _mapper.Map(postAsset, asset);
             _unitOfWork.Assets.Update(asset);
            await _unitOfWork.Save();
            return Ok(postAsset);
        }
        [HttpGet("GetAssetById/{id}")]
        public async Task<IActionResult> GetAssetById(int id)
        {
            var asset = await _unitOfWork.Assets.Get(q => q.Id == id);
            if (asset == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<Shared.DTOs.GetAsset>(asset);
            return Ok(result);
        }

    }
}
