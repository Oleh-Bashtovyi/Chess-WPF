using Chess_game.Behaviour;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System;
using System.Collections.Generic;

namespace Chess_game.Models
{
    public class ZobristHashGenerator
    {
        private Random random= new Random();

        private int isCastledHash;

        private List<uint>[] WhitePiecesHash; 
        private List<uint>[] BlackPiecesHash;
        private HashSet<int> AlreadyGeneratedNumbers = new();
        private Dictionary<int, int> ID_To_Index_Converter= new();
        private int LastIndex_In_ID_To_Index_Converter = 0;



        public ZobristHashGenerator(int Rows, int Cols, ICell[][] board)
        {
            Create_ID_To_Index_Converter(Rows, Cols, board);

            WhitePiecesHash = new List<uint>[Rows * Cols];
            BlackPiecesHash = new List<uint>[Rows * Cols];

            for (int i = 0; i < Rows * Cols; i++)
            {
                List<uint> hashes_White = new List<uint>();
                List<uint> hashes_Black = new List<uint>();
                hashes_White.Capacity = LastIndex_In_ID_To_Index_Converter + 2;
                hashes_Black.Capacity = LastIndex_In_ID_To_Index_Converter + 2;

                for (int j = 0; j < LastIndex_In_ID_To_Index_Converter; j++)
                {

                    hashes_White.Add(generateRandomHash());
                    hashes_Black.Add(generateRandomHash());
                }

                WhitePiecesHash[i] = hashes_White;
                BlackPiecesHash[i] = hashes_Black;
            }

        }


        public uint GetHashForBoard(ICell[][] board)
        {
            uint hash = 0;
            int Columns = board[0].Length;


            //проходимось по кожній клітині дошки
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[0].Length; j++)
                {
                    //якщо клітина не пуста, то робимо XOR для хеша фігури до загальногохешу дошки
                    IPlayableChessPiece? piece = board[i][j]?.Piece;
                    if (piece != null)
                    {
                        //якщо фігура відсутня у матриці констант, то слід її розширити, додавши константу нової фігури
                        Expand_ID_To_Index_Converter_If_ID_Is_Not_Present(piece);

                        hash ^= (piece.PieceColor == Color.White) ? WhitePiecesHash[i * Columns + j][ID_To_Index_Converter[piece.ID]] : BlackPiecesHash[i * Columns + j][ID_To_Index_Converter[piece.ID]];
                    }
                }
            }
            return hash;
        }




        public uint GetHashForBoardUsingMove(uint oldHash, ICell[][] board, IMove move)
        {
            uint newHash = oldHash;
            int length = board[0].Length;

            for (int i = 0; i < move.UpdatesCount; i++)
            {
                IMoveUpdate update = move.Updates[i];

                ICell cell = update.Cell;
                IPlayableChessPiece? piece = update.Old_Piece;
                if (piece != null)
                {
                    Expand_ID_To_Index_Converter_If_ID_Is_Not_Present(piece);

                    newHash ^= (piece.PieceColor == Color.White) ? WhitePiecesHash[cell.X * length + cell.Y][ID_To_Index_Converter[piece.ID]] : BlackPiecesHash[cell.X * length + cell.Y][ID_To_Index_Converter[piece.ID]];
                }
                piece = update.New_Piece;
                if (piece != null)
                {

                    Expand_ID_To_Index_Converter_If_ID_Is_Not_Present(piece);

                    newHash ^= (piece.PieceColor == Color.White) ? WhitePiecesHash[cell.X * length + cell.Y][ID_To_Index_Converter[piece.ID]] : BlackPiecesHash[cell.X * length + cell.Y][ID_To_Index_Converter[piece.ID]];
                }
            }
            
            return newHash;
        }







        private void Create_ID_To_Index_Converter(int Rows, int Cols, ICell[][] board)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    IPlayableChessPiece? piece = board[i][j]?.Piece;
                    if (piece != null && !ID_To_Index_Converter.ContainsKey(piece.ID))
                    {
                        ID_To_Index_Converter.Add(piece.ID, LastIndex_In_ID_To_Index_Converter);
                        LastIndex_In_ID_To_Index_Converter++;
                    }
                }
            }
        }



        private void Expand_ID_To_Index_Converter_If_ID_Is_Not_Present(IPlayableChessPiece? piece)
        {
            if(piece != null && !ID_To_Index_Converter.ContainsKey(piece.ID))
            {
                ID_To_Index_Converter.Add( piece.ID, LastIndex_In_ID_To_Index_Converter);
                LastIndex_In_ID_To_Index_Converter++;


                for (int i = 0; i < WhitePiecesHash.Length; i++)
                {
                    WhitePiecesHash[i].Add(generateRandomHash());
                    BlackPiecesHash[i].Add(generateRandomHash());
                }
            }
        }



        private uint generateRandomHash()
        {
            int hash;
            do
            {
                hash = random.Next(100000, int.MaxValue);

            } while (AlreadyGeneratedNumbers.Contains(hash));

            AlreadyGeneratedNumbers.Add(hash);
            return (uint)hash;
        }
    }
}
