using Sabacc.Domain;
using Xunit;

namespace Sabacc.Tests;

public class HandTests(HandsFixture fixture) : IClassFixture<HandsFixture>
{
    [Fact]
    public void Hand_Detects_RuleOfTwo()
    {
        var hand = new Hand(fixture.RuleOfTwo1());
        Assert.True(hand.IsRuleOfTwo());
        Assert.Equal(HandRank.RuleOfTwo, hand.Rank);

        hand = new Hand(fixture.RuleOfTwo2());
        Assert.True(hand.IsRuleOfTwo());
        Assert.Equal(HandRank.RuleOfTwo, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_BanthasWild()
    {
        var hand = new Hand(fixture.BanthasWild());
        Assert.True(hand.IsBanthasWild());
        Assert.Equal(HandRank.BanthasWild, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_StraightKhyron()
    {
        var hand = new Hand(fixture.StraightKhyron());
        Assert.True(hand.IsStraightKhyron());
        Assert.Equal(HandRank.StraightKhyron, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_GeeWhizz()
    {
        var hand = new Hand(fixture.GeeWizz1());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandRank.GeeWhizz, hand.Rank);

        hand = new Hand(fixture.GeeWizz2());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandRank.GeeWhizz, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_Squadron()
    {
        var hand = new Hand(fixture.Squadron());
        Assert.True(hand.IsSquadron());
        Assert.Equal(HandRank.Squadron, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_Rhylet()
    {
        var hand = new Hand(fixture.Rhylet());
        Assert.True(hand.IsRhylet());
        Assert.Equal(HandRank.Rhylet, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_YeeHaa()
    {
        var hand = new Hand(fixture.YeeHaa());
        Assert.True(hand.IsYeeHaa());
        Assert.Equal(HandRank.YeeHaa, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_PrimeSabacc()
    {
        var hand = new Hand(fixture.PrimeSabacc());
        Assert.True(hand.IsPrimeSabacc());
        Assert.Equal(HandRank.PrimeSabacc, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_Fleet()
    {
        var hand = new Hand(fixture.Fleet());
        Assert.True(hand.IsFleet());
        Assert.Equal(HandRank.Fleet, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_FullSabacc()
    {
        var hand = new Hand(fixture.FullSabacc());
        Assert.True(hand.IsFullSabacc());
        Assert.Equal(HandRank.FullSabacc, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_PureSabacc()
    {
        var hand = new Hand(fixture.PureSabacc());
        Assert.True(hand.IsPureSabacc());
        Assert.Equal(HandRank.PureSabacc, hand.Rank);
    }

    [Fact]
    public void Hand_Detects_Nulrhek()
    {
        var hand = new Hand(fixture.Nulrhek1());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandRank.Nulrhek, hand.Rank);

        hand = new Hand(fixture.Nulrhek2());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandRank.Nulrhek, hand.Rank);
    }
}