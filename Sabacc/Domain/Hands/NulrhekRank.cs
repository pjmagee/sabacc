namespace Sabacc.Domain;

public enum NulrhekRank
{
    ClosestToZero,
    PositiveScore,
    PositiveScoreWithMostCards,
    PositiveScoreWithHighestTotalOfAllPositiveCards,
    PositiveScoreWithHighestSinglePositiveCardValue
}