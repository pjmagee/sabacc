namespace Sabacc.Domain;

public class PlayerViewModel
{
    public List<DeckView> Decks { get; set; }
    public List<PlayerPublicView> Players { get; set; } = new();
    public List<Pot> Pots { get; set; } = new();
    public int Round { get; set; }

    public Me Me { get; set; }

    public Player? CurrentPlayer { get; set; }
    public Player? CurrentDealer { get; set; }
    public Dice Dice { get; set; }
    public bool DiceRolled { get; set; }

    public bool IsMePhaseOne() => Me.MyTurn && Me.Phase == Phase.One;

    public bool IsMePhaseTwo() => Me.MyTurn && Me.Phase == Phase.Two;

    public bool IsMePhaseThree() => Me.MyTurn && Me.Phase == Phase.Three && DiceRolled && !Me.Player.State.PhaseThree.Completed;

    public bool CanDiceRoll()
    {
        return Me.Player.Equals(CurrentDealer) &&
               Me.Phase == Phase.Three &&
               Me.Player.State.PhaseThree.Result is null;
    }

    public bool CanCheck()
    {
        return Me.MyTurn &&
               Me.State.Phase == Phase.Two &&
               Me.State.PhaseTwo.NoBets;
    }

    public bool CanGainOption1()
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed && // Not completed
               Me.State.Phase == Phase.One && // Is choosing phase
               Me.State.PhaseOne.Choice != PhaseOneChoice.Gain2 && // Has not picked Gain2 option
               !Me.State.PhaseOne.SwapCardId.HasValue && // Swap card doesnt have a value
               !Me.State.PhaseOne.Gain1DrawnCardId.HasValue; // Has no gain1 drawn card
    }

    public bool CanSwap()
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed &&
               Me.State.Phase == Phase.One &&
               !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanGainOption2()
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed &&
               !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanKeepOrDiscard(Card card)
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed &&
               card.Id == Me.State.PhaseOne.Gain1DrawnCardId;
    }

    public bool CanStand() => Me.MyTurn && Me.State.Phase == Phase.One &&
                              !Me.State.PhaseOne.Completed &&
                              !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;

    public bool CanCall()
    {
        return Me.MyTurn && Me.State.Phase == Phase.Two && Me.State.PhaseTwo.NoBets == false;
    }

    public bool CanRaise()
    {
        // TODO: has the credits and not reached max raise limit?
        return Me.MyTurn && Me.State.Phase == Phase.Two &&
               Me.State.PhaseTwo.NoBets == false;
    }

    public bool CanBet()
    {
        return Me.MyTurn && Me.State.Phase == Phase.Two && Me.State.PhaseTwo.NoBets;
    }

    public bool CanJunk()
    {
        // TODO:
        return Me.MyTurn && Me.State.Phase == Phase.Two;
    }

    public bool CanClaimHandPot()
    {
        return IsPhase3Pending() && Me.State.PhaseThree.WonRound;
    }

    public bool CanAcknowledgeLoss()
    {
        return IsPhase3Pending() && !Me.State.PhaseThree.WonRound && DiceRolled;
    }

    public bool CanClaimSabaccPot()
    {
        return IsPhase3Pending() && Me.State.PhaseThree.WonRound && Me.State.PhaseThree.WonSabacc;
    }

    private bool IsPhase3Pending()
    {
        return Me.Phase == Phase.Three && !Me.State.PhaseThree.Completed;
    }
}