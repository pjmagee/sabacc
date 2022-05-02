using Microsoft.AspNetCore.SignalR;

namespace Sabacc.Hubs
{
    public class PlayerNotificationHub : Hub
    {
        public const string Method = "Update";
    }
}
