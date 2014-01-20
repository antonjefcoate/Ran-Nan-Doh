using System;
using System.Web.Mvc;
using RanNanDoh.Commands;
using RanNanDoh.Domain;
using RanNanDohUi.Models.ActionFilters;
using SimpleCQRS;

namespace RanNanDohUi.Controllers
{
    using System.Collections.Generic;
using RanNanDoh.ReadModels;
using RanNanDohUi.Models;

    [PlayerRequired]
    public class GameController : PlayerEnabledController
    {
        private readonly ICommandSender _bus;
        private readonly IUserActionProvider _playerActionProvider;
        private readonly IPlayerReadModel _playerReadModel;

        public GameController(ICommandSender bus, IUserActionProvider playerActionProvider, IPlayerReadModel playerReadModel)
        {
            _bus = bus;
            _playerActionProvider = playerActionProvider;
            _playerReadModel = playerReadModel;
        }

        public ActionResult Play()
        {
            return View(Player.Id);
        }

        [HttpPost]
        public ActionResult Play(MoveType moveOne, MoveType moveTwo, MoveType moveThree)
        {
            // 1. Create a round
            var roundId = Guid.NewGuid();
            _bus.Send(new CreateRound(roundId, RoundType.VsComputer));
            

            // 2. Play moves
            _bus.Send(new PlayMoves(Player.Id, roundId, moveOne, moveTwo, moveThree));

            return RedirectToAction("PlaySummary", "Player");
        }

        public ActionResult PlayAgainstFriend(Guid? roundId, string opponentId)
        {
            //  Create a round
            if (!roundId.HasValue)
            {
                roundId = Guid.NewGuid();
                _bus.Send(new CreateRound(roundId.Value, RoundType.VsFriend));
            }

            var playerActions = _playerActionProvider.Get(PlayerSource.Facebook);
            var friends = playerActions.GetFbFriends();
            var regdFriends = ((FacebookUserActions)playerActions).GetRegisteredFbFriends();

            var model = new PlayAgainstFriendViewModel(Player.Id, friends, roundId, opponentId, regdFriends);
            return View(model);
        }

        [HttpPost]
        public ActionResult PlayAgainstRegisteredFriend(MoveType moveOne, MoveType moveTwo, MoveType moveThree, long opponentId, Guid roundId)
        {
            // Play moves
            _bus.Send(new PlayMoves(Player.Id, roundId, moveOne, moveTwo, moveThree));

            // 3. Challenge opponents
            //note:How can we get rid of FB specific code?
            var opponent = _playerReadModel.GetFacebookPlayer(opponentId);
            _bus.Send(new ChallengeRegisteredFriend(Player.Id, roundId, opponent.Id));
           
            return RedirectToAction("PlaySummary", "Player");
        }
        [HttpPost]
        public ActionResult PlayAgainstFriend(MoveType moveOne, MoveType moveTwo, MoveType moveThree, Guid roundId)
        {
            // Play moves
            _bus.Send(new PlayMoves(Player.Id, roundId, moveOne, moveTwo, moveThree));

            return RedirectToAction("PlaySummary", "Player");
        }
    }

    public class PlayAgainstFriendViewModel
    {
        private readonly Guid _playerId;
        private readonly Guid? _roundId;
        private readonly string _opponentId;
        private readonly IEnumerable<PotentialPlayer> _players;
        private readonly IEnumerable<PotentialPlayer> _registeredPlayers;

        public PlayAgainstFriendViewModel(Guid playerId, IEnumerable<PotentialPlayer> players, Guid? roundId, string opponentId, IEnumerable<PotentialPlayer> registeredPlayers)
        {
            this._playerId = playerId;
            this._players = players;
            this._registeredPlayers = registeredPlayers;
            this._roundId = roundId;
            this._opponentId = opponentId;
        }

        public IEnumerable<PotentialPlayer> Players
        {
            get
            {
                return this._players;
            }
        }

        public IEnumerable<PotentialPlayer> RegisteredPlayers
        {
            get
            {
                return this._registeredPlayers;
            }
        }

        public Guid PlayerId
        {
            get
            {
                return this._playerId;
            }
        }

        public Guid? RoundId
        {
            get
            {
                return this._roundId;
            }
        }

        public string OpponentId
        {
            get
            {
                return this._opponentId;
            }
        }
    }
}
