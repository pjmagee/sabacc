using System;
using Sabacc.Domain;

using Xunit;

namespace Sabacc.Tests;

public class WinningCalculatorTests : IClassFixture<HandsFixture>
{
    private readonly HandsFixture _handsFixture;
    private readonly WinnerCalculator _winnerCalculator;

    public WinningCalculatorTests(HandsFixture handsFixture)
    {
        _handsFixture = handsFixture;
        _winnerCalculator = new WinnerCalculator();
    }

    [Fact]
    public void PureSabacc_Beats_FullSabacc()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.FullSabacc());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.PureSabacc());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.PureSabacc, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void FullSabacc_Beats_Fleet()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.Fleet());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.FullSabacc());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.FullSabacc, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void Fleet_Beats_PrimeSabacc()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.PrimeSabacc());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.Fleet());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.Fleet, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void PrimeSabacc_Beats_YeeHaw()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.YeeHaa());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.PrimeSabacc());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.PrimeSabacc, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void YeeHaa_Beats_Rhylet()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.Rhylet());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.YeeHaa());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.YeeHaa, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void Rhylet_Beats_Squadron()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First.ValueRef.Hand.AddRange(_handsFixture.Squadron());
        players.Last.ValueRef.Hand.AddRange(_handsFixture.Rhylet());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.Rhylet, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void Squadron_Beats_GeeWhiz()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.GeeWizz1());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.Squadron());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.Squadron, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void GeeWhiz_Beats_StraightKhyron()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.StraightKhyron());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.GeeWizz1());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.GeeWhizz, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void StraightKhyron_Beats_BanthasWild()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.BanthasWild());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.StraightKhyron());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.StraightKhyron, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void BanthasWild_Beats_RuleOfTwo()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.RuleOfTwo1());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.BanthasWild());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.BanthasWild, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void BanthasWild_Beats_RuleOfTwo2()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(_handsFixture.RuleOfTwo2());
        players.Last!.ValueRef.Hand.AddRange(_handsFixture.BanthasWild());

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.BanthasWild, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void Nulrhek_WithMoreCards_Beats_OtherNulrhek()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(new[] { new Card { Value = 2 }, new Card { Value = 2 } });
        players.Last!.ValueRef.Hand.AddRange(new[] { new Card { Value = 1 }, new Card { Value = 1 }, new Card { Value = 2 } });

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.Nulrhek, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }

    [Fact]
    public void Nulrhek_ClosestToZero_Beats_OtherNulrhek()
    {
        var players = new PlayersCircularList();
        players.Join(Guid.NewGuid(), "Loser");
        players.Join(Guid.NewGuid(), "Winner");

        players.First!.ValueRef.Hand.AddRange(new[] { new Card { Value = 1 }, new Card { Value = 1 }, new Card { Value = 2 } });
        players.Last!.ValueRef.Hand.AddRange(new[] { new Card { Value = 2 }, new Card { Value = -3 } });

        var winner = _winnerCalculator.Calculate(players);

        Assert.Equal(HandRank.Nulrhek, winner.Hand.HandRank);
        Assert.Equal(players.Last.Value, winner);
    }
}