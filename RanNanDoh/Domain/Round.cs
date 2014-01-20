using System;
using RanNanDoh.Domain.Messages;
using RanNanDoh.Domain.Services;
using SimpleCQRS;

namespace RanNanDoh.Domain
{
    public class Round : AggregateRoot
    {
        private readonly IWinnerCalculator _winnerCalculater;

        private Guid _id;
        private MoveSeries _player1Moves;
        private MoveSeries _player2Moves;
        private bool _complete;
        private Guid _winnerId;
        private RoundType _roundType;

        public Round()
        { }

        public Round(Guid id, RoundType roundType)
        {
            ApplyChange(new RoundCreated(id, roundType));
        }

        //public void PlayMoves(MoveSeries moves)
        //{
        //    if (_roundType == RoundType.VsComputer)
        //    {
        //        // Play computer moves first...
        //        var computerMoves = new ComputerMoveGenerator().GetMoves();
        //        ApplyChange(new Player1MovesPlayed(_id, computerMoves, computerMoves.PlayerId));
        //    }

        //    if (_player1Moves == null)
        //    {
        //        ApplyChange(new Player1MovesPlayed(_id, moves, moves.PlayerId));
        //    }
        //    else if (_player2Moves == null)
        //    {
        //        ApplyChange(new Player2MovesPlayed(_id, moves, moves.PlayerId));
        //        HandleWinners();
        //    }
        //    else
        //        throw new InvalidOperationException("Cannot play more than 2 moves");
        //}
        public void PlayMoves(MoveSeries moves, IWinnerCalculator winnerCalculator)
        {
            if (_roundType == RoundType.VsComputer)
            {
                if (_player1Moves == null)
                {
                    ApplyChange(new Player1MovesPlayed(_id, moves, moves.PlayerId));
                }

                //Play computer moves first...
                var computerMoves = new ComputerMoveGenerator().GetMoves();
                ApplyChange(new Player2MovesPlayed(_id, computerMoves, computerMoves.PlayerId));
                HandleWinners(winnerCalculator);
            }else
            {
                if (_player1Moves == null)
                {
                    ApplyChange(new Player1MovesPlayed(_id, moves, moves.PlayerId));
                }else if (_player2Moves == null)
                {
                    ApplyChange(new Player2MovesPlayed(_id, moves, moves.PlayerId));
                    HandleWinners(winnerCalculator);
                }
            }

            

        }

        private void HandleWinners(IWinnerCalculator winnerCalculator)
        {
            // Fire win event
            var winnerId = winnerCalculator.ProcessWinner(_player1Moves, _player2Moves);
            if(winnerId == null)
                ApplyChange(new RoundDraw(_id, _player1Moves.PlayerId, _player2Moves.PlayerId));
            else
                ApplyChange(new RoundWon(_id, winnerId.Value, _player1Moves.PlayerId, _player2Moves.PlayerId));
        }

        #region Event Handling

        private void Apply(RoundCreated e)
        {
            _id = e.Id;
            _roundType = e.RoundType;
        }

        private void Apply(Player1MovesPlayed e)
        {
                _player1Moves = e.Moves;
        }

        private void Apply(Player2MovesPlayed e)
        {
            _player2Moves = e.Moves;
        }

        private void Apply(RoundCompleted e)
        {
            _complete = true;
        }

        private void Apply(RoundWon e)
        {
            _winnerId = e.WinnerId;
        }

        #endregion

        public override Guid Id
        {
            get { return _id; }
        }
    }
}