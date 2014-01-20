using System;
using System.Security.Authentication;
using System.Web.Mvc;
using RanNanDohUi.Controllers;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDohUi.Models;
using RanNanDohUi.Models.ActionFilters;
using Microsoft.Practices.ServiceLocation;

namespace RanNanDohUiTests.Models.ActionFilters
{
    [TestFixture]
    public class PlayerRequiredTests
    {
        private PlayerRequired _target;

        private IPlayerSession _session;

        private readonly PlayerDto _player = new PlayerDto("", Guid.Empty);

        [SetUp]
        public void Setup()
        {
            _session = Substitute.For<IPlayerSession>();

            var locator = Substitute.For<IServiceLocator>();
            locator.GetInstance<IPlayerSession>().Returns(_session);
            ServiceLocator.SetLocatorProvider(() => locator);

            _target = new PlayerRequired();
        }

        [Test]
        [ExpectedException(typeof(AuthenticationException))]
        public void OnActionExecuting_NoUser_Throws()
        {
            // Arrange
            _session.Get().ReturnsForAnyArgs((PlayerDto)null);
            var controller = new MockPlayerController();

            // Act
            _target.OnActionExecuting(new ActionExecutingContext { Controller = controller });
        }

        [Test]
        public void OnActionExecuting_PlayerEnabledController_AddsUserToController()
        {
            // Arrange
            _session.Get().ReturnsForAnyArgs(_player);
            var controller = new MockPlayerController();

            // Act
            _target.OnActionExecuting(new ActionExecutingContext { Controller = controller });

            // Assert
            Assert.AreEqual(_player, controller.Player);
        }
    
        class MockPlayerController : PlayerEnabledController {}
    }
}
