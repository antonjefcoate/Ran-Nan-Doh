using System;
using System.Collections.Generic;

namespace RanNanDoh.Domain
{
    public class MoveSeries
    {
        public List<MoveType> Moves { get; set; }
        public Guid PlayerId { get; set; }

        public MoveSeries (List<MoveType> moves, Guid playerId)
        { 
            Moves = moves;
            PlayerId = playerId;
        }
    }
}