namespace Sabacc.Domain;

public class Dice
{
    private List<Die> dice;

    public IEnumerable<Sides> Roll() => dice.Select(die => die.Roll());

    public bool IsSabaccShift() => dice[0].Equals(dice[1]);

    public Dice()
    {
        dice = new List<Die>(2) { new(), new() };
    }

    private class Die : IEquatable<Die>
    {
        public Sides Side { get; private set; }

        public Sides Roll()
        {
            Side = (Sides)((Sides[])Enum.GetValues(typeof(Sides))).OrderBy(x => Guid.NewGuid()).First();
            return Side;
        }

        public bool Equals(Die? other) => Side.Equals(other?.Side);

        public Die() => Roll();


    }

    public enum Sides
    {
        One = 1,
        Two,
        Three,
        Four,
        Five,
        Six
    };
}