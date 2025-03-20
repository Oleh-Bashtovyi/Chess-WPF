using Chess_game.Controls;
using Chess_game.Models;

namespace Chess_game.Behaviour.Interfaces
{
    public interface IMove
    {
        Color Executor_Color { get;}
        ICell From_Cell { get; }
        ICell To_Cell { get; }
        IPlayableChessPiece Executor_Piece { get; }
        MoveType MainMoveType { get; set; }
        short Executed_OnMove { get; set; }
        int Valuability { get; set; }
        IMoveUpdate?[] Updates { get; }
        int UpdatesCount { get; }


        string Get_MoveCoding { get; }
        string Get_MoveType { get; }
        string Get_Executor { get; }


        void Add_Update(IMoveUpdate update);
        void Remove_Update(IMoveUpdate update);
        void ClearMove();
        IMove Clone();

    }
}
