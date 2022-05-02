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

    public ClassicSabaccCloudCityRules()
    {

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

    public PlayerViewModel GetPlayerView(Guid playerId)
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