using InventoryUI.Services;
using Microsoft.AspNetCore.Components;

namespace InventoryUI.Pages
{
    public partial class PostFloor
    {
        [Inject]
        public IFloorService FloorService { get; set; } 

        protected Shared.DTOs.PostFloor FloorModel { get; set; } 
        protected bool IsSubmitting { get; set; } 
        protected string Message { get; set; }
        protected bool IsSuccess { get; set; }


        public PostFloor()
        {
            FloorModel = new Shared.DTOs.PostFloor();
            IsSubmitting = false;
            Message = string.Empty;
            IsSuccess = false;
        }
        protected async Task HandleSubmit()
        {
            IsSubmitting = true;
            Message = string.Empty;

            // استدعاء الـ API من خلال الخدمة
            bool result = await FloorService.AddFloorAsync(FloorModel);

            if (result)
            {
                IsSuccess = true;
                Message = $"تمت إضافة الطابق ({FloorModel.Name}) بنجاح عبر الـ API!";
                FloorModel = new Shared.DTOs.PostFloor(); // إعادة تصفير النموذج
            }
            else
            {
                IsSuccess = false;
                Message = "حدث خطأ أثناء الاتصال بالـ API أو حفظ البيانات.";
            }

            IsSubmitting = false;
        }
    }
}
