namespace Sabacc.Domain;

public class Die : IEquatable<Die>
{
    public DieSides Sides { get; private set; }

    public DieSides Roll()
    {
        Sides = ((DieSides[])Enum.GetValues(typeof(DieSides))).MinBy(x => Guid.NewGuid());
        return Sides;
    }

    public bool Equals(Die? other) => Sides.Equals(other?.Sides);

    public Die() => Roll();
}