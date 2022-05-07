namespace Sabacc.Domain;

public class Deck
{
    public Stack<Card> Cards { get; private set; }

    public Deck()
    {
        Cards = new Stack<Card>();
    }

    public void Shuffle()
    {
        Cards = new Stack<Card>(Cards.OrderBy(x => Guid.NewGuid()));
    }

    public void AddCards(params Card[] cards)
    {
        foreach (var card in cards)
        {
            Cards.Push(card);
        }
    }

    public void AddCards(IEnumerable<Card> cards) => AddCards(cards.ToArray());

    public Card TakeTop() => Cards.Pop();

    public IEnumerable<Card> TakeTop(int count) => Enumerable.Range(0, count).Select(x => Cards.Pop());

    public Card? ViewTop()
    {
        if (Cards.TryPeek(out var card))
            return card;

        return null;
    }
}