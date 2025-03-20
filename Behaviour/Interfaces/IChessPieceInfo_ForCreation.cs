using Chess_game.Behaviour.Commands;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;
using System.Collections.Generic;


namespace Chess_game.Behaviour
{
    public interface IChessPieceInfo_ForCreation : IChessPiece
    {
        public bool IsOriginalPieceFromGame { get; }
        public bool IsCanBeRemovedFromList { get; set; }
        public string Description { get; set; }

        public List<IMove> Get_AvailableMoves_ForMoveCommand_ForCell(IPieceMoveCommand command, ICell cell, GameBoard board);
        public string Get_DistinguishingName_UsingColor(Color color);
        public IPlayableChessPiece CLone_UsingColor(Color color);
    }
}
