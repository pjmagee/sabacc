using System.ComponentModel;
using System.Runtime.CompilerServices;

using Sabacc.Annotations;

namespace Sabacc.Domain;

public class Session
{
    public SabaccVariantType Variant { get; set; }
    public Guid Id { get; set; }
    public List<Player> Players { get; set; }
    
    public List<Deck> Deck { get; } = new();
    public List<Pot> Pots { get; set; }
    
    public SessionStatus Status { get; set; } = SessionStatus.Open;

    public Session()
    {
        Id = Guid.NewGuid();
        Pots = new List<Pot>() { new(PotType.TrueSabacc), new(PotType.TheHand) };
        Players = new List<Player>();
    }
}