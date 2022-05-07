namespace Sabacc.Domain;

public class PlayerPublicView
{
    public Player Player { get; set; }
    public bool IsTurn { get; set; }
    public bool IsDealer { get; set; }
    public PhaseOne PhaseOne => Player.State.PhaseOne;
    public PhaseTwo PhaseTwo => Player.State.PhaseTwo;
    public PhaseThree PhaseThree => Player.State.PhaseThree;
}