namespace Sabacc.Domain;

public class PlayerAction
{
    public Guid SessionId { get; set; }
    public Guid PlayerId { get; set; }
    public PhaseTwo PhaseTwo { get; set; }
    public PhaseOne PhaseOne { get; set; }
    public Phase Phase { get; set; }
    public bool MyTurn { get; set; }

    public PlayerAction()
    {
        PhaseOne = new PhaseOne();
        PhaseTwo = new PhaseTwo();
    }
}