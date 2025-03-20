using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    public interface IPieceMoveCommand : IDisposable
    {

        public List<MoveDirection> Directions { get; }

        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard board);

        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null);

        public IPieceMoveCommand Clone();

    }
}
