namespace Sabacc.Domain;

public class PhaseThree
{
    public PhaseThreeChoice? Choice { get; set; }
    public DieSides[]? DiceRolled { get; set; }
    public bool Completed { get; set; }
    public bool WonRound { get; set; }
    public bool WonSabacc => WinningHand != null && WinningHand != HandType.Nulrhek;

    public HandType? WinningHand { get; set; }

    public void Reset()
    {
        DiceRolled = null;
        Completed = false;
        WonRound = false;
        Choice = null;
    }
}