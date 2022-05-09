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
        var hand = new Hand(_fixture.RuleOfTwo());
        Assert.True(hand.IsRuleOfTwo());
        Assert.Equal(HandType.RuleOfTwo, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_BanthasWild()
    {
        var hand = new Hand(_fixture.BanthasWild());
        Assert.True(hand.IsBanthasWild());
        Assert.Equal(HandType.BanthasWild, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_StraightKhyron()
    {
        var hand = new Hand(_fixture.StraightKhyron());
        Assert.True(hand.IsStraightKhyron());
        Assert.Equal(HandType.StraightKhyron, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_GeeWhizz()
    {
        var hand = new Hand(_fixture.GeeWizz1());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandType.GeeWhizz, hand.HandType);

        hand = new Hand(_fixture.GeeWizz2());
        Assert.True(hand.IsGeeWhiz());
        Assert.Equal(HandType.GeeWhizz, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_Squadron()
    {
        var hand = new Hand(_fixture.Squadron());
        Assert.True(hand.IsSquadron());
        Assert.Equal(HandType.Squadron, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_Rhylet()
    {
        var hand = new Hand(_fixture.Rhylet());
        Assert.True(hand.IsRhylet());
        Assert.Equal(HandType.Rhylet, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_YeeHaa()
    {
        var hand = new Hand(_fixture.YeeHaa());
        Assert.True(hand.IsYeeHaa());
        Assert.Equal(HandType.YeeHaa, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_PrimeSabacc()
    {
        var hand = new Hand(_fixture.PrimeSabacc());
        Assert.True(hand.IsPrimeSabacc());
        Assert.Equal(HandType.PrimeSabacc, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_Fleet()
    {
        var hand = new Hand(_fixture.Fleet());
        Assert.True(hand.IsFleet());
        Assert.Equal(HandType.Fleet, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_FullSabacc()
    {
        var hand = new Hand(_fixture.FullSabacc());
        Assert.True(hand.IsFullSabacc());
        Assert.Equal(HandType.FullSabacc, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_PureSabacc()
    {
        var hand = new Hand(_fixture.PureSabacc());
        Assert.True(hand.IsPureSabacc());
        Assert.Equal(HandType.PureSabacc, hand.HandType);
    }

    [Fact]
    public void Hand_Detects_Nulrhek()
    {
        var hand = new Hand(_fixture.Nulrhek1());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandType.Nulrhek, hand.HandType);

        hand = new Hand(_fixture.Nulrhek2());
        Assert.True(hand.IsNulrhek());
        Assert.Equal(HandType.Nulrhek, hand.HandType);
    }
}