using System.Collections.Immutable;

using Microsoft.AspNetCore.SignalR;

using Sabacc.Hubs;

namespace Sabacc.Domain;

public class CorellianSpikeBlackSpireOutpostRules : ISabaccSession
{
    private readonly IHubContext<PlayerNotificationHub> _playerNotifier;
    private readonly IWinnerCalculator _winnerCalculator;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Player? CurrentPlayer => Players.CurrentTurn?.ValueRef;
    public Player? CurrentDealer => Players.CurrentDealer?.ValueRef;

    public Round Round { get; set; } = Round.One;

    public Phase Phase
    {
        get
        {
            if (Players.Any(p => p.PhaseOneCompleted() == false)) return Phase.One;
            if (Players.Any(p => p.PhaseTwoCompleted() == false)) return Phase.Two;
            if (Players.Any(p => p.PhaseThreeCompleted() == false)) return Phase.Three;

            throw new InvalidOperationException("Unhandled State for Phase.");
        }
    }

    public int Slots { get; private set; }
    public SabaccVariantType VariantType => SabaccVariantType.CorellianSpikeBlackSpireOutpostRules;
    public Guid Id { get; } = Guid.NewGuid();
    public IImmutableList<Guid> PlayerIds => Players.Select(p => p.Id).ToImmutableList();
    public PlayersCircularList Players { get; set; } = new();
    public Deck MainDeck { get; set; } = new();
    public Deck DiscardPile { get; set; } = new();
    public Pot HandPot { get; set; } = new(PotType.TheHand);
    public Pot SabaccPot { get; set; } = new(PotType.TrueSabacc);
    public Dice Dice { get; set; } = new();
    public SessionStatus Status { get; private set; } = SessionStatus.Open;

    public CorellianSpikeBlackSpireOutpostRules(IHubContext<PlayerNotificationHub> playerNotifier, IWinnerCalculator winnerCalculator)
    {
        _playerNotifier = playerNotifier;
        _winnerCalculator = winnerCalculator;
    }

    public void SetSlots(int slots)
    {
        if (PlayerIds.Count > slots || Status != SessionStatus.Open)
            throw new NotSupportedException();

        Slots = slots;
    }

    public async Task JoinSession(Guid id, string name)
    {
        if (Status != SessionStatus.Open || PlayerIds.Contains(id))
            throw new InvalidOperationException("You cannot join the session.");

        Players.Join(id, name);

        if (Players.Count == Slots)
            Start();

        await NotifyAsync().ConfigureAwait(false);
    }

    private async Task NotifyAsync() => await _playerNotifier.Clients.All.SendAsync(PlayerNotificationHub.Method, Id);

    public async Task LeaveSession(Guid playerId)
    {
        var player = Players.SingleOrDefault(p => p.Id == playerId);

        if (player is not null)
            Players.Leave(playerId);

        await NotifyAsync().ConfigureAwait(false);
    }

    private PlayerState GetNextState(Player player)
    {
        player.State.CanPerformAction = player.Equals(CurrentPlayer);

        if (Phase == Phase.One)
        {
            // Nothing specific
        }
        else if (Phase == Phase.Two)
        {
            List<PlayerState> states = Players.Select(p => p.State).ToList();
            player.State.PhaseTwo.NoBets = !states.Any(s => s.PhaseTwo.Choice is PhaseTwoChoice.Bet or PhaseTwoChoice.Raise);
        }
        else if (Phase == Phase.Three)
        {
            player.State.CanPerformAction = !player.State.PhaseThree.Completed;
        }

        return player.State;
    }

    public PlayerViewModel GetPlayerView(Guid playerId)
    {
        try
        {
            _semaphore.Wait();

            Player me = Players.Single(p => p.Id == playerId);

            List<PlayerPublicView> players = new List<PlayerPublicView>();

            foreach (Player player in Players)
            {
                players.Add(new()
                {
                    Player = player,
                    IsTurn = player.Equals(CurrentPlayer),
                    IsDealer = player.Equals(CurrentDealer)
                });
            }

            PlayerViewModel view = new PlayerViewModel()
            {
                Decks = GetDeckViews(),
                CurrentPlayer = Players.CurrentTurn?.ValueRef,
                CurrentDealer = Players.CurrentDealer?.ValueRef,
                Dice = Dice,
                DiceRolled = Players.CurrentDealer?.ValueRef.State.PhaseThree.DiceRolled is not null,
                Me = new Me()
                {
                    Player = me,
                    State = GetNextState(me)
                },
                Players = players,
                Pots = new List<Pot>(new[] { HandPot, SabaccPot }),
                Round = Round,
                Phase = Phase
            };

            return view;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private List<DeckView> GetDeckViews()
    {
        return new List<DeckView>(new[]
        {
            new DeckView
            {
                Name = "Main deck",
                TopCard = null,
                Total = MainDeck.Cards.Count,
                DeckType = DeckType.Draw
            },
            new DeckView
            {
                Name = "Discard Pile",
                TopCard = DiscardPile.ViewTop()?.ToString() ?? "None",
                Total = DiscardPile.Cards.Count,
                DeckType = DeckType.Discard
            }
        });
    }

    public SpectatorView GetSpectateView()
    {
        return new SpectatorView();
    }

    public async Task PlayerTurn(Guid playerId, PlayerAction action)
    {
        Player player = Players.Single(p => p.Id == playerId);

        if (action.PhaseOne is not null) HandlePhaseOne(player, action);
        else if (action.PhaseTwo is not null) HandlePhaseTwo(player, action);
        else if (action.PhaseThree is not null) HandlePhaseThree(player, action);

        await NotifyAsync().ConfigureAwait(false);
    }

    private void HandlePhaseThree(Player player, PlayerAction action)
    {
        if (action.IsDiceRoll()) HandleDiceRoll(player);
        else if (action.IsClaim()) HandleClaim(player);
        else if (action.IsReady()) HandleReady(player);

        if (IsSabaccShift)
        {
            StartSabaccShift();
        }
        else if (IsShowdown)
        {
            CalculateWinner();
        }
        else
        {
            StartNextRound();
        }
    }

    public bool IsSabaccShift => Dice.IsSabaccShift() && Players.WithoutJunked().All(p => p.PhaseThreeCompleted());

    private void HandleReady(Player player)
    {
        player.State.PhaseThree.Choice = PhaseThreeChoice.Ready;
        player.State.PhaseThree.Completed = true;
    }

    private void HandleClaim(Player player)
    {
        player.State.PhaseThree.Choice = PhaseThreeChoice.Claim;
        player.TakeCredits(HandPot);

        if (player.State.PhaseThree.WonSabacc)
        {
            player.TakeCredits(SabaccPot);
        }

        player.State.PhaseThree.Completed = true;
    }

    private void HandleDiceRoll(Player player)
    {
        Dice.Roll();

        player.State.PhaseThree.Choice = PhaseThreeChoice.DiceRoll;
        player.State.PhaseThree.DiceRolled = Dice.Sides!.Select(x => x).ToArray();
    }

    private bool IsShowdown => !Dice.IsSabaccShift() &&
                               Round == Round.Three &&
                               Players.WithoutJunked().Any(p => !p.State.PhaseThree.Completed);

    private void StartNextRound()
    {
        Round = Round switch { Round.One => Round.Two, Round.Two => Round.Three, Round.Three => Round.One };
        Players.ShiftDealerToNext();
        Players.ResetPhaseCompletions(forNextRound: true);
        AddAnte();
    }

    private void StartSabaccShift()
    {
        Players.ResetPhaseCompletions(forNextRound: false);

        Dictionary<Player, int> playerCardsCount = Players.WithoutJunked().ToDictionary(player => player, player => player.Hand.Count);

        foreach (var player in Players.WithoutJunked())
        {
            player.ShuffleHand();
            DiscardPile.AddCards(player.Hand);
            player.Hand.Clear();

            while (player.Hand.Count != playerCardsCount[player])
            {
                player.Hand.Add(TakeTop());
            }

            // Then place the rest of the deck face down on the table
            // Flip over the top card from the main deck to the discard pile
            // to start it with a new random value
            DiscardPile.AddCards(MainDeck.TakeTop());
        }
    }

    private Card TakeTop()
    {
        if (MainDeck.ViewTop() is null)
        {
            MainDeck.AddCards(DiscardPile.TakeTop(DiscardPile.Cards.Count));
            MainDeck.Shuffle();
            DiscardPile.AddCards(MainDeck.TakeTop());
        }

        return MainDeck.TakeTop(1).First();
    }

    private void CalculateWinner() => _winnerCalculator.Calculate(Players);

    private void HandlePhaseOne(Player player, PlayerAction action)
    {
        if (action.IsStand())
        {
            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Stand;
        }
        else if (action.IsSwap())
        {
            player.PlaceCredits(HandPot, 2);

            Card card = player.Hand.Find(f => f.Id == action.PhaseOne!.SwapCardId)!;

            player.Hand.Remove(card);
            player.Hand.Add(DiscardPile.TakeTop());
            DiscardPile.Cards.Push(card);

            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Swap;
        }
        else if (action.IsGain1())
        {
            if (action.PhaseOne!.Gain1ShouldDraw())
            {
                Card card = TakeTop();
                player.PlaceCredits(HandPot, credits: 1);
                player.Hand.Add(card);
                player.State.PhaseOne.Gain1DrawnCardId = card.Id;
                return;
            }

            if (action.PhaseOne.Gain1DiscardCardId.HasValue)
            {
                Card card = player.Hand.Find(card => card.Id == action.PhaseOne.Gain1DiscardCardId)!;
                player.Hand.Remove(card);
                DiscardPile.Cards.Push(card);
                player.State.PhaseOne.Completed = true;
                player.State.PhaseOne.Choice = PhaseOneChoice.Gain1;
            }
            else if (action.PhaseOne.Gain1KeepCardId.HasValue)
            {
                player.State.PhaseOne.Completed = true;
                player.State.PhaseOne.Choice = PhaseOneChoice.Gain1;
            }
        }
        else if (action.IsGain2())
        {
            Card card = player.Hand.Find(card => card.Id == action.PhaseOne.Gain2Discard.Value)!;
            player.Hand.Remove(card);
            DiscardPile.Cards.Push(card);
            player.Hand.Add(TakeTop());
            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Gain2;
        }

        SetNextTurn();
    }

    private void HandlePhaseTwo(Player player, PlayerAction action)
    {
        switch (action.PhaseTwo!.Choice)
        {
            case PhaseTwoChoice.Check:
                HandleCheck(player);
                break;
            case PhaseTwoChoice.Bet:
                HandleBet(player, action.PhaseTwo.Credits);
                break;
            case PhaseTwoChoice.Junk:
                HandleJunk(player);
                break;
            case PhaseTwoChoice.Call:
                HandleCall(player);
                break;
            case PhaseTwoChoice.Raise:
                HandleRaise(player, action.PhaseTwo.Credits);
                break;
        }

        SetNextTurn();
    }

    private void HandleCheck(Player player)
    {
        player.State.PhaseTwo.Choice = PhaseTwoChoice.Check;
        player.State.PhaseTwo.Completed = true;
    }

    private void HandleBet(Player player, int credits)
    {
        player.State.PhaseTwo.Choice = PhaseTwoChoice.Bet;
        player.State.PhaseTwo.Credits = credits;
        player.State.PhaseTwo.Completed = true;
        player.PlaceCredits(HandPot, credits);

        foreach (var other in Players.WithoutJunked().Where(p => !p.Equals(player)))
        {
            other.State.PhaseTwo.Completed = false;
        }
    }

    private void HandleJunk(Player player)
    {
        player.ShuffleHand();
        DiscardPile.AddCards(player.Hand);
        player.Hand.Clear();
        player.State.JunkedOut = true;
    }

    private void HandleRaise(Player player, int credits)
    {
        player.State.PhaseTwo.Choice = PhaseTwoChoice.Raise;
        player.State.PhaseTwo.Credits = credits;
        player.State.PhaseTwo.Completed = true;
        player.PlaceCredits(HandPot, credits);

        foreach (var other in Players.WithoutJunked().Where(p => !p.Equals(player)))
        {
            other.State.PhaseTwo.Completed = false;
        }
    }

    private void HandleCall(Player player)
    {
        int match = HandPot.Highest.Values.Max();

        player.State.PhaseTwo.Choice = PhaseTwoChoice.Call;
        player.State.PhaseTwo.Credits = match;
        player.State.PhaseTwo.Completed = true;
        player.PlaceCredits(HandPot, match);
    }

    private void SetNextTurn()
    {
        if (Players.InPhase1())
        {
            Players.ShiftTurnToNext();
        }

        if (Players.InPhase2())
        {
            Players.ShiftTurnToNext();
        }

        if (Players.InPhase3())
        {
            // Dealer rolls the SpikeDice
        }
    }

    private void Start()
    {
        Status = SessionStatus.Started;
        CreateDeck();
        Players.SetFirstRoundDealer();
        Players.SetFirstRoundTurn();
        AddAnte();
        DealCards();
        Round = Round.One;
    }

    private void DealCards()
    {
        LinkedListNode<Player> player = Players.CurrentDealer.Next!;

        while (true)
        {
            player.ValueRef.Hand.AddRange(MainDeck.TakeTop(2));

            player = player.Next! ?? Players.First!;

            if (player.Value.Equals(CurrentPlayer))
                break;
        }

        DiscardPile.AddCards(MainDeck.TakeTop());
    }

    private void AddAnte()
    {
        foreach (var player in Players)
        {
            player.PlaceCredits(HandPot, 1);
            player.PlaceCredits(SabaccPot, 2);
        }
    }

    private void CreateDeck()
    {
        var positives = new List<IEnumerable<Card>>()
        {
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Trangles" }),
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Squares" }),
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Circles" })
        };

        var negatives = new List<IEnumerable<Card>>()
        {
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Trangles" }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Squares" }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Circles" })
        };

        MainDeck.AddCards(new Card() { Name = "Sylops", Value = 0 }, new Card() { Name = "Sylops", Value = 0 });
        MainDeck.AddCards(positives.SelectMany(cards => cards).ToArray());
        MainDeck.AddCards(negatives.SelectMany(cards => cards).ToArray());
        MainDeck.Shuffle();
    }
}
