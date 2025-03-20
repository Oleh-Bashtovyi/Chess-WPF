using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chess_game.Behaviour
{
    public class AI
    {
        //public GameBoard board { get; set; }
        private Random random = new Random();
        private int MaxDeapth = 3;
        private int Inf = int.MaxValue;

        private Color TargetPlayer { get; set; }

        private Difficulty CurrentDifficulty { get; set; } = Difficulty.Easy;

        private bool IsMovesThatDangerKingShouldBeDeleated = false;

        public AI(Color targetColor)
        {
            TargetPlayer = targetColor;
        }


        private Move? BestMove { get; set; }
        private List<IMove> BestMoves { get; set; } = new();
        public static ulong AnalyzedBoardsCount { get; private set; }

        private int maxProlongationDepth = 2;



        public IMove? Get_TheBestMove_ForColor(Color color, GameBoard board, CancellationToken cancellationToken, bool isMovesThatDangerKingShouldBeDeleated)
        {
            BestMove = null;
            BestMoves.Clear();
            IsMovesThatDangerKingShouldBeDeleated = isMovesThatDangerKingShouldBeDeleated;

            //get moves with alredy done promotion action
            List<IMove> moves;
            if (IsMovesThatDangerKingShouldBeDeleated) moves = board.Get_AllPossibleMoves_WithPromotionAction_WithKingFiguresProtection_ForColor(color);
            else moves = board.Get_AllPossibleMoves_WithPromotionAction_ForColor(color);

            //no moves
            if (moves.Count == 0) return null;
            moves = moves.OrderBy(x => x.Executor_Piece.Valuability).ToList();

            //black will have (+infitity), and white (-infinity)
            int bestScore = color.GetInitialScore();

            foreach (var move in moves)
            {
                cancellationToken.ThrowIfCancellationRequested();

                board.Make_Move(move);

                int score = FindBestScore(board, color.GetOpositeColor(), MaxDeapth, /*Alpha =*/ -Inf, /*beta =*/ Inf, /*Prolongation deapth =*/ false);

                board.Undo_Move(move);

                //this move will lead to win, immediately return it
                if (score == GetMaximalScore(color)) return move;

                UpdateBestScore(ref bestScore, score, color);
                if (bestScore == score)
                {
                    move.Valuability = score;
                    BestMoves.Add(move);
                }
            }

        //order best moves by valuability
        if (color == Color.White)
        {
            BestMoves = BestMoves.OrderByDescending(x => x.Valuability).ToList();
        }
        else
        {
            BestMoves = BestMoves.OrderBy(x => x.Valuability).ToList();
        }

        //search for moves withsame valuability
        int lastMoveWithSameValuability = 0;
        for (int i = 1; i < BestMoves.Count; i++)
        {
            if (BestMoves[i].Valuability == BestMoves[0].Valuability)
            {
                lastMoveWithSameValuability = i;
            }
            else break;
        }


        if(BestMoves.Count== 0)
        {
                throw new Exception();
        }

        return BestMoves[random.Next(0, lastMoveWithSameValuability)];

        }




        public int FindBestScore(GameBoard board, Color currentColor, int deapth, int alpha, int beta, bool prolongateDeapth)
        {
            AnalyzedBoardsCount++;

            //base case
            if ((deapth <= 0 && !prolongateDeapth) || deapth < -maxProlongationDepth) return board.Estimate_ScoreForCurrentBoard();

            List<IMove> moves;
            if (IsMovesThatDangerKingShouldBeDeleated && currentColor != TargetPlayer && MaxDeapth - deapth < 2)
            {
                moves = board.Get_AllPossibleMoves_WithPromotionAction_WithKingFiguresProtection_ForColor(currentColor);
            }
            else moves = board.Get_AllPossibleMoves_WithPromotionAction_ForColor(currentColor);



            int bestScore = currentColor.GetInitialScore();

            foreach (Move move in moves)
            {
                if (IsRoyalPiece(move) != 0)
                {
                    //we capture a royal piece, it is win!
                    return GetMaximalScore(currentColor);
                }

                board.Make_Move(move);

                int score = FindBestScore(board, currentColor.GetOpositeColor(), deapth - 1, alpha, beta, false);

                board.Undo_Move(move);

                UpdateBestScore(ref bestScore, score, currentColor);

                if (currentColor == Color.White)
                {
                    alpha = Math.Max(alpha, bestScore);
                }
                else
                {
                    beta = Math.Min(beta, bestScore);
                }
                if (beta < alpha) break; 

            }

            return bestScore;
        }



        private int AlphaBetaImproved(GameBoard board, int alpha, int beta, Color currentColor, int deapth)
        {
            AnalyzedBoardsCount++;
            if (deapth <= 0) return board.Estimate_ScoreForCurrentBoard();
            int score = -Inf;

            List<IMove> moves = board.Get_AllPossibleMoves_WithPromotionAction_ForColor(currentColor);

            foreach (Move move in moves)
            {
                if (IsRoyalPiece(move) != 0)
                {
                    return GetMaximalScore(currentColor);
                }

                board.Make_Move(move);
                int temp = -AlphaBetaImproved(board, -beta, -alpha, currentColor.GetOpositeColor(), deapth - 1);
                board.Undo_Move(move);

                if (temp > score) score = temp;
                if (score > alpha) alpha = score;
                if (alpha >= beta) return alpha;
            }

            return alpha;
        }


        void UpdateBestScore( ref int bestScore, int newScore, Color color)
        {
            if (color == Color.White)
            {
                bestScore = Math.Max(bestScore, newScore);
            }
            else
            {
                bestScore = Math.Min(bestScore, newScore);
            }
        }



        private int IsRoyalPiece(Move move)
        {
            if (move.MainMoveType != MoveType.Capture && move.MainMoveType != MoveType.CapturePromotion) return 0;

            int royalPieceDelta = 0;

            for (int i = 0; i < move.UpdatesCount; i++)
            {
                IMoveUpdate update = move.Updates[i]!;

                ICell cell = update.Cell;
                if (update.Old_Piece?.IsKingFigure ?? false)
                {
                    --royalPieceDelta;
                }
            }
            return royalPieceDelta;
        }

        public void SetDifficulty(Difficulty difficulty)
        {
            CurrentDifficulty= difficulty;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    MaxDeapth = 2;
                    break;
                case Difficulty.Medium:
                    MaxDeapth = 3;
                    break;
                case Difficulty.Hard:
                    MaxDeapth = 4;
                    break;
                case Difficulty.Extream:
                    MaxDeapth = 5;
                    break;
                default:
                    MaxDeapth = 0;
                    break;
            }
        }


        public void ClearAnalyzedBoardsCount() => AnalyzedBoardsCount= 0;

        public int GetMaximalScore(Color color) => color.GetOpositeColor().GetInitialScore();
    }
}

