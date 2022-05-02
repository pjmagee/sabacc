using System.Collections.Immutable;

using Sabacc.Services;

namespace Sabacc.Domain;

public interface ISabaccSession
{
    Guid Id { get; }
    public int Slots { get; }
    public int Round { get; }

    IImmutableList<Guid> PlayerIds { get; }
    SabaccVariantType VariantType { get; }
    SessionStatus Status { get; }

    void SetSlots(int slots);
    void JoinSession(Guid playerId);
    void LeaveSession(Guid playerId);

    PlayerViewModel GetPlayerView(Guid playerId);
    SpectatorView GetSpectateView();
    void PlayerTurn(PlayerAction playerAction);
}