using System.Collections.Immutable;

using Microsoft.AspNetCore.SignalR;

using Sabacc.Hubs;

namespace Sabacc.Domain;

public class ClassicSabaccCloudCityRules : ISabaccSession
{
    public Guid Id { get; }
    public int Slots { get; }
    public int Round { get; }
    public IImmutableList<Guid> PlayerIds { get; }
    public SabaccVariantType VariantType { get; }
    public SessionStatus Status { get; }

    public void SetSlots(int slots)
    {
        throw new NotImplementedException();
    }

    public async Task JoinSession(Guid playerId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task LeaveSession(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public PlayerViewModel GetPlayerView(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public SpectatorView GetSpectateView()
    {
        throw new NotImplementedException();
    }

    public async Task PlayerTurn(Guid playerId, PlayerAction action)
    {
        throw new NotImplementedException();
    }
}