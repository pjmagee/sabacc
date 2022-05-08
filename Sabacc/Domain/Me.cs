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