namespace Sabacc.Domain;

public class Me
{
    public Guid Id { get; set; }
    public int Credits { get; set; }
    public List<Card> Hand { get; set; }
    public PlayerAction ActionState { get; set; }
    public bool MyTurn => ActionState.MyTurn;

    public Phase Phase
    {
        get
        {
            if (!ActionState.PhaseOne.Completed)
                return Phase.Choose;

            if (!ActionState.PhaseTwo.Completed)
                return Phase.Bet;

            if (ActionState.PhaseOne.Completed && ActionState.PhaseTwo.Completed)
                return Phase.SpikeDice;

            throw new InvalidOperationException("Unhandled action state for getting the Phase");
        }
    }
}