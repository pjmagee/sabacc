using System.Collections.Immutable;

using Sabacc.Services;

namespace Sabacc.Domain;

public interface ISabaccSession
{
    Guid Id { get; }
    public int Slots { get; }

    IImmutableList<Guid> PlayerIds { get; }
    SabaccVariantType VariantType { get; }
    SessionStatus Status { get; }

    void SetSlots(int slots);
    Task JoinSession(Guid id, string name);
    Task LeaveSession(Guid id);

    PlayerViewModel GetPlayerView(Guid playerId);
    SpectatorView GetSpectateView();
    Task PlayerTurn(Guid playerId, PlayerAction action);
}