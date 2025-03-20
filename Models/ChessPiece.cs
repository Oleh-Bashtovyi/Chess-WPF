using Chess_game.Behaviour;
using Chess_game.Behaviour.Commands;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Chess_game.Models
{
    public class ChessPiece : IPlayableChessPiece
    {
        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================

        #region MAIN INFO
        [JsonIgnore] public byte ID { get; }
        [JsonIgnore] public Color PieceColor { get; set; }
        [JsonIgnore] public string Name { get; }
        [JsonIgnore] public int Valuability { get; set; }
        [JsonIgnore] public string GetDistinguishName => $"{PieceColor}_{Name}";

        //Piece will be saved in code, like:     7:1:18:b6
        //it means: 
        //first number = 7 - ID
        //second number = 1 - Color (0 = black, 1 = white)
        //third number = 18 - when first played (-3000 = havent played yet)
        //fourth abreviation - enpassant cell (X or else - no such cell)
        public string PieceEncoding => Get_PieceEncoding();
        #endregion


        #region GAME PROCCES
        [JsonIgnore] public short WasFirstlyPlayed { get; set; } = -3000;
        [JsonIgnore] public (byte, byte)? EnPassantCellToBeatPiece { get; set; } = null;
        [JsonIgnore] public List<IPieceMoveCommand> MoveCommands { get; set; }
        [JsonIgnore] public bool IsCanBeCaptured { get; set; } = false;
        [JsonIgnore] public bool IsKingFigure { get; set; } = false;
        [JsonIgnore] public bool IsAbleToCastle { get; set; } = false;
        [JsonIgnore] public bool IsCastleSource { get; set; } = false;
        [JsonIgnore] public bool IsPromotable => (PromotionListID > 0);
        [JsonIgnore] public bool IsPlayedAtLeastOnce => WasFirstlyPlayed > 0;
        [JsonIgnore] public byte PromotionListID { get; set; } = 0;
        #endregion




        //=================================================================================================================
        // CONSTRUCTOR
        //=================================================================================================================
        public ChessPiece(byte id, string name, Color color, int valuability)
        {
            ID = id;
            Name = name;
            PieceColor = color;
            Valuability = valuability;
            WasFirstlyPlayed = -3000;
            MoveCommands = new List<IPieceMoveCommand>();
        }

        public ChessPiece(IChessPieceInfo_ForCreation info, Color color)
        {
            ID = info.ID;
            Name = info.Name;
            Valuability = info.Valuability;
            MoveCommands = info.MoveCommands.Clone();
            IsCanBeCaptured = info.IsCanBeCaptured;
            IsKingFigure = info.IsKingFigure;
            IsAbleToCastle = info.IsAbleToCastle;
            IsCastleSource = info.IsCastleSource;
            PromotionListID = info.PromotionListID;

            WasFirstlyPlayed = -3000;
            PieceColor = color;
        }




        //=================================================================================================================
        // METHODS
        //=================================================================================================================
        public int GetValuability() => (PieceColor == Color.White) ? Valuability :  (PieceColor == Color.Black) ? -Valuability : 0;



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



        public void MarkAsPlayed(short moveWhenItWasFirstlyPlayed)
        {
            if (!IsPlayedAtLeastOnce)
            {
                WasFirstlyPlayed = moveWhenItWasFirstlyPlayed;
            }
        }

        public void UnMarkAsPlayed( short moveWhenItWasPlayed)
        {
            if (WasFirstlyPlayed >= moveWhenItWasPlayed)
            {
                WasFirstlyPlayed = -3000;
            }
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




        public IPlayableChessPiece Clone()
        {
            ChessPiece piece = new (ID, Name, PieceColor, Valuability);

            piece.IsAbleToCastle = IsAbleToCastle;
            piece.IsCastleSource = IsCastleSource;
            piece.IsCanBeCaptured = IsCanBeCaptured;
            piece.IsKingFigure = IsKingFigure;
            piece.EnPassantCellToBeatPiece = EnPassantCellToBeatPiece;
            piece.MoveCommands = MoveCommands;
            piece.WasFirstlyPlayed = WasFirstlyPlayed;
            piece.PromotionListID= PromotionListID;
            return piece;
        }



        public string Get_PieceEncoding()
        {
            //will be like:    3:1|fp=16|en=b2
            StringBuilder builder = new();
            builder.Append($"{ID}:{(int)PieceColor}");

            List<string> additionInfo= new List<string>();
 
            if(IsPlayedAtLeastOnce)
            {
                additionInfo.Add($"fp={WasFirstlyPlayed}");
            }
            if(EnPassantCellToBeatPiece!= null)
            {
                additionInfo.Add($"en={EnPassantCellToBeatPiece.Value.Item1},{EnPassantCellToBeatPiece.Value.Item2}");
            }


            if(additionInfo.Count > 0)
            {
                builder.Append('|');

                foreach(string item in additionInfo)
                {
                    builder.Append(item);
                    builder.Append(';');
                }
            }

            return builder.ToString();
        }


        //7:8:1_3:1|fpl=16|enp=b2


        //=================================================================================================================
        // OVERRIDE
        //=================================================================================================================
        public override bool Equals(object? obj)
        {
            if(obj is IPlayableChessPiece other)
            {
                if(ID == other.ID &&
                    WasFirstlyPlayed == other.WasFirstlyPlayed &&
                    PromotionListID == other.PromotionListID&&
                    PieceColor == other.PieceColor &&
                    (EnPassantCellToBeatPiece?.Equals(other.EnPassantCellToBeatPiece) ?? ( EnPassantCellToBeatPiece == null && other.EnPassantCellToBeatPiece == null))
                    ) return true;

                return false;
            }


            return base.Equals(obj);
        }

        public void Dispose()
        {
            foreach(IPieceMoveCommand command in MoveCommands)
            {
                command.Dispose();
            }
            MoveCommands.Clear();
        }
    }
}
