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
        Assert.True(new Hand(_fixture.RuleOfTwo()).IsRuleOfTwo());
    }

    [Fact]
    public void Hand_Detects_BanthasWild()
    {
        Assert.True(new Hand(_fixture.BanthasWild()).IsBanthasWild());
    }

    [Fact]
    public void Hand_Detects_StraightKhyron()
    {
        Assert.True(new Hand(_fixture.StraightKhyron()).IsStraightKhyron());
    }

    [Fact]
    public void Hand_Detects_GeeWhizz()
    {
        Assert.True(new Hand(_fixture.GeeWizz1()).IsGeeWhiz());

        Assert.True(new Hand(_fixture.GeeWizz2()).IsGeeWhiz());
    }

    [Fact]
    public void Hand_Detects_Squadron()
    {
        Assert.True(new Hand(_fixture.Squadron()).IsSquadron());
    }

    [Fact]
    public void Hand_Detects_Rhylet()
    {
        Assert.True(new Hand(_fixture.Rhylet()).IsRhylet());
    }

    [Fact]
    public void Hand_Detects_YeeHaa()
    {
        Assert.True(new Hand(_fixture.YeeHaa()).IsYeeHaa());
    }

    [Fact]
    public void Hand_Detects_PrimeSabacc()
    {
        Assert.True(new Hand(_fixture.PrimeSabacc()).IsPrimeSabacc());
    }

    [Fact]
    public void Hand_Detects_Fleet()
    {
        Assert.True(new Hand(_fixture.Fleet()).IsFleet());
    }


    [Fact]
    public void Hand_Detects_FullSabacc()
    {
        Assert.True(new Hand(_fixture.FullSabacc()).IsFullSabacc());
    }

    [Fact]
    public void Hand_Detects_PureSabacc()
    {
        Assert.True(new Hand(_fixture.PureSabacc()).IsPureSabacc());
    }
}