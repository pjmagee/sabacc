namespace Sabacc.Domain;

//public class SpecialCard : Card, ICloneable
//{
//    public SpecialValue SpecialValue { get; }

//    public SpecialCard(SpecialValue specialValue)
//    {
//        SpecialValue = specialValue;
//    }

//    public override int Value => (int)SpecialValue;

//    public override string Name => Enum.GetName(typeof(SpecialValue), SpecialValue);

//    public override string Type => nameof(SpecialCard);

//    public object Clone()
//    {
//        return new SpecialCard(SpecialValue);
//    }
//}