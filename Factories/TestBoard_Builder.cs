using Chess_game.Behaviour;
using Chess_game.Controls;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public class TestBoard_Builder
    {

        private GameBoard? _gameBoard;
/*        private int _rows;
        private int _columns;


        public static List<BoardInfo> TestingBoards;


        static TestBoard_Builder()
        {
            TestingBoards= new List<BoardInfo>();

            string name = "4X4 board test";
            int rows = 4;
            int cols = 4;
            int[,] board = new int[,]
            {
               { 0,11,0,0},
               { 13,13,13,13},
               { 0,0,0,0},
               { 27,0,21,27},
            };

            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


             name = "4X4 checkmate board test";
             rows = 4;
             cols = 4;
            board = new int[,]
            {
                       { 11,0,  0, 0},
                       { 0,13,  0, 0},
                       { 0,0,  0, 0},
                       { 0,27,  0, 25},
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));      
            
            name = "4X4 checkmate board test 2";
            rows = 4;
            cols = 4;
            board = new int[,]
            {
               { 11,0,  0, 0},
               { 13,13,  0, 0},
               { 0,0,  0, 0},
               { 21,0,  0, 26},
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));




            name = "5X7 castle board test";
            rows = 5;
            cols = 7;
            board = new int[,]
            {
                       { 0, 0,  0, 0,  0,  0,  0},
                       { 0, 0,  0, 0,  0,  13,  0},
                       { 26,0,  0, 21, 0,  0,  26 },
                       { 0, 0,  0, 0,  0,  0,  0},
                       { 0, 0,  0, 0,  0,  0,  0},
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));




            name = "16X16 max board size test";
            rows = 16;
            cols = 16;
            board = new int[,]
            {
                {11, 12, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {12, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 26, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 22, 22, 0, 0, 0, 21},
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "8X6 checkmate test";
            rows = 8;
            cols = 6;
            board = new int[,]
            {
                {11,0,0,0,0,0,},
                {13,13,0,22,0,0 },
                {0,0,0,0,0,0 },
                {0,0,0,0,0,26 },
                {0,0,0,0,0,0 },
                {16,0,0,0,0,0 },
                {0,0,0,0,22,22 },
                {0,0,0,0,0,21 },
                
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


            name = "3X5 stalemate test";
            rows = 3;
            cols = 5;
            board = new int[,]
            {
                {11,0,0,0,0},
                {0,13,0,26,0 },
                {0,0,22,0,21 },
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));

            name = "16X2 stupid board test";
            rows = 16;
            cols = 2;
            board = new int[,]
            {
                {11,0 },
                {0,0 },
                {0,0 },
                {0,0 },
                {0,0 },
                {12,0 },
                {0,0 },
                {0,0 },
                {0,0 },
                {0,0 },
                {0,0 },
                {0,22 },
                {0,0 },
                {0,0 },
                {0,0 },
                {0,21 },
                
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "3X15 Arena";
            rows = 3;
            cols = 15;
            board = new int[,]
            {
                {25,0,0,12,22,0,0,0,0,0,0,22,12,0,15 },
                {0,0,0,0,0,0,0,11,0,21,0,0,0,0,0 },
                {15,0,0,22,12,0,0,0,0,0,0,12,22,0,25 },

            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


            name = "8X8 castle test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                {16,0,0,11,0,0, 0,16 },
                {16,0,0,0,11,0, 0, 16 },
                {16,0,0,11,0,0,0, 11 },
                {13,13,13,13  ,13,13,13, 13 },
                {22,22,22,22,22,22,22, 22 },
                {0,0,0,0,0,0,26 , 0},
                {26,0,0,21,0,0, 0,26 },
                {26,0,0,0,21,0,0,26 }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


            name = "8X8 castle-checkmate test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                {0,0,   0,0,    12,0,    13,0 },
                {26,0,   22,0,   12,0,    13,0 },
                {13,13, 13,13,  12,0,    0,16 },
                {16,0,  0,11,   0,21,   0,0 },
                {12,12, 12,12,  0,0,    0,16 },
                {0,0,   0,22,    0,0,    0,0 },
                {0,0,   0,0,    0,0,    13,0 },
                {0,0,   0,0,    0,0,    13,16 }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "8X8 en passant test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                {13, 13,13, 13, 13, 13, 13, 13 },
                {0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0 },
                {13,0,13,0,13,0,13,0 },
                {22,0,22,0,0,0,0,0 },
                {22,22,22,22,0,22,0,22 },
                {0,0,0,0,0,0,0,0 },
                {22,22,22,22,22,22,22,22 },
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


            name = "6X6 en passant-checkmate test";
            rows = 6;
            cols = 6;
            board = new int[,]
            {
                {11,0,0,0,0,0 },
                {0,0,0,0,0,0 },
                {0,0,0,0,0,0 },
                {0,0,0,0,0,13 },
                {16,0,0,0,0,15 },
                {0,0,22,21,22,0 },
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "6X6 promotion test";
            rows = 6;
            cols = 6;
            board = new int[,]
            {
                {11,15,0,0,0,0 },
                {0,0,0,0,0,0 },
                {0,0,0,22,0,22 },
                {0,0,0,22,22,22 },
                {22,12,0,0,0,21 },
                {0,0,0,0,0,0 },
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "8X8 promotion-knight-checkmate test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                {0,0,    0,13,    16,13,    0,0, },
                {0,0,    0,0,     0,0,      0,0, },
                {0,0,    0,0,     0,0,      0,0, },
                {0,22,   0,0,     0,0,      0,0 },
                {0,0,    0,0,     22,22,    13,0, },
                {0,0,    0,13,    22,13,    22,13 },
                {13,0,   0,13,    14,21,    13,13, },
                {16,0,   0,13,    13,12,    16,0, }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));




            name = "8X8 promotion-Queen-Rock-checkmate test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                            {11,0,   0,0,     0,0,    0,0, },
                            {0,0,    0,0,     0,0,    0,0, },
                            {0,0,    0,0,     0,0,    0,0, },
                            {0,0,    0,0,     0,0,    0,0 },
                            {0,0,    0,0,     0,0,    22,0, },
                            {0,0,    22,22,   22,22,    22,22 },
                            {0,13,   22,22,   22,22,    22,22, },
                            {0,0,    0,0,     0,0,    0,21, }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));




            name = "8X8 promotion-Queen-checkmate test";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                            {0,0,   0,0,    0,0,    0,21, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {22,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {13,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,21, }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



            name = "8X8 kings death end game";
            rows = 8;
            cols = 8;
            board = new int[,]
            {
                            {11,0,   0,0,    0,0,    0,16, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    21,0,    0,0, },
                            {26,0,   0,0,    0,0,    11,0, }
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));



             name = "4X4 Tricky board";
             rows = 4;
             cols = 4;
            board = new int[,]
            {
               { 11,0, 0, 16},  //11 - black king, 16 - black bishop, 13 - black pawn
               { 0,13, 0, 22},  //White: king - 2 (because of pawn), Bishop - 2, pawn - 0, queen - 6
               { 0,0,  0, 21},  //Black: King - 2 (can not catle because of pawn), pawn - 0 (because king will be under atack), Rock - 2
               { 0,27, 0, 25},
            };
            TestingBoards.Add(new BoardInfo(name, rows, cols, board));


            *//*            board = new int[,]
                        {
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0, }
                        };*//*
        }


        public TestBoard_Builder()
        {
        }*/





/*        public void BuildTestBoard(BoardInfo testBoardInfo)
        {
            _gameBoard = new GameBoard(testBoardInfo.Rows, testBoardInfo.Cols);
            _rows = testBoardInfo.Rows;
            _columns = testBoardInfo.Cols;

            for (int i = 0; i < testBoardInfo.Rows; i++)
            {
                for (int j = 0; j < testBoardInfo.Cols; j++)
                {
                    int piece = testBoardInfo.Board[i, j];
                    Color color = piece / 10 == 1 ? Color.Black : Color.White;
                    _gameBoard.AddPieceToBoard(i, j, piece % 10, color);
                }
            }
        }*/
    }
}
