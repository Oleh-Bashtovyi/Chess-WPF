using Chess_game.Behaviour;
using Chess_game.Behaviour.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Extensions
{
    public static class List_Extension
    {

        public static List<IPieceMoveCommand> Clone(this List<IPieceMoveCommand> commands)
        {
            List<IPieceMoveCommand> clonedMoveCommands = new();
            foreach(IPieceMoveCommand command in commands)
            {
                clonedMoveCommands.Add(command.Clone());
            }
            return clonedMoveCommands;
        }


        public static List<MoveDirection> Clone(this List<MoveDirection> moveDirections)
        {
            List<MoveDirection> clonedList= new();
            foreach(var move in moveDirections)
            {
                clonedList.Add(move.Clone());
            }
            return clonedList;
        }
    }
}
