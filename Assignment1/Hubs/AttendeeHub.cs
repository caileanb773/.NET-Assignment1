using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Assignment1.Hubs
{
    [Authorize]
    public class AttendeeHub : Hub
    {
        public async Task JoinEventGroup(int eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event-{eventId}");
        }
    }
}
