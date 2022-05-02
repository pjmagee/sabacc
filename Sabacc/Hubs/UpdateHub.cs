using Microsoft.AspNetCore.SignalR;
using Sabacc.Domain;

namespace Sabacc.Hubs
{
    public class UpdateHub : Hub
    {
        public const string Method = "Update";
    }
}
