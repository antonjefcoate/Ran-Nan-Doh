using System;
using System.Collections.Generic;
using System.Linq;

using NSubstitute;
using NUnit.Framework;
using RanNanDoh.Domain;
using RanNanDoh.Domain.Messages;
using RanNanDoh.Domain.Services;
using RanNanDohUiTests;
using SimpleCQRS;

namespace RanNanDohTests.Domain
{
    public class When_a_new_against_a_computer_round_is_created : AggregateRootTestFixture<Round> 
    {
        private Guid _guid;
        private Guid _playerId;
        private MoveSeries _moves;
        private IWinnerCalculator _winnerCalculator;

        protected override void SetUp()
        {
            _winnerCalculator = Substitute.For<IWinnerCalculator>();
        }

        protected override IEnumerable<Event> Given()
        {
            this._guid = Guid.NewGuid();
            this._playerId = Guid.NewGuid();
            this._moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._playerId);
            _winnerCalculator.ProcessWinner(Arg.Any<MoveSeries>(), Arg.Any<MoveSeries>()).Returns((Guid?)null);

            return new List<Event> { new RoundCreated(_guid, RoundType.VsComputer) };
        }

        protected override void When()
        {
            target.PlayMoves(this._moves, _winnerCalculator);
        }


        [Test]
        public void Then_Player1Played_event_is_fired_with_player_id()
        {
            Assert.AreEqual(_playerId, ((Player1MovesPlayed)events[0]).PlayerId);
        }

        [Test]
        public void Then_Player2Played_event_is_fired_with_computer_id()
        {
            Guid computerGuid = Guid.Empty;
            Assert.AreEqual(computerGuid, ((Player2MovesPlayed)events[1]).PlayerId);
        }
    }

    public class When_computer_beats_a_player : AggregateRootTestFixture<Round> 
    {
        private Guid _guid;
        private Guid _playerId;
        private MoveSeries _moves;
        private IWinnerCalculator _winnerCalculator;

        protected override void SetUp()
        {
            _winnerCalculator = Substitute.For<IWinnerCalculator>();
        }

        protected override IEnumerable<Event> Given()
        {
            this._guid = Guid.NewGuid();
            this._playerId = Guid.NewGuid();
            this._moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._playerId);
            _winnerCalculator.ProcessWinner(Arg.Any<MoveSeries>(), Arg.Any<MoveSeries>()).Returns(_playerId);

            return new List<Event> { new RoundCreated(_guid, RoundType.VsComputer) };
        }

        protected override void When()
        {
            target.PlayMoves(this._moves, _winnerCalculator);
        }

        [Test]
        public void Then_RoundWon_event_is_fired_with_player_id()
        {
            Assert.AreEqual(_playerId, ((RoundWon)events.Last()).WinnerId);
        }

    }

    public class When_player_beats_computer : AggregateRootTestFixture<Round>
    {
        private Guid _guid;
        private Guid _playerId;
        private MoveSeries _moves;
        private IWinnerCalculator _winnerCalculator;

        protected override void SetUp()
        {
            _winnerCalculator = Substitute.For<IWinnerCalculator>();
        }

        protected override IEnumerable<Event> Given()
        {
            this._guid = Guid.NewGuid();
            this._playerId = Guid.NewGuid();
            this._moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._playerId);
            _winnerCalculator.ProcessWinner(Arg.Any<MoveSeries>(), Arg.Any<MoveSeries>()).Returns(Guid.Empty);

            return new List<Event> { new RoundCreated(_guid, RoundType.VsComputer) };
        }

        protected override void When()
        {
            target.PlayMoves(this._moves, _winnerCalculator);
        }

        [Test]
        public void Then_RoundWon_event_is_fired_with_computer_guid()
        {
            Guid computerGuid = Guid.Empty;
            Assert.AreEqual(computerGuid, ((RoundWon)events.Last()).WinnerId);
        }

    }

    public class When_player_1_plays_against_another_player : AggregateRootTestFixture<Round>
    {
        private Guid _guid;
        private Guid _player1Id;
        private Guid _player2Id;
        private MoveSeries _moves;
        private IWinnerCalculator _winnerCalculator;

        protected override void SetUp()
        {
            _winnerCalculator = Substitute.For<IWinnerCalculator>();
        }

        protected override IEnumerable<Event> Given()
        {
            this._guid = Guid.NewGuid();
            this._player1Id = Guid.NewGuid();
            this._player2Id = Guid.NewGuid();
            this._moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._player1Id);
            _winnerCalculator.ProcessWinner(Arg.Any<MoveSeries>(), Arg.Any<MoveSeries>()).Returns((Guid?)null);

            return new List<Event> { new RoundCreated(_guid, RoundType.VsFriend) };
        }

        protected override void When()
        {
            target.PlayMoves(this._moves, _winnerCalculator);
        }


        [Test]
        public void Then_Player1Played_event_is_fired_with_player1_id()
        {
            Assert.AreEqual(_player1Id, ((Player1MovesPlayed)events[0]).PlayerId);
        }
    }

    public class When_player_2_responds_to_player_1_invite : AggregateRootTestFixture<Round>
    {
        private Guid _guid;
        private Guid _player1Id;
        private Guid _player2Id;
        private MoveSeries _player1moves;
        private MoveSeries _player2moves;
        private IWinnerCalculator _winnerCalculator;


        protected override void SetUp()
        {
            _winnerCalculator = Substitute.For<IWinnerCalculator>();
        }

        protected override IEnumerable<Event> Given()
        {
            this._guid = Guid.NewGuid();
            this._player1Id = Guid.NewGuid();
            this._player2Id = Guid.NewGuid();
            this._player1moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._player1Id);
            this._player2moves = new MoveSeries(new List<MoveType> { MoveType.Block, MoveType.Kick, MoveType.Punch }, this._player2Id);
            _winnerCalculator.ProcessWinner(Arg.Any<MoveSeries>(), Arg.Any<MoveSeries>()).Returns((Guid?)null);

            return new List<Event> { new RoundCreated(_guid, RoundType.VsFriend), new Player1MovesPlayed(_guid, _player1moves, _player1Id) };
        }

        protected override void When()
        {
            target.PlayMoves(this._player2moves, _winnerCalculator);
        }

        [Test]
        public void Then_Player2Played_event_is_fired_with_player2_id()
        {
            Assert.AreEqual(_player2Id, ((Player2MovesPlayed)events.First(e => e is Player2MovesPlayed)).PlayerId);
        }
    }
}
