using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using RanNanDoh.Commands;
using RanNanDoh.Domain;
using RanNanDoh.Domain.Services;

using SimpleCQRS;

namespace RanNanDoh.CommandHandlers
{
    public class PlayerCommandHandlers
    {
        private readonly IRepository<Player> _repository;

        public PlayerCommandHandlers(IRepository<Player> repository)
        {
            _repository = repository;
        }

        public void Handle(CreateNativePlayer message)
        {
            var player = new Player(message.PlayerId, message.UserName, PlayerSource.Native, string.Empty);
            _repository.Save(player, -1);
        }

        public void Handle(CreateFacebookPlayer message)
        {
            var player = new Player(message.PlayerId, message.UserName, PlayerSource.Facebook, message.FacebookUid.ToString());
            player.UpdateAuthToken(message.AuthToken);
            _repository.Save(player, -1);
        }
    }

    public class RoundCommandHandlers
    {
        private readonly IRepository<Round> _roundRepo;
        private readonly IRepository<Player> _playerRepo;

        private readonly IWinnerCalculator _winnerCalculator;

        public RoundCommandHandlers(IRepository<Round> roundRepo, IRepository<Player> playerRepo, IWinnerCalculator winnerCalculator)
        {
            _roundRepo = roundRepo;
            _playerRepo = playerRepo;
            _winnerCalculator = winnerCalculator;
        }

        public void Handle(CreateRound message)
        {
            var round = new Round(message.RoundId, message.RoundType);
            //ToDo: -1 says not worried?? Why?
            _roundRepo.Save(round, -1);
        }

        public void Handle(PlayMoves message)
        {
            var round = _roundRepo.GetById(message.RoundId);
            var moveSeries = new MoveSeries(new List<MoveType>{message.Move1, message.Move2, message.Move3}, message.PlayerId);

            round.PlayMoves(moveSeries, _winnerCalculator);

            _roundRepo.Save(round, -1);
        }

        public void Handle(ChallengeRegisteredFriend message)
        {
            var challenger = _playerRepo.GetById(message.PlayerId);
            var opponent = _playerRepo.GetById(message.OpponentId);
            var round = _roundRepo.GetById(message.RoundId);

            challenger.IssueChallenge(opponent, round);

            _roundRepo.Save(round, -1);

            _playerRepo.Save(opponent, -1);
        }
    }

    public class PlayerNotifier : Handles<NotifyPlayer>
    {
        public void Handle(NotifyPlayer message)
        {
            var notifier = ServiceLocator.Current.GetInstance<IChallengeNotifier>();

            // Notify opponent player
            notifier.Notify(message.ExternalId, message.AuthToken, message.RoundId, message.ChallengerUsername, message.UserName);
        }
    }

}
