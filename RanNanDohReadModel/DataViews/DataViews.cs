using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace RanNanDoh.DataViews
{
    using ReadModels;

    public class PlayerListDto : IViewModel
    {
        public List<string> Names { get; set; }
        public int TotalCount { get; set; }
        public Guid Id { get; set; }
    }

    public class PlayerDto : IViewModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public  List<Challenge> ChallengesAwaitingAction{ get; set; }

        public PlayerDto(string name, Guid id)
        {
            Name = name;
            Id = id;
            ChallengesAwaitingAction = new List<Challenge>();
        }
    }

    public class Challenge
    {
        private readonly Guid _roundId;

        private readonly Guid _challengerId;

        private readonly string _challengerName;

        public Challenge(Guid roundId, Guid challengerId, string challengerName)
        {
            this._roundId = roundId;
            this._challengerId = challengerId;
            this._challengerName = challengerName;
        }

        public Guid RoundId
        {
            get
            {
                return this._roundId;
            }
        }

        public Guid ChallengerId
        {
            get
            {
                return this._challengerId;
            }
        }

        public string ChallengerName
        {
            get
            {
                return this._challengerName;
            }
        }
    }

    public class FacebookPlayerDto : PlayerDto
    {
        public FacebookPlayerDto(string name, Guid id, long facebookUid)
            : base(name, id)
        {
            FacebookUid = facebookUid;
        }

        public long FacebookUid { get; set; }
    }

    public class UserRoundResultDto : IViewModel
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string OpponentName { get; set; }
        public RoundResult Result { get; set; }
        public Guid Id { get; set; }
    }


    public class UserRoundResultListDto : IViewModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public List<UserRoundResultDto> Items { get; set; }
    }

    public enum RoundResult
    {
        Pending,
        Won,
        Lost,
        Draw
    }
}