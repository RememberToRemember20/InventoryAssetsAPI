using InventoryUI.Services;
using Microsoft.AspNetCore.Components;

namespace InventoryUI.Pages
{
    public partial class FloorById
    {
        [Parameter]
        public int FloorId { get; set; }

        [Inject]
        public IFloorService FloorService { get; set; } = default!;

        protected Shared.DTOs.GetFloor? Floor { get; set; }
        protected bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadFloorDetails();
        }

        private async Task LoadFloorDetails()
        {
            IsLoading = true;
            // استدعاء الـ API لجلب الطابق مع الغرف الخاصة به
            Floor = await FloorService.GetFloorWithRoomsAsync(FloorId);
            IsLoading = false;
        }
    }
}
