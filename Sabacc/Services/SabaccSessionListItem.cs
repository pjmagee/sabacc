using Sabacc.Domain;

namespace Sabacc.Services;

public class SabaccSessionListItem
{
    public SabaccVariantType VariantType { get; set; }

    public Guid SessionId { get; set; }

    public int Slots { get; set; }

    public int Players { get; set; }

    public SessionStatus SessionStatus { get; set; }

    public SabaccSessionListItem(ISabaccSession session)
    {
        SessionId = session.Id;
        Players = session.PlayerIds.Count;
        SessionStatus = session.Status;
        Slots = session.Slots;
        VariantType = session.VariantType;
    }
}