namespace Sabacc.Domain;

public class DeckView
{
    public DeckType DeckType { get; set; }
    public string Name { get; set; }
    public int Total { get; set; }
    public string? TopCard { get; set; }
}