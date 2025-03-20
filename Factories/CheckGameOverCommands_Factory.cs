using Chess_game.Behaviour.Commands.check_game_over_commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public static class CheckGameOverCommands_Factory
    {

        public static readonly CheckWhetherAtLeastOneKingInBeatenListCommand AtLeastOneKingInBeatenList = new();
        public static readonly CheckWhetherAtLeastOnePieceIsExistingCommand AtLeastOnePieceIsExisting = new();
        public static readonly CheckWhetherAtLeastOnePieceIsExistingForColorsCommand AtLeastOnePieceIsExistingForColors = new();
        public static readonly CheckCheckMateCommand CheckCheckMate = new();
        public static readonly CheckWhetherMoveLimitExceededCommand MoveLimitExceeded = new();
        public static readonly CheckRepetitionDrawCommand RepetitionDraw = new();

        public static List<ICheckGameOverCommand> GetClassicGameGameOverCommands()
        {
            List <ICheckGameOverCommand> list =  new List<ICheckGameOverCommand>()
            {
                AtLeastOneKingInBeatenList,
                AtLeastOnePieceIsExistingForColors,
                CheckCheckMate,
                MoveLimitExceeded,
                RepetitionDraw
            };
            return list.OrderBy(x => x.Priority).ToList();
        }



        public static List<ICheckGameOverCommand> GetFogOfWarGameGameOverCommands()
        {
            List<ICheckGameOverCommand> list = new List<ICheckGameOverCommand>()
            {
                AtLeastOneKingInBeatenList,
                AtLeastOnePieceIsExistingForColors,
                MoveLimitExceeded,
                RepetitionDraw
            };
            return list.OrderBy(x => x.Priority).ToList();
        }



        public static List<ICheckGameOverCommand> GetOneColorTrainingGameGameOverCommands()
        {
            List<ICheckGameOverCommand> list = new List<ICheckGameOverCommand>()
            {
                AtLeastOnePieceIsExisting,
                MoveLimitExceeded
            };
            return list.OrderBy(x => x.Priority).ToList();
        }
    }
}
