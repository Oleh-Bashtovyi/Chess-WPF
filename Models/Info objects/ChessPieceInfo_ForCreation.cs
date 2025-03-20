using Chess_game.Behaviour;
using Chess_game.Behaviour.Commands;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess_game.Models
{
    public class ChessPieceInfo_ForCreation : IChessPieceInfo_ForCreation
    {
        #region MAIN INFO
        public byte ID { get; set;}
        public string Name { get; set; } = "Default";
        public int Valuability { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        #endregion


        #region GAME PROCCES
        public List<IPieceMoveCommand> MoveCommands { get; set; }
        public bool IsCanBeCaptured { get; set; } = false;
        public bool IsKingFigure { get; set; } = false;
        public bool IsAbleToCastle { get; set; } = false;
        public bool IsCastleSource { get; set; } = false;
        public bool IsPromotable => PromotionListID > 0;
        #endregion




        private bool _isCanBeRemovedFromList = true;
        public bool IsCanBeRemovedFromList
        {
            get { return _isCanBeRemovedFromList; }
            set
            {
                if (IsOriginalPieceFromGame)
                {
                    throw new InvalidOperationException("This piece belongs to original game. You can not remove it from list or change ability to remove it!");
                }
                else _isCanBeRemovedFromList = value;
            }
        }
        public bool IsOriginalPieceFromGame { get; set; } = false;
        public byte PromotionListID { get; set; } = 0;



        //=================================================================================================================
        // CONSTRUCTOR
        //=================================================================================================================
        public ChessPieceInfo_ForCreation(byte id)
        {
            ID = id;
            MoveCommands = new List<IPieceMoveCommand>();
        }

        public ChessPieceInfo_ForCreation(IChessPiece piece)
        {
            ID = piece.ID;
            Name = piece.Name;
            Valuability = piece.Valuability;
            MoveCommands = piece.MoveCommands.Clone();
            IsCanBeCaptured = piece .IsCanBeCaptured;
            IsKingFigure = piece .IsKingFigure;
            IsAbleToCastle = piece.IsAbleToCastle;
            IsCastleSource = piece.IsCastleSource;
            PromotionListID = piece.PromotionListID;
        }




        //=================================================================================================================
        // METHODS
        //=================================================================================================================
        public int GetValuability()
        {
            return 0;
        }



        public List<IMove> Get_AllPossibleMoves_ForCell(ICell cell, GameBoard board)
        {
            List<IMove> moves = new();
            foreach (var move in MoveCommands)
            {
                moves.AddRange(move.Get_AvailableMoves_ForCell(cell, board));
            }
            return moves;
        }





        public List<IMove> GetAvailableMovesFor_AI_ForCell(ICell cell, GameBoard board)
        {
            List<IMove> moves = new();
            foreach (var move in MoveCommands)
            {
                moves.AddRange(move.Get_AvailableMoves_ForCell(cell, board));
            }
            return moves;
        }


        public List<ICell> Get_AllCellThatUnderAtack_ByCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            List<ICell> cells = new();
            foreach (var move in MoveCommands)
            {
                cells.AddRange(move.Get_AllCellsThatUnderAtack_OfCell(cell, gameBoard, victim));
            }
            return cells;
        }


        public List<IMove> Get_AvailableMoves_ForMoveCommand_ForCell(IPieceMoveCommand command, ICell cell, GameBoard board)
        {
            return command.Get_AvailableMoves_ForCell(cell, board).ToList();
        }


        public string Get_DistinguishingName_UsingColor(Color color) => $"{color}_{Name}";




        public IPlayableChessPiece CLone_UsingColor(Color color)
        {
            throw new NotImplementedException();
        }
    }
}
