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
    public class Board_Builder
    {
        private GameBoard? _gameBoard;

/*        public static BoardInfo ClassicBoardInfo { get; set; }

        static Board_Builder()
        {
            int rows = 8;
            int columns = 8;

            int[,] classicBoard = new int[,]
            {
                            {16,14,   15,17,    11,15,    14,16, },
                            {13,13,   13,13,    13,13,    13,13, },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {0,0,   0,0,    0,0,    0,0, },
                            {0,0,   0,0,    0,0,    0,0 },
                            {22,22,   22,22,    22,22,    22,22, },
                            {26,24,   25,27,    21,25,    24,26, },
            };

            ClassicBoardInfo = new BoardInfo("Classic board", rows, columns, classicBoard);
        }*/



        public GameBoard GetGameBoard()
        {
            if (_gameBoard == null) throw new Exception("Board must be built firstly!");

            GameBoard board = _gameBoard;

            //clear old board
            _gameBoard = null;

            return board;
        }









        /*        private static readonly string[] ClassicBoardEncoding = new string[64]
                {
                      "0:0:1_6:0:-3000:X",
                      "0:1:1_4:0:-3000:X",
                      "0:2:1_5:0:-3000:X",
                      "0:3:1_7:0:-3000:X",
                      "0:4:1_1:0:-3000:X",
                      "0:5:1_5:0:-3000:X",
                      "0:6:1_4:0:-3000:X",
                      "0:7:1_6:0:-3000:X",
                      "1:0:0_3:0:-3000:X",
                      "1:1:0_3:0:-3000:X",
                      "1:2:0_3:0:-3000:X",
                      "1:3:0_3:0:-3000:X",
                      "1:4:0_3:0:-3000:X",
                      "1:5:0_3:0:-3000:X",
                      "1:6:0_3:0:-3000:X",
                      "1:7:0_3:0:-3000:X",
                      "2:0:0_null",
                      "2:1:0_null",
                      "2:2:0_null",
                      "2:3:0_null",
                      "2:4:0_null",
                      "2:5:0_null",
                      "2:6:0_null",
                      "2:7:0_null",
                      "3:0:0_null",
                      "3:1:0_null",
                      "3:2:0_null",
                      "3:3:0_null",
                      "3:4:0_null",
                      "3:5:0_null",
                      "3:6:0_null",
                      "3:7:0_null",
                      "4:0:0_null",
                      "4:1:0_null",
                      "4:2:0_null",
                      "4:3:0_null",
                      "4:4:0_null",
                      "4:5:0_null",
                      "4:6:0_null",
                      "4:7:0_null",
                      "5:0:0_null",
                      "5:1:0_null",
                      "5:2:0_null",
                      "5:3:0_null",
                      "5:4:0_null",
                      "5:5:0_null",
                      "5:6:0_null",
                      "5:7:0_null",
                      "6:0:0_2:1:-3000:X",
                      "6:1:0_2:1:-3000:X",
                      "6:2:0_2:1:-3000:X",
                      "6:3:0_2:1:-3000:X",
                      "6:4:0_2:1:-3000:X",
                      "6:5:0_2:1:-3000:X",
                      "6:6:0_2:1:-3000:X",
                      "6:7:0_2:1:-3000:X",
                      "7:0:1_6:1:-3000:X",
                      "7:1:1_4:1:-3000:X",
                      "7:2:1_5:1:-3000:X",
                      "7:3:1_7:1:-3000:X",
                      "7:4:1_1:1:-3000:X",
                      "7:5:1_5:1:-3000:X",
                      "7:6:1_4:1:-3000:X",
                      "7:7:1_6:1:-3000:X"
                };*/






        private static readonly string[] ClassicBoardEncoding = new string[64]
        {
      "0:0:1_6:0",
      "0:1:1_4:0",
      "0:2:1_5:0",
      "0:3:1_7:0",
      "0:4:1_1:0",
      "0:5:1_5:0",
      "0:6:1_4:0",
      "0:7:1_6:0",
      "1:0:0_3:0",
      "1:1:0_3:0",
      "1:2:0_3:0",
      "1:3:0_3:0",
      "1:4:0_3:0",
      "1:5:0_3:0",
      "1:6:0_3:0",
      "1:7:0_3:0",
      "2:0:0_null",
      "2:1:0_null",
      "2:2:0_null",
      "2:3:0_null",
      "2:4:0_null",
      "2:5:0_null",
      "2:6:0_null",
      "2:7:0_null",
      "3:0:0_null",
      "3:1:0_null",
      "3:2:0_null",
      "3:3:0_null",
      "3:4:0_null",
      "3:5:0_null",
      "3:6:0_null",
      "3:7:0_null",
      "4:0:0_null",
      "4:1:0_null",
      "4:2:0_null",
      "4:3:0_null",
      "4:4:0_null",
      "4:5:0_null",
      "4:6:0_null",
      "4:7:0_null",
      "5:0:0_null",
      "5:1:0_null",
      "5:2:0_null",
      "5:3:0_null",
      "5:4:0_null",
      "5:5:0_null",
      "5:6:0_null",
      "5:7:0_null",
      "6:0:0_2:1",
      "6:1:0_2:1",
      "6:2:0_2:1",
      "6:3:0_2:1",
      "6:4:0_2:1",
      "6:5:0_2:1",
      "6:6:0_2:1",
      "6:7:0_2:1",
      "7:0:1_6:1",
      "7:1:1_4:1",
      "7:2:1_5:1",
      "7:3:1_7:1",
      "7:4:1_1:1",
      "7:5:1_5:1",
      "7:6:1_4:1",
      "7:7:1_6:1"
        };








        public GameBoard Build_ClassicBoard()
        {
            GameCell[][] grid = new GameCell[8][];
            for (int i = 0; i < 8; i++)
            {
                grid[i] = new GameCell[8];
            }

            foreach (string code in ClassicBoardEncoding)
            {
                GameCell cell = GameCells_Factory.Create_Cell_UsingCode(code);

                grid[cell.X][cell.Y] = cell;
            }

            _gameBoard = new GameBoard(grid);

            return _gameBoard;
        }




        public static GameBoard Create_ClassicBoard()
        {
            GameCell[][] grid = new GameCell[8][];
            for (int i = 0; i < 8; i++)
            {
                grid[i] = new GameCell[8];
            }

            foreach (string code in ClassicBoardEncoding)
            {
                GameCell cell = GameCells_Factory.Create_Cell_UsingCode(code);

                grid[cell.X][cell.Y] = cell;
            }

            return  new GameBoard(grid);
        }






        /*        public void BuildClassicBoard(Color mainPlayer = Color.White)
                {
                    _gameBoard = new GameBoard(_rows, _columns);


                    for (int i = 0; i < ClassicBoardInfo.Rows; i++)
                    {
                        for (int j = 0; j < ClassicBoardInfo.Cols; j++)
                        {
                            int piece = ClassicBoardInfo.Board[i, j];
                            Color color = piece / 10 == 1 ? Color.Black : Color.White;


                            _gameBoard.AddPieceToBoard(i, j, piece % 10, color);
                        }
                    }


        *//*            if (mainPlayer != Color.White)
                    {
                        ReverseBoard();
                    }*//*
                }*/



        /*        private void ReverseBoard()
                {
                    //int midRows = _rows / 2 + ((_rows % 2 == 1) ? 1 : 0);   
                    int midRows = _rows / 2 ;   

                    for (int i = 0; i < midRows; i++)
                    {

                        for (int j = 0; j < _columns; j++)
                        {

                            //swap
                            IPlayableChessPiece? temp = _gameBoard[i, j]?.Piece;

                            _gameBoard[i,j].Piece = _gameBoard[_rows - 1 - i, _columns- 1-j].Piece;

                            _gameBoard[_rows - 1 - i, _columns - 1 - j].Piece = temp;
                        }
                    }

                    if(_rows % 2 == 1)
                    {
                        int midColumns = _columns / 2;

                        for (int i = 0; i < midColumns; i++)
                        {
                            //swap
                            IPlayableChessPiece? temp = _gameBoard[midRows, _columns-1-i]?.Piece;

                            _gameBoard[midRows, i].Piece = _gameBoard[midRows, _columns - 1 - i].Piece;

                            _gameBoard[midRows, _columns - 1 - i].Piece = temp;
                        }
                    }

                }*/




















    }
}
