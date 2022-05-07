using System.Collections;
using System.Collections.Immutable;

using Microsoft.AspNetCore.SignalR;

using Sabacc.Hubs;

namespace Sabacc.Domain;

public enum Phase
{
    One,
    Two,
    Three
}

public class Scorer
{
    public Player ComputeWinner(List<Player> players)
    {
        throw new NotImplementedException();
    }
}

public class CorellianSpikeBlackSpireOutpostRules : ISabaccSession
{
    private readonly IHubContext<PlayerNotificationHub> _playerNotifier;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Player? CurrentPlayer => Players.CurrentTurn?.ValueRef;
    public Player? CurrentDealer => Players.CurrentDealer?.ValueRef;

    public int Round { get; set; } = 1;
    public int Slots { get; private set; }
    public SabaccVariantType VariantType => SabaccVariantType.CorellianSpikeBlackSpireOutpostRules;
    public Guid Id { get; } = Guid.NewGuid();
    public IImmutableList<Guid> PlayerIds => Players.Select(p => p.Id).ToImmutableList();
    public PlayersCircularList Players { get; set; } = new();
    public Deck MainDeck { get; set; } = new();
    public Deck DiscardPile { get; set; } = new();
    public Pot HandPot { get; set; } = new(PotType.TheHand);
    public Pot SabaccPot { get; set; } = new(PotType.TrueSabacc);
    public Dice SpikeDice { get; set; } = new();
    public SessionStatus Status { get; private set; } = SessionStatus.Open;

    public CorellianSpikeBlackSpireOutpostRules(IHubContext<PlayerNotificationHub> playerNotifier)
    {
        _playerNotifier = playerNotifier;
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


        player.State.MyTurn = player.Equals(CurrentPlayer);

        List<PlayerState> states = Players.Select(p => p.State).ToList();

        player.State.Phase = player.State.Phase switch
        {
            Phase.One => states.All(p => p.PhaseOne.Completed) ? Phase.Two : Phase.One,
            Phase.Two => states.All(p => p.PhaseTwo.Completed) ? Phase.Three : Phase.Two,
            Phase.Three => states.All(p => p.PhaseThree.Completed) ? Phase.One : Phase.Three
        };

        if (player.State.Phase == Phase.Three)
        {
            player.State.MyTurn = true; // No particular order for Acknowledge Loss/Win Actions
        }

        if (player.State.Phase == Phase.Two)
        {
            player.State.PhaseTwo.NoBets = !states.Any(s => s.PhaseTwo.Choice is PhaseTwoChoice.Bet or PhaseTwoChoice.Raise);
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

            var view = new PlayerViewModel()
            {
                Decks = GetDeckViews(),
                CurrentPlayer = Players.CurrentTurn?.ValueRef,
                CurrentDealer = Players.CurrentDealer?.ValueRef,
                SpikeDice = SpikeDice,
                Me = new Me()
                {
                    Player = me,
                    State = GetNextState(me)
                },
                Players = players,
                Pots = new List<Pot>(new[] { HandPot, SabaccPot }),
                Round = Round
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

    public async Task PlayerTurn(Guid playerId, PlayerState action)
    {
        Player player = Players.Single(p => p.Id == playerId);

        switch (action.Phase)
        {
            case Phase.One: HandlePhaseOne(player, action); break;
            case Phase.Two: HandlePhaseTwo(player, action); break;
            case Phase.Three: HandlePhaseThree(player, action); break;
        }

        await NotifyAsync().ConfigureAwait(false);
    }

    private void HandlePhaseThree(Player player, PlayerState action)
    {
        if (action.PhaseThree.Choice == PhaseThreeChoice.DealerRoll)
        {
            SpikeDice.Roll();

            player.State.PhaseThree.Choice = PhaseThreeChoice.DealerRoll;
            player.State.PhaseThree.Result = SpikeDice.Sides;
            player.State.PhaseThree.IsSabaccShift = SpikeDice.IsSabaccShift();

            if (SpikeDice.IsSabaccShift())
            {
                // Oh boyyyyyyyyyyyyyyy
            }
            else
            {
                // Calculate who won

            }
        }
        else if (action.PhaseThree.Choice == PhaseThreeChoice.ClaimWin)
        {
            // Player claim pot amounts
            // Clear appropriate Pots (Sabacc, Hand or Both)
            // Cards back to Main deck
        }
        else if (action.PhaseThree.Choice == PhaseThreeChoice.AcknowledgeLoss)
        {
            // Cards back to Main deck
        }

    }

    private void HandlePhaseOne(Player player, PlayerState action)
    {
        if (action.IsStand())
        {
            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Stand;

            SetNextTurn();
        }
        else if (action.IsSwap())
        {
            player.PlaceCredits(HandPot, 2);

            Card card = player.Hand.Find(f => f.Id == action.PhaseOne.SwapCardId)!;

            player.Hand.Remove(card);
            player.Hand.Add(DiscardPile.TakeTop());
            DiscardPile.Cards.Push(card);

            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Swap;

            SetNextTurn();
        }
        else if (action.IsGain1())
        {
            if (action.PhaseOne.Gain1ShouldDraw())
            {
                Card card = MainDeck.TakeTop();

                player.PlaceCredits(HandPot, credits: 1);
                player.Hand.Add(card);
                player.State.PhaseOne.Gain1DrawnCardId = card.Id;
            }
            else if (action.PhaseOne.Gain1DiscardCardId.HasValue)
            {
                Card card = player.Hand.Find(card => card.Id == action.PhaseOne.Gain1DiscardCardId)!;

                player.Hand.Remove(card);
                DiscardPile.Cards.Push(card);

                player.State.PhaseOne.Completed = true;
                player.State.PhaseOne.Choice = PhaseOneChoice.Gain1;

                SetNextTurn();
            }
            else if (action.PhaseOne.Gain1KeepCardId.HasValue)
            {
                player.State.PhaseOne.Completed = true;
                player.State.PhaseOne.Choice = PhaseOneChoice.Gain1;

                SetNextTurn();
            }
        }
        else if (action.IsGain2())
        {
            Card card = player.Hand.Find(card => card.Id == action.PhaseOne.Gain2Discard.Value)!;
            player.Hand.Remove(card);
            DiscardPile.Cards.Push(card);
            player.Hand.Add(MainDeck.TakeTop());
            player.State.PhaseOne.Completed = true;
            player.State.PhaseOne.Choice = PhaseOneChoice.Gain2;

            SetNextTurn();
        }
    }

    private void HandlePhaseTwo(Player player, PlayerState action)
    {
        if (action.PhaseTwo.Choice == PhaseTwoChoice.Check)
        {
            player.State.PhaseTwo.Choice = PhaseTwoChoice.Check;
            player.State.PhaseTwo.Completed = true;
            SetNextTurn();
        }
        else if (action.PhaseTwo.Choice == PhaseTwoChoice.Bet)
        {
            player.State.PhaseTwo.Choice = PhaseTwoChoice.Bet;
            player.State.PhaseTwo.Credits = action.PhaseTwo.Credits;
            player.State.PhaseTwo.Completed = true;
            player.PlaceCredits(HandPot, action.PhaseTwo.Credits);

            foreach (var other in Players.Where(p => !p.Equals(player)))
            {
                // Others MUST Call, Fold or Raise when someone bets
                other.State.PhaseTwo.Completed = false;
            }

            SetNextTurn();
        }
        else if (action.PhaseTwo.Choice == PhaseTwoChoice.Junk)
        {
            player.ShuffleHand();
            DiscardPile.AddCards(player.Hand);
            player.Hand.Clear();

            SetNextTurn();
        }
        else if (action.PhaseTwo.Choice == PhaseTwoChoice.Call)
        {
            // Slightly invalid
            var match = HandPot.Highest.Values.Max();

            player.State.PhaseTwo.Choice = PhaseTwoChoice.Call;
            player.State.PhaseTwo.Credits = match;
            player.State.PhaseTwo.Completed = true;
            player.PlaceCredits(HandPot, match);

            SetNextTurn();
        }
        else if (action.PhaseTwo.Choice == PhaseTwoChoice.Raise)
        {
            player.State.PhaseTwo.Choice = PhaseTwoChoice.Raise;
            player.State.PhaseTwo.Credits = action.PhaseTwo.Credits;
            player.State.PhaseTwo.Completed = true;
            player.PlaceCredits(HandPot, action.PhaseTwo.Credits);

            SetNextTurn();
        }
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
        Round = 1;
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

    public void AddAnte()
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
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Trangles", State = CardState.InDeck}),
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Squares", State = CardState.InDeck}),
            Enumerable.Range(1, 10).Select(x => new Card() {Value = x, Suit = "Circles", State = CardState.InDeck})
        };

        var negatives = new List<IEnumerable<Card>>()
        {
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Trangles", State = CardState.InDeck }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Squares", State = CardState.InDeck }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = -x, Suit = "Circles", State = CardState.InDeck })
        };

        MainDeck.AddCards(new Card() { Name = "Sylops", Value = 0 }, new Card() { Name = "Sylops", Value = 0 });
        MainDeck.AddCards(positives.SelectMany(cards => cards).ToArray());
        MainDeck.AddCards(negatives.SelectMany(cards => cards).ToArray());
        MainDeck.Shuffle();
    }
}
