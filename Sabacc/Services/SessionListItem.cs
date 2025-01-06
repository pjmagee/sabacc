using Sabacc.Domain;

namespace Sabacc.Services;

public class SessionListItem(ISabaccSession session)
{
    public SabaccVariantType VariantType { get; set; } = session.VariantType;

    public Guid SessionId { get; set; } = session.Id;

    public int Slots { get; set; } = session.Slots;

    public int Players { get; set; } = session.PlayerIds.Count;

    public SessionStatus SessionStatus { get; set; } = session.Status;
}