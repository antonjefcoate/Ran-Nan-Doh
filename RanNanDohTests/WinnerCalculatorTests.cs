using System;
using System.Collections.Generic;

namespace RanNanDoh.Domain.Services
{
    using NUnit.Framework;

    [TestFixture]
    public class WinnerCalculatorTests
    {
        private WinnerCalculator _target;
        private Guid _player1 = Guid.NewGuid();
        private Guid _player2 = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _target = new WinnerCalculator();
        }

        [Test]
        public void ProcessWinner_EqualMoveSeries_ReturnNullIndicatingDraw()
        {
            //Arrange
            var moves1 = new MoveSeries(new List<MoveType>{ MoveType.Kick, MoveType.Punch, MoveType.Block }, _player1);
            var moves2 = new MoveSeries(new List<MoveType>{ MoveType.Kick, MoveType.Punch, MoveType.Block }, _player2);
            
            //Act
            Guid? winner =_target.ProcessWinner(moves1, moves2);

            //Assert
            Assert.IsNull(winner);
        }

        [Test]
        public void ProcessWinner_Player1Wins()
        {
            //Arrange
            var moves1 = new MoveSeries(new List<MoveType> { MoveType.Punch, MoveType.Block, MoveType.Block }, _player1);
            var moves2 = new MoveSeries(new List<MoveType> { MoveType.Kick, MoveType.Punch, MoveType.Block }, _player2);

            //Act
            Guid? winner = _target.ProcessWinner(moves1, moves2);

            //Assert
            Assert.AreEqual(_player1, winner);
        }

        [Test]
        public void ProcessWinner_Player2Wins()
        {
            //Arrange
            var moves1 = new MoveSeries(new List<MoveType> { MoveType.Kick, MoveType.Punch, MoveType.Block }, _player1);
            var moves2 = new MoveSeries(new List<MoveType> { MoveType.Punch, MoveType.Block, MoveType.Block }, _player2);

            //Act
            Guid? winner = _target.ProcessWinner(moves1, moves2);

            //Assert
            Assert.AreEqual(_player2, winner);
        }
    }
}
