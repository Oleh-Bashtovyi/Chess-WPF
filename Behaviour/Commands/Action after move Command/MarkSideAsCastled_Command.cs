using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;

namespace Chess_game.Behaviour.Commands.Action_after_move_Command
{
    class MarkSideAsCastledCommand : IAdditionAction_Command
    {
        public short ID => 3;

        public Color TargetSide { get; set; } = Color.None;

        public MarkSideAsCastledCommand(Color targetSide)
        {
            TargetSide = targetSide;
        }

        public void DoAction(IMove sender, GameBoard gameBoard)
        {
            gameBoard.Mark_SideAsCastled_ForColor(TargetSide);
        }

        public void UndoAction(IMove sender, GameBoard gameBoard)
        {
            gameBoard.Unmark_SideAsCastled_ForColor(TargetSide);
        }
    }
}
