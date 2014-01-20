using System;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDoh.ReadModels;
using RanNanDohUi.Controllers;
using RanNanDohUi.Infrastructure.WebUtils;
using RanNanDohUi.Models;
using SimpleCQRS;

namespace RanNanDohUiTests.Models
{
    [TestFixture]
    public class PlayerSessionTests
    {
        private PlayerSession _target;

        private ISessionState<PlayerDto> _session;

        [SetUpAttribute]
        public void Setup()
        {
            _session = Substitute.For<ISessionState<PlayerDto>>();

            _target = new PlayerSession(_session);
        }

        [Test]
        public void Get_NoUser_ReturnsNull()
        {
            // Arrange
            _session.Get("").ReturnsForAnyArgs((PlayerDto)null);

            // Act
            var result = _target.Get();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Get_UserExists_ReturnsUser()
        {
            // Arrange
            var player = new PlayerDto("", Guid.Empty);
            _session.Get(PlayerSession.SessionKey).ReturnsForAnyArgs(player);

            // Act
            var result = _target.Get();

            // Assert
            Assert.AreEqual(player, result);
        }

        [Test]
        public void Set_AssignesToSession()
        {
            // Arrange
            var player = new PlayerDto("", Guid.Empty);

            // Act
            _target.Set(player);

            // Assert
            _session.Received().Store(PlayerSession.SessionKey, player);
        }
    }
}
