using System;
using System.Collections.Generic;
using System.Linq;
using RanNanDoh.DataViews;
using RanNanDoh.Domain;
using RanNanDoh.Domain.Messages;
using SimpleCQRS;

namespace RanNanDoh.ReadModels.Handlers
{
    public class UserRoundListDtoHandler :
        Handles<Player1MovesPlayed>,
        Handles<Player2MovesPlayed>,
        Handles<PlayerWasChallenged>,
        Handles<RoundWon>,
        Handles<RoundDraw>,
        IViewModelDropper
    {
        private readonly IViewModelWriter<UserRoundResultListDto> _writer;

        private readonly IViewModelReader<UserRoundResultListDto> _reader;

        private readonly IViewModelReader<PlayerDto> _playerReader;

        public UserRoundListDtoHandler(
            IViewModelWriter<UserRoundResultListDto> writer,
            IViewModelReader<UserRoundResultListDto> reader,
            IViewModelReader<PlayerDto> playerReader)
        {
            _writer = writer;
            _reader = reader;
            _playerReader = playerReader;
        }

        public void Handle(Player1MovesPlayed message)
        {
            AddRoundResultItem(message.PlayerId, message.RoundId);
        }

        public void Handle(Player2MovesPlayed message)
        {
            AddRoundResultItem(message.PlayerId, message.RoundId);
        }

        public void Handle(PlayerWasChallenged message)
        {
            AddRoundResultItem(message.PlayerId, message.RoundId);
        }

        private void AddRoundResultItem(Guid playerId, Guid roundId)
        {
            var playerDto = _playerReader.Get(playerId);
            var item = new UserRoundResultDto { Id = roundId, PlayerId = playerId, PlayerName = playerDto.Name };

            var userResults = _reader.Get(playerId);
            if (userResults == null)
            {
                userResults = new UserRoundResultListDto{Items = new List<UserRoundResultDto>(), Id = playerId};
                userResults.Items.Add(item);
                _writer.Add(playerId, userResults);
            }
            else
            {
                userResults.Items.Add(item);
                _writer.Update(playerId, userResults);
            }
        }

        public void Handle(RoundWon message)
        {
            var user1Rounds = _reader.Get(message.Player1Id);
            var user1Item = user1Rounds.Items.First(r => r.Id == message.RoundId);
            var user2Rounds = _reader.Get(message.Player2Id);
            var user2Item = user2Rounds.Items.First(r => r.Id == message.RoundId);

            for (int i = 0; i < user1Rounds.Items.Count(); i++)
            {
                var result = user1Rounds.Items[i];
                if (result.Id == message.RoundId)
                {
                    user1Rounds.Items[i].OpponentName = user2Item.PlayerName;
                    user1Rounds.Items[i].Result = user1Item.PlayerId == message.WinnerId ? RoundResult.Won : RoundResult.Lost;
                }
            }

            for (int i = 0; i < user2Rounds.Items.Count(); i++)
            {
                var result = user2Rounds.Items[i];
                if (result.Id == message.RoundId)
                {
                    user2Rounds.Items[i].OpponentName = user1Item.PlayerName;
                    user2Rounds.Items[i].Result = user2Item.PlayerId == message.WinnerId ? RoundResult.Won : RoundResult.Lost;
                }
            }
            _writer.Update(message.Player1Id, user1Rounds);
            _writer.Update(message.Player2Id, user2Rounds);
        }

        public void Handle(RoundDraw message)
        {
            UserRoundResultListDto user1Rounds = this._reader.Get(message.Player1Id);
            var user1Item = user1Rounds.Items.First(r => r.Id == message.RoundId);
            UserRoundResultListDto user2Rounds = this._reader.Get(message.Player2Id);
            var user2Item = user2Rounds.Items.First(r => r.Id == message.RoundId);

            for (int i = 0; i < user1Rounds.Items.Count(); i++)
            {
                var result = user1Rounds.Items[i];
                if (result.Id == message.RoundId)
                {
                    user1Rounds.Items[i].OpponentName = user2Item.PlayerName;
                    user1Rounds.Items[i].Result = RoundResult.Draw;
                }
            }

            for (int i = 0; i < user2Rounds.Items.Count(); i++)
            {
                var result = user2Rounds.Items[i];
                if (result.Id == message.RoundId)
                {
                    user2Rounds.Items[i].OpponentName = user1Item.PlayerName;
                    user2Rounds.Items[i].Result = RoundResult.Draw;
                }
            }

            _writer.Update(message.Player1Id, user1Rounds);
            _writer.Update(message.Player2Id, user2Rounds);
        }

        public void Drop()
        {
            _writer.Drop();
        }
    }

    public class PlayerDtoHandler : Handles<PlayerCreated>, Handles<Player1MovesPlayed>,
        IViewModelDropper
    {
        private readonly IViewModelWriter<PlayerDto> _writer;

        private readonly IViewModelReader<PlayerDto> _reader;

        public PlayerDtoHandler(IViewModelWriter<PlayerDto> writer, IViewModelReader<PlayerDto> reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void Handle(PlayerCreated message)
        {
            // ToDo: eventually these can even be stored in seperate locations etc...
            switch (message.Source)
            {
                case PlayerSource.Native:
                    BullShitDatabase.Users.Add(message.Id, new PlayerDto(message.UserName, message.Id));
                    _writer.Add(message.Id, new PlayerDto(message.UserName, message.Id));
                    break;
                case PlayerSource.Facebook:
                    _writer.Add(message.Id, new FacebookPlayerDto(message.UserName, message.Id, long.Parse(message.ExternalId)));
                    break;
            }
        }

        public void Handle(Player1MovesPlayed message)
        {
            this.AddComputerPlayerIfNotFound(message.PlayerId);
            
        }
        public void Handle(Player2MovesPlayed message)
        {
            this.AddComputerPlayerIfNotFound(message.PlayerId);
            var player = _reader.Get(message.PlayerId);
            Challenge toBeDeleted = player.ChallengesAwaitingAction.FirstOrDefault(x => x.RoundId == message.RoundId);
           if(toBeDeleted != null)
           {
               player.ChallengesAwaitingAction.Remove(toBeDeleted);
           }
            _writer.Update(player.Id, player);
        }

        public void Handle(PlayerWasChallenged message)
        {
            this.AddComputerPlayerIfNotFound(message.PlayerId);
            var player = _reader.Get(message.PlayerId);
            var challenger = _reader.Get(message.ChallengerId);
            player.ChallengesAwaitingAction.Add(new Challenge(message.RoundId, challenger.Id, challenger.Name));
            _writer.Update(player.Id, player);
        }

        private void AddComputerPlayerIfNotFound(Guid playerId)
        {
            PlayerDto player;
            try
            {
                player = _reader.Get(playerId);
            }
            //todo: Following exception handling only works in the case of in-memory dictionary
            catch (KeyNotFoundException)
            {
                player = new PlayerDto("Computer", playerId);
                _writer.Add(playerId, player);
            }
        }

        public void Drop()
        {
            _writer.Drop();
        }
    }
}