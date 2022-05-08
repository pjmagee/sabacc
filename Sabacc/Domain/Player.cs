using System.Linq;

namespace Sabacc.Domain;

public class Player : IEquatable<Player>
{
    public Guid Id { get; }
    public string Name { get; }
    public int Credits { get; set; } = 1500;
    public Hand Hand { get; private set; }
    public PlayerState State { get; }



    public Player(Guid id, string name)
    {
        Name = name;
        Id = id;
        Hand = new Hand();
        State = new PlayerState();
    }

    public void ShuffleHand()
    {
        Hand = new Hand(Hand.OrderBy(x => Guid.NewGuid()));
    }

    public void PlaceCredits(Pot pot, int credits)
    {
        Credits -= credits;

        pot.Add(this, credits);
    }

    public void TakeCredits(Pot pot)
    {
        Credits += pot.Total;
        pot.Clear();
    }

    public bool Equals(Player? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Player)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
}