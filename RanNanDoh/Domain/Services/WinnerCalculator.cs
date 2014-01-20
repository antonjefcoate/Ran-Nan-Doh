using System;

namespace RanNanDoh.Domain.Services
{
    using System.Linq;

    public interface IWinnerCalculator
    {
        /// <summary>
        /// Choosed the winner based on the power matrix. 
        /// In the case of a draw. An empty Guid is returned.
        /// </summary>
        /// <param name="user1Moves"></param>
        /// <param name="user2Moves"></param>
        /// <returns></returns>
        Guid? ProcessWinner(MoveSeries user1Moves, MoveSeries user2Moves);
    }

    public class WinnerCalculator : IWinnerCalculator
    {
        private const int DRAW = 0;
        private const int BEAT = 1;
        private const int DEFEAT = -1;

        /*   0  means draw
         *   1  means win
         *  -1  means loss
         * 
         *      Kick| Punch | Block
         * Kick   0 |   -1  |   1
         * Punch  1 |   0   |  -1
         * Block -1 |   1   |   0
         */
        int[,] _powerMatrix = new[,]
            {
                //Kick  , Punch , Block
  /*Kick*/      { DRAW  , DEFEAT, BEAT      }, 
  /*Punch*/     { BEAT  , DRAW  , DEFEAT    }, 
  /*Block*/     { DEFEAT, BEAT  , DRAW      }
            };
        /// <summary>
        /// Choosed the winner based on the power matrix. 
        /// In the case of a draw. An empty Guid is returned.
        /// </summary>
        /// <param name="user1Moves"></param>
        /// <param name="user2Moves"></param>
        /// <returns></returns>
        public Guid? ProcessWinner(MoveSeries user1Moves, MoveSeries user2Moves)
        {
            int matrixScore = user1Moves.Moves.Cast<int>()
                .Zip(user2Moves.Moves.Cast<int>(), Tuple.Create)
                .Sum(x => _powerMatrix[x.Item1, x.Item2]);

            if (matrixScore > 0) return user1Moves.PlayerId;
            if (matrixScore < 0) return user2Moves.PlayerId;
            return null;
        }
    }
}