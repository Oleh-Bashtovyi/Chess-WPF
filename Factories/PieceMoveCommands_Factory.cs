using Chess_game.Behaviour.Commands;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public static class PieceMoveCommands_Factory
    {

        private static Dictionary<byte, List<IPieceMoveCommand>> Id_ToPieceMoveCommands { get; } = new();
        private static List<IPieceMoveCommand> EmptyList = new();

        private static IPieceMoveCommand LowerEnPassantCommand = new PawnEnPassantCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Up));
        private static IPieceMoveCommand UpperEnPassantCommand = new PawnEnPassantCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Down));


        static PieceMoveCommands_Factory()
        {
            List<IPieceMoveCommand> KingMoveCommands = new()
            {
                new MoveAndAtackCommand(MoveDirections_Factory.GetKingMovesDirections()),
                new CastleMoveCommand(MoveDirections_Factory.GetKingsCastleDirections())
            };
            Id_ToPieceMoveCommands.Add(1, KingMoveCommands);



            List<IPieceMoveCommand> LowerPawnCommands = new()
            {
                 new AtackCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Up)),
                 new PawnMoveCommand(Direction.Up),
                 new PawnEnPassantCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Up))
            };
            Id_ToPieceMoveCommands.Add(2, LowerPawnCommands);


            List<IPieceMoveCommand> UpperPawnCommands = new()
            {
                 new AtackCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Down)),
                 new PawnMoveCommand(Direction.Down),
                 new PawnEnPassantCommand(MoveDirections_Factory.GetPawnAtacksDirection(Direction.Down))
            };
            Id_ToPieceMoveCommands.Add(3, UpperPawnCommands);

            List<IPieceMoveCommand> KnightCommands = new()
            {
                 new MoveAndAtackCommand(MoveDirections_Factory.GetKnightMovesDirections())
            };
            Id_ToPieceMoveCommands.Add(4, KnightCommands);

            List<IPieceMoveCommand> BishopCommands = new()
            {
                 new MoveAndAtackCommand(MoveDirections_Factory.GetBishopMovesDirections())
            };
            Id_ToPieceMoveCommands.Add(5, BishopCommands);

            List<IPieceMoveCommand> RockCommands = new()
            {
                 new MoveAndAtackCommand(MoveDirections_Factory.GetRockMovesDirections())
            };
            Id_ToPieceMoveCommands.Add(6, RockCommands);

            List<IPieceMoveCommand> QueenCommands = new()
            {
                 new MoveAndAtackCommand(MoveDirections_Factory.GetQueenMovesDirections())
            };
            Id_ToPieceMoveCommands.Add(7, QueenCommands);
        }



        public static List<IPieceMoveCommand> Get_PieceMoveCommand_UsingID(byte id)
        {
            if (Id_ToPieceMoveCommands.ContainsKey(id))
            {
                return Id_ToPieceMoveCommands[id];
            }
            else return EmptyList;
        } 

    }
}
