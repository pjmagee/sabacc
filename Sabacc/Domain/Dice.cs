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

    private class Die : IEquatable<Die>
    {
        public DieSides Sides { get; private set; }

        public DieSides Roll()
        {
            Sides = ((DieSides[])Enum.GetValues(typeof(DieSides))).OrderBy(x => Guid.NewGuid()).First();
            return Sides;
        }

        public bool Equals(Die? other) => Sides.Equals(other?.Sides);

        public Die() => Roll();
    }
}