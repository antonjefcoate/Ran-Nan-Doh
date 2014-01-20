using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDohUi.Controllers;
using RanNanDohUi.Models;
using RanNanDohUi.Models.ActionFilters;

namespace RanNanDohUiTests.Models.ActionFilters
{
    [TestFixture]
    public class PlayerPopulateTests
    {
        private PlayerPopulate _target;

        private IPlayerSession _session;

        private readonly PlayerDto _player = new PlayerDto("", Guid.Empty);

        [SetUp]
        public void Setup()
        {
            _session = Substitute.For<IPlayerSession>();

            var locator = Substitute.For<IServiceLocator>();
            locator.GetInstance<IPlayerSession>().Returns(_session);
            ServiceLocator.SetLocatorProvider(() => locator);

            _target = new PlayerPopulate();
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