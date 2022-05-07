namespace Sabacc.Domain;

public class PlayerState
{
    public PhaseOne PhaseOne { get; set; }
    public PhaseTwo PhaseTwo { get; set; }
    public PhaseThree PhaseThree { get; set; }

    public Phase Phase { get; set; }
    public bool MyTurn { get; set; }

    public PlayerState()
    {
        PhaseOne = new PhaseOne();
        PhaseTwo = new PhaseTwo();
        PhaseThree = new PhaseThree();
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