namespace Sabacc.Domain;

public class Player : IEquatable<Player>
{
    public Guid Id { get; }

    public int Credits { get; set; } = 1500;

    public List<Card> Hand { get; }

    public Player(Guid id)
    {
        Id = id;
        Hand = new List<Card>();
    }

    public void Ante(Pot pot, int credits)
    {
        Credits -= credits;

        if (pot.Contributions.ContainsKey(Id))
        {
            pot.Contributions[Id] += credits;
        }
        else
        {
            pot.Contributions.Add(Id, credits);
        }
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
        return Equals((Player) obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
}