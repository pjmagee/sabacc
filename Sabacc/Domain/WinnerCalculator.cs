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
            }
        }

        if (winnerFound)
            return players.Single(p => p.State.PhaseThree.WonRound);

        throw new NotImplementedException("Need to figure out this next bit...");
    }
}