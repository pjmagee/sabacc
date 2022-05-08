namespace Sabacc.Domain;

public class PhaseOne
{
    public PhaseOneChoice? Choice { get; set; } = null!;

    public bool Completed { get; set; }
    public Guid? SwapCardId { get; set; }
    public Guid? Gain1DrawnCardId { get; set; }
    public Guid? Gain1KeepCardId { get; set; }
    public Guid? Gain1DiscardCardId { get; set; }
    public Guid? Gain2Discard { get; set; }

    public PhaseOne(PhaseOneChoice choice)
    {
        Choice = choice;
    }

    public PhaseOne()
    {

    }

    public bool Gain1ShouldDraw() => !Gain1DrawnCardId.HasValue &&
                                     !Gain1KeepCardId.HasValue &&
                                     !Gain1DiscardCardId.HasValue;
}

public enum PhaseOneChoice
{
    Stand,
    Gain1,
    Gain2,
    Swap
}