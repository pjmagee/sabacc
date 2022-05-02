namespace Sabacc.Domain;

public class HiddenPlayer
{
    public Guid Id { get; set; }
    public int Cards { get; set; }
    public int Credits { get; set; }
    public bool IsTurn { get; set; }
    public bool IsDealer { get; set; }
    public PlayChoice LastChoice { get; set; }
}