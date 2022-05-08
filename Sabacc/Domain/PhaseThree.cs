namespace Sabacc.Domain;

public class PhaseThree
{
    public PhaseThreeChoice? Choice { get; set; }
    public DieSides[]? Result { get; set; }
    public bool IsSabaccShift { get; set; }
    public bool Completed { get; set; }
    public bool WonRound { get; set; }
    public bool WonSabacc => WinningHand != null && WinningHand != HandType.Nulrhek;

    public HandType? WinningHand { get; set; }
}