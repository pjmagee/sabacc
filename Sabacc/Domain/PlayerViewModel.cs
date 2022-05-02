namespace Sabacc.Domain;

public class PlayerViewModel
{
    public List<DeckView> Decks { get; set; }
    public Me Me { get; set; }
    public List<HiddenPlayer> Players { get; set; } = new();
    public List<Pot> Pots { get; set; } = new();
    public int Rounds { get; set; }
    public Guid? PlayersTurn { get; set; }
    public Guid? CurrentDealer { get; set; }

    public bool CanGainOption1()
    {
        return Me.MyTurn &&
               !Me.ActionState.PhaseOne.Completed && // Not completed
               Me.ActionState.Phase == Phase.Choose && // Is choosing phase
               Me.ActionState.PhaseOne.Choice != PhaseOneChoice.Gain2 && // Has not picked Gain2 option
               !Me.ActionState.PhaseOne.SwapCardId.HasValue && // Swap card doesnt have a value
               !Me.ActionState.PhaseOne.Gain1DrawnCardId.HasValue; // Has no gain1 drawn card
    }

    public bool CanSwap()
    {
        return Me.MyTurn &&
               !Me.ActionState.PhaseOne.Completed &&
               Me.ActionState.Phase == Phase.Choose &&
               !Me.ActionState.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanGainOption2()
    {
        return Me.MyTurn &&
               !Me.ActionState.PhaseOne.Completed &&
               !Me.ActionState.PhaseOne.Gain1DrawnCardId.HasValue;
    }

    public bool CanKeepOrDiscard(Card card)
    {
        return Me.MyTurn &&
               !Me.ActionState.PhaseOne.Completed &&
               card.Id == Me.ActionState.PhaseOne.Gain1DrawnCardId;
    }

    public bool CanStand() => Me.MyTurn &&
                              !Me.ActionState.PhaseOne.Completed &&
                              !Me.ActionState.PhaseOne.Gain1DrawnCardId.HasValue;
}