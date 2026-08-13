
using Shared.DTOs;
using System.Net.Http.Json;

namespace InventoryUI.Services
{
    public class FloorService:IFloorService
    {
        private readonly HttpClient _http;

        public FloorService(HttpClient http)
        {
            _http = http;
        }

        //public async Task<bool> AddFloorAsync(PostFloor floorDto)
        //{
        //    // إرسال طلب POST محمل ببيانات الطابق إلى الـ API
        //    var response = await _http.PostAsJsonAsync("api/PostFloor", floorDto);
        //    return response.IsSuccessStatusCode;
        //}

        public async Task<bool> AddFloorAsync(PostFloor floorDto)
        {
            // إرسال طلب POST محمل ببيانات الطابق إلى الـ API
            var response = await _http.PostAsJsonAsync("api/Floor/PostFloor", floorDto);
            return response.IsSuccessStatusCode;

        }

        public async Task<bool> AddRoomAsync(PostRoom roomDto)
        {
            var response = await _http.PostAsJsonAsync("api/Room/PostRoom", roomDto);
            return response.IsSuccessStatusCode;
        }

        //public async Task<List<GetFloor>> GetFloorsAsync()
        //{
        //}

        public async Task<List<GetFloor>> GetFloorsAsync()
        {
            return await _http.GetFromJsonAsync<List<GetFloor>>("api/Floor/GetFloor")
                   ?? new List<GetFloor>();
        }
        public async Task<GetFloor?> GetFloorWithRoomsAsync(int floorId)
        {
            return await _http.GetFromJsonAsync<GetFloor>($"api/Floor/GetFloorById/{floorId}");
        }
        public async Task<(bool Success, string Message)> AddAssetAsync(PostAsset assetDto)
        {
            var response = await _http.PostAsJsonAsync("api/Asset/PostAsset", assetDto);

            if (response.IsSuccessStatusCode)
            {
                return (true, "تمت إضافة الأصل بنجاح!");
            }

            // قراءة رسالة الخطأ في حال تكرار الباركود أو فشل التحقق
            string errorMessage = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMessage) ? "حدث خطأ أثناء حفظ الأصل." : errorMessage);
        }
        public async Task<List<GetAsset>> GetAssetsByRoomAsync(int roomId)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<GetAsset>>($"api/Asset/GetAssets/{roomId}");
                return result ?? new List<GetAsset>();
            }
            catch
            {
                return new List<GetAsset>();
            }
        }
    }
}
