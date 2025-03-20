using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Linq;

namespace Chess_game.Behaviour
{
    public class Move : IMove
    {
        public Color Executor_Color { get; set; }
        public IPlayableChessPiece Executor_Piece { get; set; }
        public MoveType MainMoveType { get; set; }
        public ICell From_Cell { get; set; }
        public ICell To_Cell { get; set; }

        public short Executed_OnMove { get; set; }
        public int Valuability { get; set; }



        public void ClearMove()
        {
/*            Executor_Piece = null;
            From_Cell = null;
            To_Cell = null;*/
        }



        public IMoveUpdate?[] Updates { get; private set; } = new IMoveUpdate[4];
        public int UpdatesCount { get; private set; }  = 0;



        public Move(Color executor, IPlayableChessPiece executorPiece, ICell fromCell, ICell toCell, MoveType mainMoveType, short executedOnMove = -1, int valuability = 0)
        {
            Executor_Color = executor;
            Executor_Piece = executorPiece;
            MainMoveType = mainMoveType;
            Executed_OnMove = executedOnMove;
            Valuability = valuability;

            From_Cell = fromCell;
            To_Cell = toCell;
        }

        public void Add_Update(IMoveUpdate update)
        {
            if(UpdatesCount == Updates.Length)
            {
                Updates = Updates.Union(new IMoveUpdate[2]).ToArray();
            }

            Updates[UpdatesCount] = update;
            UpdatesCount++;
        }


        public void Remove_Update(IMoveUpdate update)
        {
            if (UpdatesCount != 0)
            {
                Updates[UpdatesCount] = null;
                UpdatesCount--;
            }
        }


        public IMove Clone()
        {
            Move copy = new Move(Executor_Color, Executor_Piece, From_Cell, To_Cell, MainMoveType);

            for (int i = 0; i < UpdatesCount; i++)
            {
                copy.Add_Update(Updates[i]!);
            }

            return copy;
        }

        public string Get_MoveCoding => $"{From_Cell.Abreviation}-{To_Cell.Abreviation}";
        public string Get_MoveType => MainMoveType.ToName();
        public string Get_Executor => $"{Executor_Piece.PieceColor.ToString()[0]}-{Executor_Piece.Name}";


        public override bool Equals(object? obj)
        {
            if(obj is IMove other)
            {


                //executed on move also include!
                //and updates include!
                if(Executor_Color == other.Executor_Color &&
                    Executor_Piece.Equals(other.Executor_Piece) &&
                    MainMoveType.Equals(other.MainMoveType)&&
                    To_Cell.Equals(other.To_Cell) && 
                    From_Cell.Equals(other.From_Cell)) return true;

            }
            return false;
        }


    }
}
