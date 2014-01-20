using System;
using System.Collections.Generic;
using SimpleCQRS;

namespace RanNanDoh.Domain.Messages
{
    public class FriendChallenged : Event
    {
        public string OpponentId { get; private set; }
        public Guid RoundId { get; private set; }

        public FriendChallenged(string opponentId, Guid roundId)
        {
            OpponentId = opponentId;
            RoundId = roundId;
        }
    }

    public class PlayerChallenged : Event
    {
        public Guid ChallengerId { get; private set; }
        public Guid OpponentId { get; private set; }
        public Guid RoundId { get; private set; }

        public PlayerChallenged(Guid challengerId, Guid opponentId, Guid roundId)
        {
            ChallengerId = challengerId;
            OpponentId = opponentId;
            RoundId = roundId;
        }
    }
    public class AuthTokenUpdated : Event
    {
        public string AuthToken { get; private set; }
        public Guid PlayerId { get; private set; }

        public AuthTokenUpdated(Guid playerId, string authToken)
        {
            PlayerId = playerId;
            AuthToken = authToken;
        }
    }

    public class PlayerCreated : Event
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public PlayerSource Source { get; private set; }
        public string ExternalId { get; private set; }

        public PlayerCreated(Guid id, string userName, PlayerSource source, string externalId)
        {
			Id = id;
			UserName = userName;
            Source = source;
            ExternalId = externalId;
        }
	}

    public class MoveSeriesCreated : Event
    {
        public Guid Id { get; private set; }
        public List<MoveType> Moves { get; private set; }
        public Guid PlayerId { get; private set; }

        public MoveSeriesCreated(Guid id, List<MoveType> moves, Guid playerId)
        {
            Id = id;
            Moves = moves;
            PlayerId = playerId;
		}
	}

    public class RoundCreated : Event
    {
        public Guid Id { get; private set; }
        public RoundType RoundType { get; private set; }

        public RoundCreated(Guid id, RoundType roundType)
        {
            Id = id;
            RoundType = roundType;
		}
	}
    
    public interface IMovesPlayed
    {
        Guid RoundId { get;  }
        MoveSeries Moves { get; }
        Guid PlayerId { get; }
    }
    public class Player1MovesPlayed : Event, IMovesPlayed
    {
        public Guid RoundId { get; private set; }
        public MoveSeries Moves { get; private set; }
        public Guid PlayerId { get; private set; }

        public Player1MovesPlayed(Guid roundId, MoveSeries moves, Guid playerId)
        {
            RoundId = roundId;
            Moves = moves;
            PlayerId = playerId;
		}
	}
    public class Player2MovesPlayed : Event, IMovesPlayed
    {
        public Guid RoundId { get; private set; }
        public MoveSeries Moves { get; private set; }
        public Guid PlayerId { get; private set; }

        public Player2MovesPlayed(Guid roundId, MoveSeries moves, Guid playerId)
        {
            RoundId = roundId;
            Moves = moves;
            PlayerId = playerId;
        }
    }
    
    public class RoundCompleted : Event
    {
        public Guid RoundId { get; private set; }

        public RoundCompleted(Guid roundId)
        {
            RoundId = roundId;
		}
	}

    public class RoundWon : Event
    {
        public Guid RoundId { get; private set; }
        public Guid WinnerId { get; private set; }
        public Guid Player1Id { get; private set; }
        public Guid Player2Id { get; private set; }

        public RoundWon(Guid roundId, Guid winnerId, Guid player1Id, Guid player2Id)
        {
            RoundId = roundId;
            WinnerId = winnerId;
            Player1Id = player1Id;
            Player2Id = player2Id;
        }
    }
    public class RoundDraw : Event
    {
        public Guid RoundId { get; private set; }
        public Guid Player1Id { get; private set; }
        public Guid Player2Id { get; private set; }

        public RoundDraw(Guid roundId, Guid player1Id, Guid player2Id)
        {
            RoundId = roundId;
            Player1Id = player1Id;
            Player2Id = player2Id;
        }
    }
}

