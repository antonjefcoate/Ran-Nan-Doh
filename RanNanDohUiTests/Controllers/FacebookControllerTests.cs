using System;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDoh.Domain;
using RanNanDohUi.Controllers;
using RanNanDohUi.Models;

namespace RanNanDohUiTests.Controllers
{

    [TestFixture]
    public class FacebookControllerTests
    {
        private FacebookController _target;

        private IFacebookClientProvider _facebookClientProvider;
        private IUserActions _facebookUserActions;
        private readonly PlayerDto _player = new PlayerDto("", Guid.Empty);

        [SetUp]
        public void Setup()
        {
            _facebookClientProvider = Substitute.For<IFacebookClientProvider>();
            _facebookUserActions = Substitute.For<IUserActions>();
            var actionProvider = Substitute.For<IUserActionProvider>();
            actionProvider.Get(PlayerSource.Facebook).Returns(_facebookUserActions);
            
            _target = new FacebookController(_facebookClientProvider, actionProvider);
        }
        [Test]
        public void FbCallBack_ExistingUser_DoesNotCreateUser()
        {
            // Arrange
            _facebookUserActions.Login("").ReturnsForAnyArgs(true);
            _target.Player = _player;

            // Act
            _target.FbCallBack("", null);

            // Assert
            _facebookUserActions.DidNotReceive().Create("");
        }

        [Test]
        public void FbCallBack_UserLoggedIn_DoesNothing()
        {
            // Arrange
            _target.Player = _player;

            // Act
            _target.FbCallBack("", null);

            // Assert
            _facebookUserActions.DidNotReceiveWithAnyArgs().Login("");
        }
        
    }
}