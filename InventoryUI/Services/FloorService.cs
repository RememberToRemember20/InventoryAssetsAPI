
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
        public async Task<bool> DeleteAssetAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Asset/DeleteAsset/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<GetAsset?> GetAssetByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<GetAsset>($"api/Asset/GetAssetById/{id}");
            }
            catch
            {
                return null;
            }
        }

        // تحديث بيانات الأصل
        public async Task<bool> UpdateAssetAsync(int id, PostAsset assetDto)
        {
            // افترضنا أن الـ Endpoint في الـ API اسمها UpdateAsset
            var response = await _http.PutAsJsonAsync($"api/Asset/UpdateAsset/{id}", assetDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> StartSessionAsync(CreateAuditSessionDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/Session/StartSession", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<StartSessionResponseDTO>();
            return result.SessionId; // بافتراض أن الـ API يرجع SessionId
        }

        public async Task<ScanResultDTO> ScanBarcodeAsync(ScanBarcodeDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/Session/ScanBarcode", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ScanResultDTO>();
        }

        public async Task<AuditSummaryDTO> GetSessionSummaryAsync(int sessionId)
        {
            return await _http.GetFromJsonAsync<AuditSummaryDTO>($"api/Session/SessionSummary/{sessionId}");
        }

        public async Task<string> ReconcileSessionAsync(int sessionId)
        {
            var response = await _http.PostAsJsonAsync($"api/Session/ReconcileSession/{sessionId}", new { });
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ReconcileSessionDTO>();
            return result.Message;
        }

        public async Task<List<GetRoom>> GetRoomsAsync()
        {
            return await _http.GetFromJsonAsync<List<GetRoom>>("api/Room/GetRoom")
                   ?? new List<GetRoom>();
        }
        public async Task<ReconciliationReportDTO> GetReconciliationReportAsync(int sessionId)
        {
            var response = await _http.GetAsync($"api/Session/report/{sessionId}");
            response.EnsureSuccessStatusCode();

            var report = await response.Content.ReadFromJsonAsync<ReconciliationReportDTO>();

            if (report == null)
                throw new Exception("فشل في تحليل بيانات التقرير.");

            return report;
        }
        public async Task<List<AuditSessionListDTO>> GetAllAuditSessionsAsync()
        {
            return await _http.GetFromJsonAsync<List<AuditSessionListDTO>>("api/Session/sessions")
                   ?? new List<AuditSessionListDTO>();
        }
        public async Task<ScannedItemDTO> AddScanToSessionAsync(int sessionId, long barcode)
        {
            var requestBody = new { Barcode = barcode };
            var response = await _http.PostAsJsonAsync($"api/Session/{sessionId}/scan", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                // قراءة رسالة الخطأ من الـ API (مثل "الباركود غير صحيح" أو "الجلسة مغلقة")
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"فشل إضافة الباركود: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<ScannedItemDTO>();
            return result ?? throw new Exception("لم يتم استرجاع بيانات العنصر المضاف.");
        }

        // 2. خدمة إغلاق الجلسة
        public async Task FinalizeAuditSessionAsync(int sessionId)
        {
            var response = await _http.PostAsync($"api/Session/{sessionId}/finalize", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"فشل إغلاق الجلسة: {error}");
            }
        }
    }
}
