namespace Sabacc.Domain;

public class PhaseTwo
{
    public bool Completed { get; set; }
    public bool Call { get; set; }
    public bool Raise { get; set; }
    public bool Junk { get; set; }
    public bool Check { get; set; }
    public bool Bet { get; set; }

    public int Credits { get; set; }
}