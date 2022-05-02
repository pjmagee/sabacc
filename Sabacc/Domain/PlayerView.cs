namespace Sabacc.Domain;

public class PlayerView
{
    public List<DeckView> Decks { get; set; }

    public Me Me { get; set; }

    public List<HiddenPlayer> Hidden { get; set; } = new();

    public List<Pot> Pots { get; set; } = new();
}

public class Me
{
    public Guid Id { get; set; }
    public int Credits { get; set; }
    public List<Card> Hand { get; set; }
    public PlayerAction TurnState { get; set; }
    public bool MyTurn => TurnState.MyTurn;
}

public enum DeckType
{
    Draw,
    Discard
}

public class DeckView
{
    public DeckType DeckType { get; set; }
    public string Name { get; set; }
    public int Total { get; set; }
    public string? TopCard { get; set; }
}

public class HiddenPlayer
{
    public Guid Id { get; set; }
    public int Cards { get; set; }
    public int Credits { get; set; }
    public bool IsTurn { get; set; }
    public bool IsDealer { get; set; }
}