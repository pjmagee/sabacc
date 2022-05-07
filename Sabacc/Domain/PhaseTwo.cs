namespace Sabacc.Domain;

public class PhaseTwo
{
    public PhaseTwoChoice? Choice { get; set; }
    public bool Completed { get; set; }
    public int Credits { get; set; }
    public bool NoBets { get; set; }
}

public enum PhaseTwoChoice
{
    Check = 0,
    Bet = 1,
    Raise = 2,
    Junk = 3,
    Call = 4
}