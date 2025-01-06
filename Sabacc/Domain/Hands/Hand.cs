namespace Sabacc.Domain;

public class Hand : List<Card>
{
    public Hand()
    {

    }

    public Hand(IEnumerable<Card> cards) : base(cards)
    {

    }

    public HandRank Rank
    {
        get
        {
            if (IsPureSabacc()) return HandRank.PureSabacc;
            if (IsFullSabacc()) return HandRank.FullSabacc;
            if (IsFleet()) return HandRank.Fleet;
            if (IsPrimeSabacc()) return HandRank.PrimeSabacc;
            if (IsYeeHaa()) return HandRank.YeeHaa;
            if (IsRhylet()) return HandRank.Rhylet;
            if (IsSquadron()) return HandRank.Squadron;
            if (IsGeeWhiz()) return HandRank.GeeWhizz;
            if (IsStraightKhyron()) return HandRank.StraightKhyron;
            if (IsBanthasWild()) return HandRank.BanthasWild;
            if (IsRuleOfTwo()) return HandRank.RuleOfTwo;
            if (IsSabacc()) return HandRank.Sabacc;
            if (IsNulrhek()) return HandRank.Nulrhek;

            throw new InvalidOperationException("Unhandled HandRank");
        }
    }

    public int Sum => this.Sum(c => c.Value);

    public bool IsSabacc() => Sum == 0;

    public bool IsNulrhek() => Sum != 0;

    public bool IsRuleOfTwo()
    {
        var isSabacc = Count == 4 || Count == 5 && Sum == 0;
        var group = this.GroupBy(c => Math.Abs(c.Value));

        return isSabacc && group.Count(pair => pair.Count() == 2) == 2
                        && !group.First().Key.Equals(group.Last().Key);
    }

    public bool IsBanthasWild()
    {
        var sabacc = Count == 3 || Count == 4 || Count == 5 && Sum == 0;
        var isWild = this.GroupBy(x => x.Value).Any(group => group.Count() == 3);
        return sabacc && isWild;
    }

    public bool IsPureSabacc()
    {
        return Count == 2 && this[0].Value == 0 && this[1].Value == 0;
    }

    public bool IsFullSabacc()
    {
        return Count == 5 && Sum == 0 &&
               this.Count(c => c.Value == 0) == 1 &&
               this.Count(c => c.Value == 10) == 2 &&
               this.Count(c => c.Value == -10) == 2;
    }

    public bool IsFleet()
    {
        return Count == 5 && Sum == 0 &&
               this.Count(c => c.Value == 0) == 1 && // 1 sylop
               this.Count(c => c.Value == 10) == 0 && // No tens
               this.Count(c => c.Value == -10) == 0 && // No tens
               this.Count(c => c.Value > 0) == 2 && // 2 positive
               this.Count(c => c.Value < 0) == 2; // 2 negative
    }

    public bool IsPrimeSabacc()
    {
        return Count == 3 && Sum == 0 &&
               this.Count(c => c.Value == 0) == 1 &&
               this.Count(c => c.Value == 10) == 1 &&
               this.Count(c => c.Value == -10) == 1;
    }

    public bool IsYeeHaa()
    {
        return Count == 3 && Sum == 0 &&
               this.Count(c => c.Value == 0) == 1 &&
               this.Count(c => c.Value != 10 && c.Value > 0) == 1 &&
               this.Count(c => c.Value != -10 && c.Value < 0) == 1;
    }

    public bool IsRhylet()
    {
        return Count == 5 && Sum == 0 &&
               (this.GroupBy(x => x.Value).Any(g => g.Count() == 3 && g.Key > 0) &&
                this.GroupBy(x => x.Value).Any(g => g.Count() == 2 && g.Key < 0) ||
                (this.GroupBy(x => x.Value).Any(g => g.Count() == 3 && g.Key < 0) &&
                 this.GroupBy(x => x.Value).Any(g => g.Count() == 2 && g.Key > 0)));

    }

    public bool IsSquadron()
    {
        var isSabacc = Count == 4 && Sum == 0;
        var groups  = this.GroupBy(c => c.Value);
        return isSabacc && groups.Count() == 2 && Math.Abs(groups.First().Key).Equals(Math.Abs(groups.Last().Key));
    }

    public bool IsGeeWhiz()
    {
        return Sum == 0 &&
               (
                   (Exists(c => c.Value == 1) &&
                    Exists(c => c.Value == 2) &&
                    Exists(c => c.Value == 3) &&
                    Exists(c => c.Value == 4) &&
                    Exists(c => c.Value == -10))
                   ||
                   (Exists(c => c.Value == -1) &&
                    Exists(c => c.Value == -2) &&
                    Exists(c => c.Value == -3) &&
                    Exists(c => c.Value == -4) &&
                    Exists(c => c.Value == 10)
                   )
               );
    }

    public bool IsStraightKhyron()
    {
        var four = Sum == 0 && Count == 4;
        var sorted = this.Select(c => Math.Abs(c.Value)).OrderBy(value => value).ToArray();
        return four && Enumerable.Range(1, sorted.Length - 1).All(i => sorted[i] - 1 == sorted[i - 1]);
    }
}