using Sabacc.Domain;
using Xunit;

namespace Sabacc.Tests;

public class HandTests : IClassFixture<HandsFixture>
{
    private readonly HandsFixture _fixture;

    public HandTests(HandsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Hand_Detects_RuleOfTwo()
    {
        var hand = new Hand(_fixture.RuleOfTwo1());
        Assert.True(hand.IsRuleOfTwo());
        Assert.Equal(HandRank.RuleOfTwo, hand.HandRank);

        hand = new Hand(_fixture.RuleOfTwo2());
        Assert.True(hand.IsRuleOfTwo());
        Assert.Equal(HandRank.RuleOfTwo, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_BanthasWild()
    {
        var hand = new Hand(_fixture.BanthasWild());
        Assert.True(hand.IsBanthasWild());
        Assert.Equal(HandRank.BanthasWild, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_StraightKhyron()
    {
        var hand = new Hand(_fixture.StraightKhyron());
        Assert.True(hand.IsStraightKhyron());
        Assert.Equal(HandRank.StraightKhyron, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_GeeWhizz()
    {
        var hand = new Hand(_fixture.GeeWizz1());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandRank.GeeWhizz, hand.HandRank);

        hand = new Hand(_fixture.GeeWizz2());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandRank.GeeWhizz, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_Squadron()
    {
        var hand = new Hand(_fixture.Squadron());
        Assert.True(hand.IsSquadron());
        Assert.Equal(HandRank.Squadron, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_Rhylet()
    {
        var hand = new Hand(_fixture.Rhylet());
        Assert.True(hand.IsRhylet());
        Assert.Equal(HandRank.Rhylet, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_YeeHaa()
    {
        var hand = new Hand(_fixture.YeeHaa());
        Assert.True(hand.IsYeeHaa());
        Assert.Equal(HandRank.YeeHaa, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_PrimeSabacc()
    {
        var hand = new Hand(_fixture.PrimeSabacc());
        Assert.True(hand.IsPrimeSabacc());
        Assert.Equal(HandRank.PrimeSabacc, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_Fleet()
    {
        var hand = new Hand(_fixture.Fleet());
        Assert.True(hand.IsFleet());
        Assert.Equal(HandRank.Fleet, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_FullSabacc()
    {
        var hand = new Hand(_fixture.FullSabacc());
        Assert.True(hand.IsFullSabacc());
        Assert.Equal(HandRank.FullSabacc, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_PureSabacc()
    {
        var hand = new Hand(_fixture.PureSabacc());
        Assert.True(hand.IsPureSabacc());
        Assert.Equal(HandRank.PureSabacc, hand.HandRank);
    }

    [Fact]
    public void Hand_Detects_Nulrhek()
    {
        var hand = new Hand(_fixture.Nulrhek1());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandRank.Nulrhek, hand.HandRank);

        hand = new Hand(_fixture.Nulrhek2());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandRank.Nulrhek, hand.HandRank);
    }
}