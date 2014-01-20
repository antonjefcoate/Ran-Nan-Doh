using System;
using System.Web.Mvc;
using RanNanDoh.Commands;
using RanNanDoh.DataViews;
using RanNanDoh.Domain;
using RanNanDohUi.Models;
using RanNanDoh.ReadModels;
using RanNanDohUi.Models.ActionFilters;

namespace RanNanDohUi.Controllers
{
    public class PlayerController : PlayerEnabledController
    {
        private readonly IPlayerReadModel _playerReadModel;
        private readonly IUserActionProvider _playerActionProvider;

        public PlayerController(IPlayerReadModel playerReadModel, IUserActionProvider playerActionProvider)
        {
            _playerReadModel = playerReadModel;
            _playerActionProvider = playerActionProvider;
        }

        public ActionResult All()
        {
            PlayerListDto model = _playerReadModel.GetAllPlayers();
            return View(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(Player);
        }

        [HttpPost]
        public ActionResult Create(string userName)
        {
            _playerActionProvider.Get(PlayerSource.Native).Create("");
            return Index();
        }

        [PlayerRequired]
        public ActionResult PlaySummary()
        {
            var model = _playerReadModel.GetPlayerSummaryGameHistory(Player.Id);
            return View(model);
        }
    }
}
