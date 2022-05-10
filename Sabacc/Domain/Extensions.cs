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

    public static string ToDisplayName(this NulrhekRank? rank)
    {
        if (rank is null) return string.Empty;

        return rank switch
        {
            NulrhekRank.ClosestToZero => "Closest to Zero",
            NulrhekRank.PositiveScore => "Positive Score",
            NulrhekRank.PositiveScoreWithMostCards => "Positive with most cards",
            NulrhekRank.PositiveScoreWithHighestTotalOfAllPositiveCards => "Highest total of positive cards",
            NulrhekRank.PositiveScoreWithHighestSinglePositiveCardValue => "Highest single positive card"
        };
    }

    public static string ToDisplayName(this HandRank handRank)
    {
        var text = handRank.ToString();
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