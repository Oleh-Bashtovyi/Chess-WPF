using Chess_game.Behaviour.Commands;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System.Collections.Generic;

namespace Chess_game.Behaviour
{
    public interface IChessPiece
    {

        public List<IPieceMoveCommand> MoveCommands { get; }

        byte ID { get; }
        int Valuability { get; }
        string Name { get; }
        bool IsAbleToCastle { get; }
        bool IsCastleSource { get; }

        bool IsPromotable { get; }
        byte PromotionListID { get; }


        bool IsKingFigure { get; }
        bool IsCanBeCaptured { get; }


        public List<IMove> Get_AllPossibleMoves_ForCell(ICell cell, GameBoard board);
        public List<IMove> GetAvailableMovesFor_AI_ForCell(ICell cell, GameBoard board);
        public List<ICell> Get_AllCellThatUnderAtack_ByCell(ICell cell, GameBoard gameBoard, ICell? victim = null);

    }
}
