namespace Sabacc.Domain;

public class WinnerCalculator : IWinnerCalculator
{
    public Player Calculate(PlayersCircularList players)
    {
        bool winnerFound = false;

        var groups = players.GroupBy(player => player.Hand.HandType);

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

        var handLookup = players.ToLookup(x => x.Hand.HandType, x => x);

        if (!winnerFound && handLookup[HandType.Sabacc].Any())
        {
            // 1. Win by Most Cards
            var mostCards = handLookup[HandType.Sabacc]
                .GroupBy(p => p.Hand.Count)
                .OrderByDescending(grp => grp.Key)
                .First();

            if (mostCards.Count() == 1)
            {
                Player winner = mostCards.Single();
                winner.State.PhaseThree.WinningHand = winner.Hand.HandType;
                winner.State.PhaseThree.WonRound = true;
                winnerFound = true;
            }

            if (!winnerFound)
            {
                // 2. Win by highest total of all positive cards
                var highestTotal = handLookup[HandType.Sabacc]
                    .GroupBy(x => x.Hand.Where(c => c.Value > 0).Sum(c => c.Value))
                    .OrderByDescending(grp => grp.Key)
                    .First();

                if (highestTotal.Count() == 1)
                {
                    Player winner = highestTotal.Single();
                    winner.State.PhaseThree.WinningHand = winner.Hand.HandType;
                    winner.State.PhaseThree.WonRound = true;
                    winnerFound = true;
                }

                if (!winnerFound)
                {
                    // 3. Win by highest single positive card
                    var highestSinglePositive = handLookup[HandType.Sabacc]
                        .GroupBy(x => x.Hand.Where(c => c.Value > 0).Max(c => c.Value))
                        .OrderByDescending(grp => grp.Key)
                        .First();

                    if (highestSinglePositive.Count() == 1)
                    {
                        Player winner = highestSinglePositive.Single();
                        winner.State.PhaseThree.WinningHand = winner.Hand.HandType;
                        winner.State.PhaseThree.WonRound = true;
                        winnerFound = true;
                    }
                }
            }
        }

        if (!winnerFound && handLookup[HandType.Nulrhek].Any())
        {
            var closestToZero = handLookup[HandType.Nulrhek]
                .GroupBy(x => Math.Abs(0 - x.Hand.Sum))
                .OrderBy(grp => grp.Key)
                .First();

            if (closestToZero.Count() == 1)
            {
                Player player = closestToZero.First();
                player.State.PhaseThree.WinningHand = player.Hand.HandType;
                player.State.PhaseThree.WonRound = true;
                winnerFound = true;
            }

            // Basically, if they tie...
            if (!winnerFound)
            {
                // 1. Positive closest to 0
                var closestToZeroPositive = handLookup[HandType.Nulrhek]
                    .Where(x => x.Hand.Sum > 0)
                    .GroupBy(x => x.Hand.Sum)
                    .OrderByDescending(grp => grp.Key)
                    .LastOrDefault();

                if (closestToZeroPositive?.Count() == 1)
                {
                    Player player = closestToZeroPositive.First();

                    if (player.Hand.Sum > 0)
                    {
                        player.State.PhaseThree.WinningHand = player.Hand.HandType;
                        player.State.PhaseThree.WonRound = true;
                        winnerFound = true;
                    }
                }
            }

            if (!winnerFound)
            {
                // 2. Positive score with most cards closest to 0
                var mostCards = handLookup[HandType.Nulrhek]
                    .Where(x => x.Hand.Sum > 0)
                    .GroupBy(x => x.Hand.Count)
                    .OrderByDescending(x => x.Key)
                    .First();

                if (mostCards.Count() == 1)
                {
                    Player player = mostCards.First();
                    player.State.PhaseThree.WinningHand = player.Hand.HandType;
                    player.State.PhaseThree.WonRound = true;
                    winnerFound = true;
                }
            }

            if (!winnerFound)
            {
                // 3. Positive score with highest total value of all positive cards, closest to 0


            }
        }

        if (winnerFound)
            return players.Single(p => p.State.PhaseThree.WonRound);


        throw new NotImplementedException("Need to figure out this next bit...");
    }
}