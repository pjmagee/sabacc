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