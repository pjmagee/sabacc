using System.Collections.Immutable;

using Microsoft.AspNetCore.SignalR;

using Sabacc.Hubs;

namespace Sabacc.Domain;

public class ClassicSabaccCloudCityRules : ISabaccSession
{
    private readonly IHubContext<UpdateHub> _hubContext;
    public Guid Id { get; }
    public int Slots { get; }
    public int Round { get; }
    public IImmutableList<Guid> PlayerIds { get; }
    public SabaccVariantType VariantType { get; }
    public SessionStatus Status { get; }

    public ClassicSabaccCloudCityRules(IHubContext<UpdateHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void SetSlots(int slots)
    {
        throw new NotImplementedException();
    }

    public void JoinSession(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public void LeaveSession(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public PlayerView GetPlayerView(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public SpectatorView GetSpectateView()
    {
        throw new NotImplementedException();
    }

    public void PlayerTurn(PlayerAction playerAction)
    {
        throw new NotImplementedException();
    }
}