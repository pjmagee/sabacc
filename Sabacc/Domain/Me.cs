namespace Sabacc.Domain;

public class Me
{
    public Player Player { get; set; }
    public int Credits => Player.Credits;
    public List<Card> Hand => Player.Hand;
    public PlayerState State { get; set; }
    public bool MyTurn => State.MyTurn;
    public Phase Phase => State.Phase;
}

public static class Extensions
{
    public static string ToDisplayName(this Phase phase)
    {
        return phase switch
        {
            Phase.One => "Phase 1 (dealt cards)",
            Phase.Two => "Phase 2 (betting)",
            Phase.Three => "Phase 3 (Spike dice)"
        };
    }
}