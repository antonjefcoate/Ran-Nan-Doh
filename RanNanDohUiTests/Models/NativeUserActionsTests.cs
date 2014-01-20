using System;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.Commands;
using RanNanDoh.DataViews;
using RanNanDoh.ReadModels;
using RanNanDohUi.Controllers;
using RanNanDohUi.Infrastructure.WebUtils;
using RanNanDohUi.Models;
using SimpleCQRS;

namespace RanNanDohUiTests.Models
{
    [TestFixture]
    public class NativeUserActionsTests
    {
        private NativeUserActions _target;

        private IPlayerSession _session;
        private ICommandSender _bus;
        private IPlayerReadModel _playerReadModel;

        private readonly PlayerDto _player = new PlayerDto("", Guid.Empty);

        [SetUpAttribute]
        public void Setup()
        {
            _session = Substitute.For<IPlayerSession>();
            _bus = Substitute.For<ICommandSender>();
            _playerReadModel = Substitute.For<IPlayerReadModel>();

            _target = new NativeUserActions(_session, _bus, _playerReadModel);
        }

        [Test]
        public void Create_ReturnsPlayerId()
        {
            // Act
            var result = _target.Create("");

            // Assert
            Assert.AreNotEqual(Guid.Empty, result);
        }

        [Test]
        public void Create_SendsCreateCommand()
        {
            // Arrange
            const string characterName = "characterName";

            // Act
            var result = _target.Create(characterName);

            // Assert
            _bus.Received().Send(Arg.Is<CreateNativePlayer>(x => 
                x.PlayerId == result &&
                x.UserName == characterName));
        }


        [Test]
        public void Create_PopulatesPlayerSession()
        {
            // Arrange
            _playerReadModel.GetUser(Guid.Empty).ReturnsForAnyArgs(_player);

            // Act
            _target.Create("");

            // Assert
            _session.Received().Set(_player);
        }
    }
}
