using InventoryUI.Services;
using Microsoft.AspNetCore.Components;

namespace InventoryUI.Pages
{
    public partial class GetFloor
    {
        [Inject]
        public IFloorService FloorService { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        protected List<Shared.DTOs.GetFloor> Floors { get; set; } = new();
        protected bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadFloorsData();
        }

        private async Task LoadFloorsData()
        {
            IsLoading = true;
            // طلب بيانات الطوابق من الـ API
            Floors = await FloorService.GetFloorsAsync();
            IsLoading = false;
        }

        protected void ViewFloorDetails(int floorId)
        {
            // الانتقال إلى صفحة عرض الأصول الخاصة بهذا الطابق المباشرة
            Navigation.NavigateTo($"/floors/{floorId}");
        }
    }
}
