using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RanNanDoh.Domain;
using RanNanDoh.Domain.Messages;
using RanNanDohUiTests;
using SimpleCQRS;

namespace RanNanDohTests.Domain
{
    class When_a_game_is_ongoing_between_two_players : AggregateRootTestFixture<Player> 
    {
        private Guid _roundId;
        private Player _opponent;
        private Guid _opponentId;
        private Guid _playerId;

        protected override IEnumerable<Event> Given()
        {
            return new List<Event>
                {
                    new PlayerChallenged(target.Id, _opponentId, _roundId)
                };
            
        }

        protected override void When()
        {
            this.target.IssueChallenge(_opponent, new Round(_roundId, RoundType.VsFriend));
        }

        protected override void SetUp()
        {
            _roundId = Guid.NewGuid();
            _opponentId = Guid.NewGuid();
            _playerId = Guid.NewGuid();
            _opponent = new Player(_opponentId, "username", PlayerSource.Native, "anything");
        }

        [Test]
        public void ThenAnExceptionIsExpected()
        {
            Assert.That(caught is InvalidOperationException);
        }

        protected override Player CreateSut()
        {
            return new Player(_playerId, "my username", PlayerSource.Native, "anything");
        }
    }
}
