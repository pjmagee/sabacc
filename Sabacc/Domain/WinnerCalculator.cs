namespace Sabacc.Domain;

public class WinnerCalculator : IWinnerCalculator
{
    public Player Calculate(PlayersCircularList players)
    {
        bool winnerFound = false;

        var groups = players.GroupBy(player => player.Hand.Rank);

        foreach (var group in groups.OrderBy(x => x.Key))
        {
            if (winnerFound) break;

            if (group.Count() == 1)
            {
                Player winner = group.Single();
                winner.State.PhaseThree.WinningHand = group.Key;
                winner.State.PhaseThree.WonRound = true;
                winnerFound = true;
                break;
            }
        }

        var handLookup = players.ToLookup(x => x.Hand.Rank, x => x);

        if (!winnerFound && handLookup[HandRank.Sabacc].Any())
        {
            // 1. Win by Most Cards
            var mostCards = handLookup[HandRank.Sabacc]
                .GroupBy(p => p.Hand.Count)
                .OrderByDescending(grp => grp.Key)
                .First();

            if (mostCards.Count() == 1)
            {
                Player winner = mostCards.Single();
                winner.State.PhaseThree.WinningHand = winner.Hand.Rank;
                winner.State.PhaseThree.WonRound = true;
                winnerFound = true;
            }

            if (!winnerFound)
            {
                // 2. Win by highest total of all positive cards
                var highestTotal = handLookup[HandRank.Sabacc]
                    .GroupBy(x => x.Hand.Where(c => c.Value > 0).Sum(c => c.Value))
                    .OrderByDescending(grp => grp.Key)
                    .First();

                if (highestTotal.Count() == 1)
                {
                    Player winner = highestTotal.Single();
                    winner.State.PhaseThree.WinningHand = winner.Hand.Rank;
                    winner.State.PhaseThree.WonRound = true;
                    winnerFound = true;
                }

                if (!winnerFound)
                {
                    // 3. Win by highest single positive card
                    var highestSinglePositive = handLookup[HandRank.Sabacc]
                        .GroupBy(x => x.Hand.Where(c => c.Value > 0).Max(c => c.Value))
                        .OrderByDescending(grp => grp.Key)
                        .First();

                    if (highestSinglePositive.Count() == 1)
                    {
                        Player winner = highestSinglePositive.Single();
                        winner.State.PhaseThree.WinningHand = winner.Hand.Rank;
                        winner.State.PhaseThree.WonRound = true;
                        winnerFound = true;
                    }
                }
            }
        }

        if (!winnerFound && handLookup[HandRank.Nulrhek].Any())
        {
            var closestToZero = handLookup[HandRank.Nulrhek]
                .GroupBy(x => Math.Abs(0 - x.Hand.Sum))
                .OrderBy(grp => grp.Key)
                .First();

            if (closestToZero.Count() == 1)
            {
                Player player = closestToZero.First();
                player.State.PhaseThree.WinningHand = player.Hand.Rank;
                player.State.PhaseThree.WonRound = true;
                player.State.PhaseThree.NulrhekRank = NulrhekRank.ClosestToZero;
                winnerFound = true;
            }

            if (!winnerFound)
            {
                var positiveScore = handLookup[HandRank.Nulrhek]
                    .GroupBy(x => x.Hand.Where(c => c.Value > 0).Sum(c => c.Value))
                    .OrderByDescending(grp => grp.Key)
                    .FirstOrDefault();

                if (positiveScore?.Count() == 1)
                {
                    Player player = positiveScore.First();

                    if (player.Hand.Sum > 0)
                    {
                        player.State.PhaseThree.WinningHand = player.Hand.Rank;
                        player.State.PhaseThree.WonRound = true;
                        player.State.PhaseThree.NulrhekRank = NulrhekRank.PositiveScore;
                        winnerFound = true;
                    }
                }
            }

            if (!winnerFound)
            {
                var positiveScoreWithMostCards = handLookup[HandRank.Nulrhek]
                    .GroupBy(x => x.Hand.Count(c => c.Value > 0))
                    .OrderByDescending(x => x.Key)
                    .FirstOrDefault();

                if (positiveScoreWithMostCards?.Count() == 1)
                {
                    Player player = positiveScoreWithMostCards.First();
                    player.State.PhaseThree.WinningHand = player.Hand.Rank;
                    player.State.PhaseThree.WonRound = true;
                    player.State.PhaseThree.NulrhekRank = NulrhekRank.PositiveScoreWithMostCards;
                    winnerFound = true;
                }
            }

            if (!winnerFound)
            {
                var positiveScoreWithHighestTotalValueOfAllPositiveCards
                    = handLookup[HandRank.Nulrhek]
                    .GroupBy(x => x.Hand.Where(c => c.Value > 0).Sum(c => c.Value))
                    .OrderByDescending(x => x.Key)
                    .FirstOrDefault();

                if (positiveScoreWithHighestTotalValueOfAllPositiveCards?.Count() == 1)
                {
                    Player player = positiveScoreWithHighestTotalValueOfAllPositiveCards.First();
                    player.State.PhaseThree.WinningHand = player.Hand.Rank;
                    player.State.PhaseThree.WonRound = true;
                    player.State.PhaseThree.NulrhekRank = NulrhekRank.PositiveScoreWithHighestTotalOfAllPositiveCards;
                    winnerFound = true;
                }
            }

            if (!winnerFound)
            {
                var positiveScoreWithHighestSinglePositiveCardValue
                    = handLookup[HandRank.Nulrhek]
                        .GroupBy(x => x.Hand.Max(c => c.Value > 0))
                        .OrderByDescending(x => x.Key)
                        .FirstOrDefault();

                if (positiveScoreWithHighestSinglePositiveCardValue?.Count() == 1)
                {
                    Player player = positiveScoreWithHighestSinglePositiveCardValue.First();
                    player.State.PhaseThree.WinningHand = player.Hand.Rank;
                    player.State.PhaseThree.WonRound = true;
                    player.State.PhaseThree.NulrhekRank = NulrhekRank.PositiveScoreWithHighestSinglePositiveCardValue;
                    winnerFound = true;
                }
            }
        }

        if (winnerFound)
            return players.Single(p => p.State.PhaseThree.WonRound);

        throw new NotImplementedException("Need to figure out this next bit...");
    }
}