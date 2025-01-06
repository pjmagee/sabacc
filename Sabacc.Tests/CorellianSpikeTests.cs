using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Sabacc.Domain;
using Sabacc.Hubs;
using Xunit;
using Moq;
using Sabacc.Domain.SabaccVariants;

namespace Sabacc.Tests
{
    public class CorellianSpikeTests
    {
        Mock<IHubContext<PlayerNotificationHub>> GetHubContextMock()
        {
            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            Mock<IHubContext<PlayerNotificationHub>> hubContext = new Mock<IHubContext<PlayerNotificationHub>>();
            hubContext.Setup(x => x.Clients).Returns(() => mockClients.Object);
            return hubContext;
        }

        [Fact]
        public void Session_Status_WhenNew_IsOpen()
        {
            var hubContext = GetHubContextMock();
            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);

            Assert.Equal(SessionStatus.Open, sabacc.Status);
        }

        [Fact]
        public void Session_Status_WhenFull_IsStarted()
        {
            var hubContext = GetHubContextMock();
            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);

            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            Assert.Equal(SessionStatus.Open, sabacc.Status);

            sabacc.JoinSession(player1, "P1").Wait();
            sabacc.JoinSession(player2, "P2").Wait();

            Assert.Equal(SessionStatus.Started, sabacc.Status);
        }

        [Fact]
        public void WhenStarted_Dealer_IsAssigned()
        {
            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            var hubContext = GetHubContextMock();
            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);

            Assert.Null(sabacc.CurrentDealer);
            sabacc.JoinSession(player1, "P1").Wait();
            sabacc.JoinSession(player2, "P2").Wait();
            Assert.NotNull(sabacc.CurrentDealer);
        }

        [Fact]
        public void WhenStarted_Player_IsAssigned()
        {
            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);

            Assert.Null(sabacc.CurrentPlayer);

            sabacc.JoinSession(player1, "P1").Wait();
            sabacc.JoinSession(player2, "P2").Wait();

            Assert.NotNull(sabacc.CurrentPlayer);
        }

        [Fact]
        public void WhenStarted_FirstPlayer_ToLeft_OfDealer()
        {
            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);

            sabacc.JoinSession(player1, "P1").Wait();
            sabacc.JoinSession(player2, "P2").Wait();

            Assert.Equal(sabacc.Players.CurrentTurn, sabacc.Players.CurrentDealer.Next);
        }

        [Fact]
        public void WhenStarted_TheCards_AreDealt()
        {
            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);
            sabacc.JoinSession(player1, "P1").Wait(); ;
            sabacc.JoinSession(player2, "P2").Wait(); ;

            Assert.Equal(57, sabacc.MainDeck.Cards.Count);
            Assert.Equal(1, sabacc.DiscardPile.Cards.Count);
            Assert.Equal(2, sabacc.Players.First.ValueRef.Hand.Count);
            Assert.Equal(2, sabacc.Players.Last.ValueRef.Hand.Count);
        }

        [Fact]
        public void CurrentPlayer_WhenStarted_PlayerCanPickAllChoices()
        {
            Guid player1 = Guid.NewGuid();
            Guid player2 = Guid.NewGuid();

            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);
            sabacc.JoinSession(player1, "P1").Wait();
            sabacc.JoinSession(player2, "P2").Wait();

            var view = sabacc.GetPlayerView(sabacc.CurrentPlayer.Id);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());
        }

        [Fact]
        public void CurrentPlayer_PhaseOne_Swap_Cannot_Choose_Other()
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);
            sabacc.SetSlots(2);
            sabacc.JoinSession(Guid.NewGuid(), "P1").Wait();
            sabacc.JoinSession(Guid.NewGuid(), "P2").Wait();

            var player1 = sabacc.CurrentPlayer.Id;

            var view = sabacc.GetPlayerView(player1);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());

            var action = new PlayerAction(new PhaseOne() { Choice = PhaseOneChoice.Swap, SwapCardId = view.Me.Hand[0].Id });
            sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, action).Wait();

            view = sabacc.GetPlayerView(player1);

            Assert.False(view.CanGainOption1());
            Assert.False(view.CanGainOption2());
            Assert.False(view.CanSwap());
            Assert.False(view.CanStand());
        }

        [Fact]
        public void CurrentPlayer_PhaseOne_Gain1_Cannot_Choose_Other()
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(2);
            sabacc.JoinSession(Guid.NewGuid(), "P1").Wait();
            sabacc.JoinSession(Guid.NewGuid(), "P2").Wait();

            var player1 = sabacc.CurrentPlayer.Id;

            var view = sabacc.GetPlayerView(player1);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());

            sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, new PlayerAction(new PhaseOne { Choice = PhaseOneChoice.Gain1 })).Wait();

            view = sabacc.GetPlayerView(player1);

            Assert.True(view.Me.State.PhaseOne.Gain1DrawnCardId.HasValue);

            Assert.False(view.CanGainOption1());
            Assert.False(view.CanGainOption2());
            Assert.False(view.CanSwap());
            Assert.False(view.CanStand());
        }

        [Fact]
        public void CurrentPlayer_PhaseOne_Gain2_Cannot_Choose_Other()
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(2);
            sabacc.JoinSession(Guid.NewGuid(), "P1").Wait();
            sabacc.JoinSession(Guid.NewGuid(), "P2").Wait();

            var player1 = sabacc.CurrentPlayer.Id;

            var view = sabacc.GetPlayerView(player1);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());

            var action = new PlayerAction(new PhaseOne(PhaseOneChoice.Gain2) {Gain2Discard = view.Me.Hand[0].Id});
            sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, action).Wait();

            view = sabacc.GetPlayerView(player1);

            Assert.False(view.CanGainOption1());
            Assert.False(view.CanGainOption2());
            Assert.False(view.CanSwap());
            Assert.False(view.CanStand());
        }

        [Fact]
        public void CurrentPlayer_PhaseOne_Gain1_CanOnly_Discard_Or_Keep_DrawnCard()
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(2);
            sabacc.JoinSession(Guid.NewGuid(), "P1").Wait();
            sabacc.JoinSession(Guid.NewGuid(), "P2").Wait();

            var player1 = sabacc.CurrentPlayer.Id;

            var view = sabacc.GetPlayerView(player1);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());

            sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, new PlayerAction(new PhaseOne(PhaseOneChoice.Gain1))).Wait();

            view = sabacc.GetPlayerView(player1);

            Assert.True(view.Me.State.PhaseOne.Gain1DrawnCardId.HasValue);
            Assert.True(view.CanKeepOrDiscard(view.Me.Hand.Find(c => c.Id == view.Me.State.PhaseOne.Gain1DrawnCardId.Value)));
            Assert.False(view.CanKeepOrDiscard(view.Me.Hand.Find(c => c.Id != view.Me.State.PhaseOne.Gain1DrawnCardId.Value)));

            Assert.False(view.CanGainOption1());
            Assert.False(view.CanGainOption2());
            Assert.False(view.CanSwap());
            Assert.False(view.CanStand());
        }

        [Fact]
        public void CurrentPlayer_PhaseOne_Stand_Cannot_Choose_Other()
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(2);
            sabacc.JoinSession(Guid.NewGuid(), "P1").Wait();
            sabacc.JoinSession(Guid.NewGuid(), "P2").Wait();

            var player1 = sabacc.CurrentPlayer.Id;

            var view = sabacc.GetPlayerView(player1);

            Assert.True(view.CanGainOption1());
            Assert.True(view.CanGainOption2());
            Assert.True(view.CanSwap());
            Assert.True(view.CanStand());

            sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, new PlayerAction(new PhaseOne(PhaseOneChoice.Stand))).Wait();

            view = sabacc.GetPlayerView(player1);

            Assert.False(view.CanGainOption1());
            Assert.False(view.CanGainOption2());
            Assert.False(view.CanSwap());
            Assert.False(view.CanStand());
        }

        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [Theory]
        public void Players_Finish_PhaseOne_Starts_PhaseTwo(int players)
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(players);

            var ids = Enumerable.Range(1, players).Select(x => Guid.NewGuid()).ToList();

            foreach (var id in ids)
            {
                sabacc.JoinSession(id, id.ToString()).Wait();
            }

            Assert.All(ids, playerId => sabacc.PlayerIds.Contains(playerId));

            for (int i = 0; i < ids.Count; i++)
            {
                var action = new PlayerAction(new PhaseOne(PhaseOneChoice.Stand));
                sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, action).Wait();
            }

            var view = sabacc.GetPlayerView(sabacc.CurrentPlayer.Id);

            Assert.Equal(Phase.Two, view.Phase);
        }

        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [Theory]
        public void Players_PhaseTwo_All_Check_Starts_PhaseThree(int players)
        {
            var hubContext = GetHubContextMock();

            var winningCalculator = new Mock<IWinnerCalculator>();
            var sabacc = new CorellianSpikeBlackSpireOutpostRules(hubContext.Object, winningCalculator.Object);

            sabacc.SetSlots(players);

            var ids = Enumerable.Range(1, players).Select(x => Guid.NewGuid()).ToList();

            foreach (var id in ids)
            {
                sabacc.JoinSession(id, id.ToString()).Wait();
            }

            Assert.All(ids, playerId => sabacc.PlayerIds.Contains(playerId));

            for (int i = 0; i < ids.Count; i++)
            {
                sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, new PlayerAction(new PhaseOne(PhaseOneChoice.Stand))).Wait();
            }

            foreach (var view in sabacc.PlayerIds.Select(id => sabacc.GetPlayerView(id)))
            {
                Assert.True(view.Me.State.PhaseOne.Completed);
            }

            for (int i = 0; i < ids.Count; i++)
            {
                var action = new PlayerAction(new PhaseTwo {Choice = PhaseTwoChoice.Check});
                sabacc.PlayerTurn(sabacc.CurrentPlayer.Id, action).Wait();
            }

            // If everyone has checked, then all have completed phase 2
            foreach (var view in sabacc.PlayerIds.Select(id => sabacc.GetPlayerView(id)))
            {
                Assert.True(view.Me.State.PhaseTwo.Completed);
            }
        }
    }
}