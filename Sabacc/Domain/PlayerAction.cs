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

    public void Validate()
    {

    }

    public bool IsSwap()
    {
        return PhaseOne.Choice == PhaseOneChoice.Swap && PhaseOne.SwapCardId.HasValue;
    }
    public bool IsStand()
    {
        return PhaseOne.Choice == PhaseOneChoice.Stand;
    }

    public bool IsGain1()
    {
        return PhaseOne.Choice == PhaseOneChoice.Gain1;
    }

    public bool IsGain2()
    {
        return PhaseOne.Choice == PhaseOneChoice.Gain2 && PhaseOne.Gain2Discard.HasValue;
    }
}