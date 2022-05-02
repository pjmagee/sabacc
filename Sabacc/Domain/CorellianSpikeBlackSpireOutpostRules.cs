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
    private readonly IHubContext<UpdateHub> _hubContext;
    private PlayerAction _currentPlayerState;

    public Phase Phase { get; private set; } = Phase.Choose;

    public Player? CurrentPlayer => Players.CurrentTurn?.ValueRef;
    public Player? CurrentDealer => Players.CurrentDealer?.ValueRef;

    public int Round { get; set; }

    public int Slots { get; private set; }

    public SabaccVariantType VariantType => SabaccVariantType.CorellianSpikeBlackSpireOutpostRules;

    public Guid Id { get; } = Guid.NewGuid();

    public IImmutableList<Guid> PlayerIds => Players.Select(p => p.Id).ToImmutableList();

    public PlayersCircularList Players { get; set; } = new PlayersCircularList();

    public Deck MainDeck { get; set; } = new Deck();

    public Deck DiscardPile { get; set; } = new Deck();

    public Pot HandPot { get; set; } = new Pot(PotType.TheHand);
    public Pot SabaccPot { get; set; } = new Pot(PotType.TrueSabacc);

    public SessionStatus Status { get; private set; } = SessionStatus.Open;

    public CorellianSpikeBlackSpireOutpostRules(IHubContext<UpdateHub> hubContext)
    {
        _hubContext = hubContext;
        _currentPlayerState = new PlayerAction();
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

        var player = new Player(playerId);

        if (Players.Count == 0)
            Players.AddFirst(player);
        else
            Players.AddLast(player);

        if (Players.Count == Slots)
            Start();

        _hubContext.Clients.All.SendAsync(UpdateHub.Method, Id);
    }

    public void LeaveSession(Guid playerId)
    {
        foreach (var player in Players)
        {
            if (player.Id == playerId)
            {
                Players.Remove(player);
            }
        }
    }

    public PlayerView GetPlayerView(Guid playerId)
    {
        var player = Players.Find(Players.Single(p => p.Id == playerId));

        var me = new Me()
        {
            Id = player.Value.Id,
            Credits = player.Value.Credits,
            Hand = player.Value.Hand
        };

        me.TurnState = _currentPlayerState;
        me.TurnState.MyTurn = CurrentPlayer == player.Value;

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

        return new PlayerView()
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
            Me = me,
            Hidden = hiddenPlayers,
            Pots = new List<Pot>(new[] { HandPot, SabaccPot })
        };
    }

    public SpectatorView GetSpectateView()
    {
        return new SpectatorView();
    }

    public void PlayerTurn(PlayerAction playerAction)
    {
        if (playerAction.Phase == Phase.Choose)
        {
            _currentPlayerState.PhaseOne = playerAction.PhaseOne;

            if (playerAction.PhaseOne.Choice == PhaseOneChoice.Stand)
            {
                NextTurn();
            }
            else if (playerAction.PhaseOne.Choice == PhaseOneChoice.Swap && playerAction.PhaseOne.SwapCardId.HasValue)
            {
                CurrentPlayer.Ante(HandPot, 2);
                Card card = CurrentPlayer.Hand.Find(f => f.Id == playerAction.PhaseOne.SwapCardId)!;
                CurrentPlayer.Hand.Remove(card);
                CurrentPlayer.Hand.Add(DiscardPile.TakeTop());
                DiscardPile.Cards.Push(card);
                NextTurn();
            }
            else if (playerAction.PhaseOne.Choice == PhaseOneChoice.Gain1)
            {
                if (playerAction.PhaseOne.Gain1ShouldDraw())
                {
                    CurrentPlayer.Ante(HandPot, credits: 1);
                    Card card = MainDeck.TakeTop();
                    playerAction.PhaseOne.Gain1DrawnCardId = card.Id;
                    CurrentPlayer.Hand.Add(card);
                }
                else if (playerAction.PhaseOne.Gain1DiscardCardId.HasValue)
                {
                    Card card = CurrentPlayer.Hand.Find(card => card.Id == playerAction.PhaseOne.Gain1DiscardCardId)!;
                    CurrentPlayer.Hand.Remove(card);
                    DiscardPile.Cards.Push(card);
                    NextTurn();
                }
                else if (playerAction.PhaseOne.Gain1KeepCardId.HasValue)
                {
                    NextTurn();
                }
            }
            else if (playerAction.PhaseOne.Choice == PhaseOneChoice.Gain2 && playerAction.PhaseOne.Gain2Discard.HasValue)
            {
                Card card = CurrentPlayer.Hand.Find(card => card.Id == playerAction.PhaseOne.Gain2Discard)!;
                CurrentPlayer.Hand.Remove(card);
                DiscardPile.Cards.Push(card);
                CurrentPlayer.Hand.Add(MainDeck.TakeTop());
                NextTurn();
            }
        }
        else if (playerAction.Phase == Phase.Bet)
        {
            // Bet phase to implement
        }

        _hubContext.Clients.All.SendAsync(UpdateHub.Method, Id);
    }

    private void NextTurn()
    {
        Players.ShiftTurnToNext();
        _currentPlayerState = new PlayerAction()
        {
            SessionId = Id,
            PlayerId = CurrentPlayer.Id
        };
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
        Phase = Phase.Choose;

        _currentPlayerState = new PlayerAction()
        {
            SessionId = Id,
            PlayerId = CurrentPlayer!.Id
        };
    }

    private void DealCards()
    {
        // Get the player next to the dealer
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
            Enumerable.Range(1, 10).Select(x => new Card() { Value = Math.Abs(x), Suit = "Trangles", State = CardState.InDeck }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = Math.Abs(x), Suit = "Squares", State = CardState.InDeck }),
            Enumerable.Range(1, 10).Select(x => new Card() { Value = Math.Abs(x), Suit = "Circles", State = CardState.InDeck })
        };

        MainDeck.AddCards(new Card() { Name = "Sylops", Value = 0 }, new Card() { Name = "Sylops", Value = 0 });
        MainDeck.AddCards(positives.SelectMany(cards => cards).ToArray());
        MainDeck.AddCards(negatives.SelectMany(cards => cards).ToArray());

        MainDeck.Shuffle();
    }
}
