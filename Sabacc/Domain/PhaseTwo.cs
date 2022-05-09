namespace Sabacc.Domain;

public class PhaseTwo
{
    public PhaseTwoChoice? Choice { get; set; }
    public bool Completed { get; set; }
    public int Credits { get; set; }
    public bool NoBets { get; set; }

    public void Reset()
    {
        Completed = false;
        Choice = null;
        Credits = 0;
        NoBets = false;
    }
}