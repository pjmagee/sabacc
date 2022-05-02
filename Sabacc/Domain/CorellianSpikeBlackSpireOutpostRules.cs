using System.Collections.Immutable;

using Microsoft.AspNetCore.SignalR;

using Sabacc.Hubs;

namespace Sabacc.Domain;

public enum Phase
{
    Choose,
    Bet,
    SpikeDice
}

public class CorellianSpikeBlackSpireOutpostRules : ISabaccSession
{
    private readonly IHubContext<PlayerNotificationHub> _playerNotifier;
    private readonly Dictionary<Guid, PlayerAction> _playerActionstates;
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
    public SessionStatus Status { get; private set; } = SessionStatus.Open;

    public CorellianSpikeBlackSpireOutpostRules(IHubContext<PlayerNotificationHub> playerNotifier)
    {
        _playerNotifier = playerNotifier;
        _playerActionstates = new Dictionary<Guid, PlayerAction>();
    }

    public void SetSlots(int slots)
    {
        if (PlayerIds.Count > slots || Status != SessionStatus.Open)
            throw new NotSupportedException();

        Slots = slots;
    }

    public void JoinSession(Guid playerId)
    {
        if (Status != SessionStatus.Open || PlayerIds.Contains(playerId))
            throw new InvalidOperationException("You cannot join the session.");

        Players.Join(playerId);

        if (Players.Count == Slots)
            Start();

        _playerActionstates[playerId] = new PlayerAction { PlayerId = playerId, SessionId = Id };
        NotifyAsync();
    }

    private async Task NotifyAsync() => _playerNotifier.Clients.All.SendAsync(PlayerNotificationHub.Method, Id);

    public void LeaveSession(Guid playerId)
    {
        Players.Leave(playerId);
        _playerActionstates.Remove(playerId);
        NotifyAsync();
    }

    private PlayerAction GetActionState(Guid playerId)
    {
        var actionState = _playerActionstates[playerId];
        actionState.MyTurn = CurrentPlayer?.Id.Equals(playerId) == true;
        actionState.Phase = actionState.Phase switch
        {
            Phase.Choose => _playerActionstates.Values.All(player => player.PhaseOne.Completed) ? Phase.Bet : Phase.Choose,
            Phase.Bet => _playerActionstates.Values.All(player => player.PhaseTwo.Completed) ? Phase.SpikeDice : Phase.Bet,
            Phase.SpikeDice => Phase.Choose // TODO
        };

        return actionState;
    }

    public PlayerViewModel GetPlayerView(Guid playerId)
    {
        try
        {
            _semaphore.Wait();

            var player = Players.Find(Players.Single(p => p.Id == playerId))!;

            var me = new Me
            {
                Id = player.Value.Id,
                Credits = player.Value.Credits,
                Hand = player.Value.Hand,
                ActionState = GetActionState(playerId)
            };

            List<HiddenPlayer> hiddenPlayers = new List<HiddenPlayer>();

            foreach (var hiddenPlayer in Players)
            {
                hiddenPlayers.Add(new HiddenPlayer
                {
                    Id = hiddenPlayer.Id,
                    Credits = hiddenPlayer.Credits,
                    Cards = hiddenPlayer.Hand.Count,
                    IsTurn = CurrentPlayer == hiddenPlayer,
                    IsDealer = CurrentDealer == hiddenPlayer
                });
            }

            var view = new PlayerViewModel()
            {
                Decks = new List<DeckView>(new[]
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
                }),
                PlayersTurn = Players.CurrentTurn?.Value.Id,
                CurrentDealer = Players.CurrentDealer?.Value.Id,
                Me = me,
                Players = hiddenPlayers,
                Pots = new List<Pot>(new[] { HandPot, SabaccPot })
            };

            return view;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public SpectatorView GetSpectateView()
    {
        return new SpectatorView();
    }

    public void PlayerTurn(PlayerAction playerAction)
    {
        if (playerAction.Phase == Phase.Choose)
        {
            HandlePhaseOne(playerAction);
        }
        else if (playerAction.Phase == Phase.Bet)
        {
            HandlePhaseTwo(playerAction);
        }
        else if (playerAction.Phase == Phase.SpikeDice)
        {

        }

        NotifyAsync();
    }

    private void HandlePhaseOne(PlayerAction playerAction)
    {
        if (playerAction.IsStand())
        {
            _playerActionstates[playerAction.PlayerId].PhaseOne.Completed = true;

            NextTurn();
        }
        else if (playerAction.IsSwap())
        {
            CurrentPlayer!.Ante(HandPot, 2);
            Card card = CurrentPlayer.Hand.Find(f => f.Id == playerAction.PhaseOne.SwapCardId)!;
            CurrentPlayer.Hand.Remove(card);
            CurrentPlayer.Hand.Add(DiscardPile.TakeTop());
            DiscardPile.Cards.Push(card);

            _playerActionstates[playerAction.PlayerId].PhaseOne.Completed = true;

            NextTurn();
        }
        else if (playerAction.IsGain1())
        {
            if (playerAction.PhaseOne.Gain1ShouldDraw())
            {
                CurrentPlayer!.Ante(HandPot, credits: 1);
                Card card = MainDeck.TakeTop();
                _playerActionstates[playerAction.PlayerId].PhaseOne.Gain1DrawnCardId = card.Id;
                CurrentPlayer.Hand.Add(card);
            }
            else if (playerAction.PhaseOne.Gain1DiscardCardId.HasValue)
            {
                Card card = CurrentPlayer!.Hand.Find(card => card.Id == playerAction.PhaseOne.Gain1DiscardCardId)!;
                CurrentPlayer.Hand.Remove(card);
                DiscardPile.Cards.Push(card);
                _playerActionstates[playerAction.PlayerId].PhaseOne.Completed = true;

                NextTurn();
            }
            else if (playerAction.PhaseOne.Gain1KeepCardId.HasValue)
            {
                _playerActionstates[playerAction.PlayerId].PhaseOne.Completed = true;

                NextTurn();
            }
        }
        else if (playerAction.IsGain2())
        {
            Card card = CurrentPlayer!.Hand.Find(card => card.Id == playerAction.PhaseOne.Gain2Discard.Value)!;
            CurrentPlayer.Hand.Remove(card);
            DiscardPile.Cards.Push(card);
            CurrentPlayer.Hand.Add(MainDeck.TakeTop());
            _playerActionstates[playerAction.PlayerId].PhaseOne.Completed = true;
            _playerActionstates[playerAction.PlayerId].PhaseOne.Choice = PhaseOneChoice.Gain2;
            NextTurn();
        }
    }

    private void HandlePhaseTwo(PlayerAction playerAction)
    {
        // This can only be done if no bet has been made yet.
        // If a bet is made (all must call or raise)
        if (playerAction.PhaseTwo.Check)
        {

        }
        // The player adds a bet to the Hand pot
        // All other players must add the same amount OR (Raise the but or Junk out)

        else if (playerAction.PhaseTwo.Bet)
        {

        }
        else if (playerAction.PhaseTwo.Junk)
        {

        }
        else if (playerAction.PhaseTwo.Call)
        {

        }
        else if (playerAction.PhaseTwo.Raise)
        {

        }

    }

    private void NextTurn()
    {
        Players.ShiftTurnToNext();
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
            player.ValueRef.Hand.Add(MainDeck.TakeTop());
            player.ValueRef.Hand.Add(MainDeck.TakeTop());

            player = player.Next! ?? Players.First!;

            if (player.ValueRef == CurrentPlayer)
                break;
        }

        DiscardPile.AddCards(MainDeck.TakeTop());
    }

    public void AddAnte()
    {
        foreach (var player in Players)
        {
            player.Ante(HandPot, 1);
            player.Ante(SabaccPot, 2);
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
