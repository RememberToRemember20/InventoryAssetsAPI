using InventoryUI.Services;
using Microsoft.AspNetCore.Components;

namespace InventoryUI.Pages
{
    public partial class PostRoom
    {
       // [Inject] public IRoomService RoomService { get; set; } = default!;
        [Inject] public IFloorService FloorService { get; set; } = default!;

        protected Shared.DTOs.PostRoom RoomModel { get; set; } = new();
        protected List<Shared.DTOs.GetFloor> Floors { get; set; } = new();

        protected bool IsLoadingFloors { get; set; } = true;
        protected bool IsSubmitting { get; set; } = false;
        protected string Message { get; set; } = string.Empty;
        protected bool IsSuccess { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            // عند فتح الصفحة، نقوم بجلب قائمة الطوابق لتعبئة القائمة المنسدلة
            IsLoadingFloors = true;
            Floors = await FloorService.GetFloorsAsync();
            IsLoadingFloors = false;
        }

        protected async Task HandleSubmit()
        {
            IsSubmitting = true;
            Message = string.Empty;

            // إرسال الكائن المحمل بـ Name و FloorId المحدد
            bool result = await FloorService.AddRoomAsync(RoomModel);

            if (result)
            {
                IsSuccess = true;
                Message = $"تمت إضافة الغرفة ({RoomModel.Name}) بنجاح!";
                RoomModel = new Shared.DTOs.PostRoom(); // تصفير النموذج
            }
            else
            {
                IsSuccess = false;
                Message = "حدث خطأ أثناء الاتصال بالـ API وتنسيق البيانات.";
            }

            IsSubmitting = false;
        }
    }
}
