namespace Sabacc.Domain;

public class Pot
{
    public PotType PotType { get; }

    public Dictionary<Player, int> Highest { get; set; }

    public Dictionary<Player, int> Contributions { get; set; }

    public Pot(PotType potType)
    {
        PotType = potType;
        Contributions = new Dictionary<Player, int>();
        Highest = new Dictionary<Player, int>();
    }

    public void Clear()
    {
        new List<Player>(Contributions.Keys).ForEach(player => Contributions[player] = 0);
        new List<Player>(Highest.Keys).ForEach(player => Highest[player] = 0);
    }

    public void Add(Player player, int amount)
    {
        if (Highest.ContainsKey(player))
        {
            Highest[player] = Math.Max(Highest[player], amount);
        }
        else
        {
            Highest.Add(player, amount);
        }

        if (Contributions.ContainsKey(player))
        {
            Contributions[player] += amount;
        }
        else
        {
            Contributions.Add(player, amount);
        }
    }

    public int Total => Contributions.Values.Sum();
}