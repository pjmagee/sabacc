using System.Diagnostics.Eventing.Reader;

namespace Sabacc.Domain;

public class Pot
{
    public PotType PotType { get; }

    public Dictionary<Guid, int> Contributions { get; set; }

    public Pot(PotType potType)
    {
        PotType = potType;
        Contributions = new Dictionary<Guid, int>();
    }

    public int Total => Contributions.Values.Sum();
}