using System;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDoh.Domain;
using RanNanDoh.ReadModels;
using RanNanDohUi.Controllers;
using RanNanDohUi.Models;

namespace RanNanDohUiTests.Controllers
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private PlayerController _target;

        private IPlayerReadModel _playerReadModel;
        private IUserActions _facebookUserActions;
        private IUserActions _nativeUserActions;

        private readonly PlayerDto _player = new PlayerDto("", Guid.Empty);

        [SetUpAttribute]
        public void Setup()
        {
            _playerReadModel = Substitute.For<IPlayerReadModel>();
            _facebookUserActions = Substitute.For<IUserActions>();
            _nativeUserActions = Substitute.For<IUserActions>();

            var actionProvider = Substitute.For<IUserActionProvider>();
            actionProvider.Get(PlayerSource.Facebook).Returns(_facebookUserActions);
            actionProvider.Get(PlayerSource.Native).Returns(_nativeUserActions);

            _target = new PlayerController(_playerReadModel, actionProvider);
        }

        
    }
}
