using System.Text;

namespace Sabacc.Domain;

public static class Extensions
{
    public static string ToDisplayName(this Phase phase)
    {
        return phase switch
        {
            Phase.One => "Phase 1 (dealt cards)",
            Phase.Two => "Phase 2 (betting)",
            Phase.Three => "Phase 3 (Spike dice)"
        };
    }

    public static string ToDisplayName(this HandType handType)
    {
        var text = handType.ToString();
        var sb = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
            {
                sb.Append(" ");
                sb.Append(text[i]);
            }
            else
            {
                sb.Append(text[i]);
            }
        }

        return sb.ToString().Trim();
    }
}