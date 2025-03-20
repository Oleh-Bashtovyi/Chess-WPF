using Chess_game.Behaviour;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public static class MoveDirections_Factory
    {


        public static List<MoveDirection> GetQueenMovesDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
                new MoveDirection (1,0,16), //down
                new MoveDirection (-1,0,16),//up
                new MoveDirection (0,1,16), //right
                new MoveDirection (0,-1,16), //left
                new MoveDirection (1,1,16),  //lower right diagonal
                new MoveDirection (1,-1,16), //lower left diagonal
                new MoveDirection (-1,1,16), //upper right diagonal
                new MoveDirection (-1,-1,16), //upper left diagonal
            };
            return directions;
        }


        public static List<MoveDirection> GetKingMovesDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
                new MoveDirection (1,0,1), //down
                new MoveDirection (-1,0,1),//up
                new MoveDirection (0,1,1), //right
                new MoveDirection (0,-1,1), //left
                new MoveDirection (1,1,1),  //lower right diagonal
                new MoveDirection (1,-1,1), //lower left diagonal
                new MoveDirection (-1,1,1), //upper right diagonal
                new MoveDirection (-1,-1,1), //upper left diagonal
            };
            return directions;
        }


        public static List<MoveDirection> GetKingsCastleDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
                new MoveDirection (0,1,1), //right
                new MoveDirection (0,-1,1), //left
            };
            return directions;
        }




        public static List<MoveDirection> GetKnightMovesDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
                new MoveDirection (2,1,1), //lower-right
                new MoveDirection (-2,1,1),//upper-right
                new MoveDirection (1,2,1), //right-lower
                new MoveDirection (1,-2,1), //left-lower

                new MoveDirection (2,-1,1),  //lower-left
                new MoveDirection (-2,-1,1), //upper-left
                new MoveDirection (-1,2,1), //right-upper
                new MoveDirection (-1,-2,1), //left-upper
            };
            return directions;
        }


        public static List<MoveDirection> GetRockMovesDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
                new MoveDirection (1,0,16), //down
                new MoveDirection (-1,0,16),//up
                new MoveDirection (0,1,16), //right
                new MoveDirection (0,-1,16), //left
            };
            return directions;
        }


        public static List<MoveDirection> GetBishopMovesDirections()
        {
            List<MoveDirection> directions = new List<MoveDirection>()
            {
               new MoveDirection (1,1,16),  //lower right diagonal
                new MoveDirection (1,-1,16), //lower left diagonal
                new MoveDirection (-1,1,16), //upper right diagonal
                new MoveDirection (-1,-1,16), //upper left diagonal
            };
            return directions;
        }



        public static List<MoveDirection> GetPawnAtacksDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    {
                        return new List<MoveDirection>()
                        {
                            new MoveDirection (-1,-1,1),
                            new MoveDirection (-1,1,1),
                        };
                    }
                case Direction.Down:
                    {
                        return new List<MoveDirection>()
                        {
                            new MoveDirection (1,-1,1),
                            new MoveDirection (1,1,1),
                        };
                    }
                default:
                {
                    return new List<MoveDirection>();
                }
            }
        }

    }
}
