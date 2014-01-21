using System;
using RanNanDoh.Domain;
using SimpleCQRS;

namespace RanNanDoh.Commands
{
    public class CreateNativePlayer : Command
    {
        public readonly Guid PlayerId;
        public readonly string UserName;

        public CreateNativePlayer(Guid playerId, string userName)
        {
            PlayerId = playerId;
            UserName = userName;
        }
    }

    public class CreateFacebookPlayer : CreateNativePlayer
    {
        public readonly PlayerSource Source;
        public readonly long FacebookUid;
        public readonly string AuthToken;

        public CreateFacebookPlayer(Guid playerId, string userName, PlayerSource source, long facebookUid, string authToken)
            : base(playerId, userName)
        {
            Source = source;
            FacebookUid = facebookUid;
            AuthToken = authToken;
        }
    }

    public class CreateRound : Command
    {
        public readonly Guid RoundId;
        public readonly RoundType RoundType;

        public CreateRound(Guid roundId, RoundType type)
        {
            RoundId = roundId;
            RoundType = type;
        }
    }

    public class PlayMoves : Command
    {
        public readonly Guid PlayerId;
        public readonly Guid RoundId;
        public readonly MoveType Move1;
        public readonly MoveType Move2;
        public readonly MoveType Move3;

        public PlayMoves(Guid playerId, Guid roundId, MoveType move1, MoveType move2, MoveType move3)
        {
            PlayerId = playerId;
            RoundId = roundId;
            Move1 = move1;
            Move2 = move2;
            Move3 = move3;
        }
    }

    public class ChallengeFriend: Command
    {
        
        public readonly Guid PlayerId;
        public readonly Guid RoundId;
        public readonly string OpponentId;

        public ChallengeFriend(Guid playerId, Guid roundId, string opponentId)
        {
            PlayerId = playerId;
            RoundId = roundId;
            OpponentId = opponentId;
        }
    }

    public class ChallengeRegisteredFriend: Command
    {
        public readonly Guid PlayerId;
        public readonly Guid RoundId;
        public readonly Guid OpponentId;

        public ChallengeRegisteredFriend(Guid playerId, Guid roundId, Guid opponentId)
        {
            PlayerId = playerId;
            RoundId = roundId;
            OpponentId = opponentId;
        }
    }

    public class NotifyPlayer : Command
    {
        public NotifyPlayer(string userName, string challengerUsername, Guid roundId, string authToken, string externalId)
        {
            UserName = userName;
            ChallengerUsername = challengerUsername;
            RoundId = roundId;
            AuthToken = authToken;
            ExternalId = externalId;
        }

        public readonly string UserName;
        public readonly string ChallengerUsername;
        public readonly Guid RoundId;
        public readonly string AuthToken;
        public readonly string ExternalId;
    }
}
