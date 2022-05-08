namespace Sabacc.Domain;

public class Card
{
    public Guid Id { get; } = Guid.NewGuid();

    public int Value { get; set; }

    public string Name { get; set; }

    public string Suit { get; set; }


    public override string ToString()
    {
        return !string.IsNullOrWhiteSpace(Suit) ? $"{Value} of {Suit}" : $"{Value} ({Name})";
    }
}

public enum PlayChoice { Draw, Trade, Stand }

// Classic Sabacc
public enum CardState { InDeck, InHand, InProtectedField }
public enum PotType { TheHand, TrueSabacc }

// Classic Sabacc
public enum Suit { Flasks, Sabers, Staves, Coins };

// Classic Sabacc
public enum SuitValue
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Eleven = 11,
    Commander = 12,
    Mistress = 13,
    Master = 14,
    Ace = 15
}

// Classic Sabacc
public enum SpecialValue
{
    TheIdiot = 0,
    TheQueenOfAirAndDarkness = -2,
    Endurance = -8,
    Balance = -11,
    Demise = -13,
    Moderation = -14,
    TheEvilOne = -15,
    TheStar = -17
}