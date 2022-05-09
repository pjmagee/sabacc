namespace Sabacc.Domain;

public class PlayerAction
{
    public PhaseOne? PhaseOne { get; set; }
    public PhaseTwo? PhaseTwo { get; set; }
    public PhaseThree? PhaseThree { get; set; }

    public PlayerAction(PhaseOne phaseOne)
    {
        PhaseOne = phaseOne;
    }

    public PlayerAction(PhaseTwo phaseTwo)
    {
        PhaseTwo = phaseTwo;
    }

    public PlayerAction(PhaseThree phaseThree)
    {
        PhaseThree = phaseThree;
    }

    public PlayerAction()
    {

    }

    public bool IsDiceRoll() => PhaseThree?.Choice == PhaseThreeChoice.DiceRoll;
    public bool IsClaim() => PhaseThree?.Choice == PhaseThreeChoice.Claim;
    public bool IsReady() => PhaseThree?.Choice == PhaseThreeChoice.Ready;

    public bool IsSwap() => PhaseOne?.Choice == PhaseOneChoice.Swap && PhaseOne.SwapCardId.HasValue;
    public bool IsStand() => PhaseOne?.Choice == PhaseOneChoice.Stand;
    public bool IsGain1() => PhaseOne?.Choice == PhaseOneChoice.Gain1;
    public bool IsGain2() => PhaseOne?.Choice == PhaseOneChoice.Gain2 && PhaseOne.Gain2Discard.HasValue;
}

public class PlayerState
{
    public PhaseOne PhaseOne { get; set; }
    public PhaseTwo PhaseTwo { get; set; }
    public PhaseThree PhaseThree { get; set; }
    public bool JunkedOut { get; set; } = false;
    public bool CanPerformAction { get; set; } = false;

    public PlayerState()
    {
        PhaseOne = new PhaseOne();
        PhaseTwo = new PhaseTwo();
        PhaseThree = new PhaseThree();
    }
}