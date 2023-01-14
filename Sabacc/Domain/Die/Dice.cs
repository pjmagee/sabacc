namespace Sabacc.Domain;

public class Dice
{
    private List<Die> dice;

    public DieSides[]? Sides { get; private set; }

    public void Roll()
    {
        Sides = dice.Select(die => die.Roll()).ToArray();
    }

    public bool IsSabaccShift() => dice[0].Equals(dice[1]);

    public Dice()
    {
        dice = new List<Die>(2) { new(), new() };
    }
}