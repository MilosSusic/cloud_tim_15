using Microsoft.AspNetCore.SignalR;

namespace SmartApiary.WebApi.Hubs
{
    public class SmartScaleHub : Hub
    {
        public async Task JoinApiaryGroup(string apiaryId)
        {
            // Context.ConnectionId je jedinstveni ID browsera/klijenta koji se upravo povezao
            await Groups.AddToGroupAsync(Context.ConnectionId, apiaryId);
        }

        // Metoda za izlazak iz grupe pcelinjaka
        public async Task LeaveApiaryGroup(string apiaryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, apiaryId);
        }
    }
}
