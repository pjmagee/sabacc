namespace Sabacc.Domain;

public class PhaseTwo
{
    public PhaseTwoChoice? Choice { get; set; }
    public bool Completed { get; set; }
    public int Credits { get; set; }
    public bool NoBets { get; set; }
}