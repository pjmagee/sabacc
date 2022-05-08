using System.Collections.Generic;

using Sabacc.Domain;

using Xunit;

namespace Sabacc.Tests;

public class HandsFixture
{
    public IEnumerable<Card> PureSabacc()
    {
        yield return new Card() { Value = 0 };
        yield return new Card() { Value = 0 };
    }

    public IEnumerable<Card> FullSabacc()
    {
        yield return new Card() { Value = 0 };
        yield return new Card() { Value = 10 };
        yield return new Card() { Value = 10 };
        yield return new Card() { Value = -10 };
        yield return new Card() { Value = -10 };
    }
    public IEnumerable<Card> Fleet()
    {
        yield return new Card() { Value = 0 };
        yield return new Card() { Value = 4 };
        yield return new Card() { Value = -4 };
        yield return new Card() { Value = 5 };
        yield return new Card() { Value = -5 };
    }

    public IEnumerable<Card> PrimeSabacc()
    {
        yield return new Card() { Value = 0 };
        yield return new Card() { Value = 10 };
        yield return new Card() { Value = -10 };
    }

    public IEnumerable<Card> YeeHaa()
    {
        yield return new Card() { Value = 0 };
        yield return new Card() { Value = 9 };
        yield return new Card() { Value = -9 };
    }

    public IEnumerable<Card> Rhylet()
    {
        yield return new Card() { Value = 6 };
        yield return new Card() { Value = 6 };
        yield return new Card() { Value = 6 };
        yield return new Card() { Value = -9 };
        yield return new Card() { Value = -9 };
    }

    public IEnumerable<Card> Squadron()
    {
        yield return new Card() { Value = 3 };
        yield return new Card() { Value = 3 };
        yield return new Card() { Value = -3 };
        yield return new Card() { Value = -3 };
    }

    public IEnumerable<Card> GeeWizz1()
    {
        yield return new Card() { Value = 1 };
        yield return new Card() { Value = 2 };
        yield return new Card() { Value = 3 };
        yield return new Card() { Value = 4 };
        yield return new Card() { Value = -10 };
    }

    public IEnumerable<Card> GeeWizz2()
    {
        yield return new Card() { Value = -1 };
        yield return new Card() { Value = -2 };
        yield return new Card() { Value = -3 };
        yield return new Card() { Value = -4 };
        yield return new Card() { Value = 10 };
    }

    public IEnumerable<Card> StraightKhyron()
    {
        yield return new Card() { Value = -10 };
        yield return new Card() { Value = 9 };
        yield return new Card() { Value = 8 };
        yield return new Card() { Value = -7 };
    }

    public IEnumerable<Card> BanthasWild()
    {
        yield return new Card() { Value = -10 };
        yield return new Card() { Value = -2 };
        yield return new Card() { Value = 4 }; // square
        yield return new Card() { Value = 4 }; // triangle
        yield return new Card() { Value = 4 }; // circle
    }

    public IEnumerable<Card> RuleOfTwo()
    {
        yield return new Card() { Value = -6 }; // Optional 5th card btw

        yield return new Card() { Value = -2 };
        yield return new Card() { Value = -2 };

        yield return new Card() { Value = 5 };
        yield return new Card() { Value = 5 };
    }
}
