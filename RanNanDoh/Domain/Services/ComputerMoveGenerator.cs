using System;
using System.Linq;

namespace RanNanDoh.Domain.Services
{
    internal class ComputerMoveGenerator
    {
        public MoveSeries GetMoves()
        {
            var rnd = new Random();
            var moves = new[] { MoveType.Block, MoveType.Kick, MoveType.Punch}
                .OrderBy<MoveType, int>((item) => rnd.Next())
                .ToList();

            return new MoveSeries(moves, Guid.Empty);
        }
    }
}