namespace Sabacc.Domain;

public class Player
{
    public Guid Id { get; set; }

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
}