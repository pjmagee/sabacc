using System.Reflection;

namespace Sabacc.Domain;

public class PlayerViewModel
{
    public List<DeckView> Decks { get; init; }
    public List<PlayerPublicView> Players { get; init; } = new();
    public List<Pot> Pots { get; init; } = new();
    public Round Round { get; init; }
    public Me Me { get; init; }
    public Player? CurrentPlayer { get; init; }
    public Player? CurrentDealer { get; init; }
    public Dice Dice { get; init; }
    public bool DiceRolled { get; init; }
    public Phase Phase { get; set; }
    public PlayerPublicView? Winner => Players.FirstOrDefault(p => p.Player.State.PhaseThree?.WonRound == true);

    public bool IsMePhaseOne() => Me.MyTurn && Phase == Phase.One;

    public bool IsMePhaseTwo() => Me.MyTurn && Phase == Phase.Two;

    public bool IsMePhaseThree() => Me.MyTurn && Phase == Phase.Three && DiceRolled && !Me.Player.State.PhaseThree.Completed;


    public bool IsShowdown()
    {
        return Me.MyTurn && Round == Round.Three && Phase == Phase.Three && DiceRolled;
    }

    public bool CanDiceRoll()
    {
        return Me.Player.Equals(CurrentDealer) && Phase == Phase.Three && Me.Player.State.PhaseThree.DiceRolled is null;
    }

    public bool CanCheck()
    {
        return Me.MyTurn && Phase == Phase.Two && Me.State.PhaseTwo.NoBets;
    }

    public bool CanGainOption1()
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed && // Not completed
               Phase == Phase.One && // Is choosing phase
               Me.State.PhaseOne.Choice != PhaseOneChoice.Gain2 && // Has not picked Gain2 option
               !Me.State.PhaseOne.SwapCardId.HasValue && // Swap card doesnt have a value
               !Me.State.PhaseOne.Gain1DrawnCardId.HasValue; // Has no gain1 drawn card
    }

    public bool CanSwap()
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed &&
               Phase == Phase.One &&
               !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanGainOption2()
    {
        return Me.MyTurn && !Me.State.PhaseOne.Completed && !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanKeepOrDiscard(Card card)
    {
        return Me.MyTurn &&
               !Me.State.PhaseOne.Completed &&
               card.Id == Me.State.PhaseOne.Gain1DrawnCardId;
    }

    public bool CanStand() => Me.MyTurn && Phase == Phase.One &&
                              !Me.State.PhaseOne.Completed &&
                              !Me.State.PhaseOne.Gain1DrawnCardId.HasValue;

    public bool CanCall()
    {
        return Me.MyTurn && Phase == Phase.Two && Me.State.PhaseTwo.NoBets == false;
    }

    public bool CanRaise()
    {
        // TODO: has the credits and not reached max raise limit?
        return Me.MyTurn && Phase == Phase.Two &&
               Me.State.PhaseTwo.NoBets == false;
    }

    public bool CanBet()
    {
        return Me.MyTurn && Phase == Phase.Two && Me.State.PhaseTwo.NoBets;
    }

    public bool CanJunk()
    {
        return Me.MyTurn && Phase == Phase.Two && !Me.State.JunkedOut;
    }

    public bool IsWinner()
    {
        return IsPhase3Pending() && Me.State.PhaseThree.WonRound;
    }

    public bool IsLoser()
    {
        return IsPhase3Pending() && !Me.State.PhaseThree.WonRound && DiceRolled && !Dice.IsSabaccShift();
    }

    public bool IsSabaccShift()
    {
        return IsPhase3Pending() && DiceRolled && Dice.IsSabaccShift();
    }

    public bool IsSabaccWinner()
    {
        return IsPhase3Pending() && Me.State.PhaseThree.WonRound && Me.State.PhaseThree.WonSabacc;
    }

    private bool IsPhase3Pending()
    {
        return Phase == Phase.Three && !Me.State.PhaseThree.Completed;
    }
}