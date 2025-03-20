using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands.Action_after_move_Command
{
    public interface IAdditionAction_Command
    {
        short ID { get; }
        void DoAction(IMove sender, GameBoard gameBoard);
        void UndoAction(IMove sender, GameBoard gameBoard);

    }
}
