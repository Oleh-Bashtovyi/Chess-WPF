using Chess_game.Behaviour;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Chess_game.Controls.GameSessions
{
    public class PVP_GameSession : AbstractGameSession
    {

        //=================================================================================================================
        // COLORS (players)
        //=================================================================================================================
        #region COLORS
        protected async Task SwapPlayer_WithDelay(CancellationToken token)
        {
            SwapPlayer();
            await (GUI_Handler?.Show_DelayScreen(this, token, "Changing player view", 3) ?? Task.Delay(100));
        }


        protected async Task SwapPlayer_WithDelay_IfFogOfWarOn(CancellationToken token)
        {
            if (CurrentGameBoardSettings.IsFogOfWarOn)
            {
                Update_GUI();
                await SwapPlayer_WithDelay(token);
            }
            else SwapPlayer();
        }
        #endregion






        //=================================================================================================================
        #region PROMOTION
        public override async Task Make_Promotion(ushort promoteTo_ID, CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await Internal_MakePromotion(promoteTo_ID, token);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        private async Task Internal_MakePromotion(ushort promoteTo_ID, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (IsGameOver || !IsGameStarted) return;

            if (IsPromotionNow && Board.MakePromotion(promoteTo_ID, out IPlayableChessPiece? promotedFromPiece))
            {

                Edit_LastMove_ForPromotion(promotedFromPiece!);
                IsPromotionNow = false;
                Board.CurrentCellThatNustBePromoted = null;
                GUI_Handler?.Hide_PromotionScreen();

                //Перевірити умови кінця гри, якщо кінець то вивести повідомлення
                if (CheckEndGame())
                {
                    FinishGame();
                    return;
                }

                Sound_Handler?.PlaySound_UsingMoveType(this, MoveType.Promotion);

                if (CurrentGameBoardSettings.IsFogOfWarOn)
                {
                    Update_GUI();
                    await SwapPlayer_WithDelay(token);
                }
                else SwapPlayer();
                OnPropertyChanged(nameof(UndoList));
                Update_GUI();
            }
        }

        private void Start_Promotion(IMove startedInMove)
        {
            Update_GUI();
            Sound_Handler?.PlaySound_UsingMoveType(this, startedInMove.MainMoveType);
            GUI_Handler?.Show_PormotionScreen_ForColor(startedInMove.Executor_Color);
        }
        private bool Check_OnPromotion(IMove move)
        {
            if (Board.Check_OnPromotion(move, out IMoveUpdate? moveUpdate))
            {
                IsPromotionNow = true;
                Board.CurrentCellThatNustBePromoted = moveUpdate!.Cell;
                GUI_Handler?.Show_PormotionScreen_ForColor(move.Executor_Color);
                return true;
            }
            return false;
        }
        private void Edit_LastMove_ForPromotion(IPlayableChessPiece promotedFromPiece)
        {

            //change last move
            if (_undoList.Count > 0 && _undoList.Last().Executor_Color == CurrentPlayerMove)
            {
                IMove move = UndoList.Last();
                _undoList.RemoveLast();


                move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));

                if (move.MainMoveType == MoveType.Capture)
                {
                    move.MainMoveType = MoveType.CapturePromotion;
                }
                else move.MainMoveType = MoveType.Promotion;

                _undoList.AddLast(move);
            }
            else
            {
                Move move = new Move(CurrentPlayerMove, promotedFromPiece, CurrentCellThatNustBePromoted!, CurrentCellThatNustBePromoted!, MoveType.Promotion);
                move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));
                _undoList.AddLast(move);
            }
        }
        #endregion



        //=================================================================================================================
        #region MAKE MOVE
        public override async Task Make_Move(Move move, CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await Internal_MakeMove(move, token);
            }
            catch (OperationCanceledException){ }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        private async Task Internal_MakeMove(IMove? move, CancellationToken token)
        {
            if (!IsGameStarted || IsPromotionNow || IsGameOver || move == null) return;

            token.ThrowIfCancellationRequested();

            //Зробити хід
            Board.Make_Move(move);
            MovesLeft--;

            //Опрацювати усі списки (Список побитих фігур, undo, redo а також хеш список)
            Procces_AllLists_ForMove(move);

            //перевірити чи потрібно перетворювати фігуру після виконання ходу
            //перетворення відбувається раніше, ніж перевірка на кінець гри, 
            //бо гравець повинен обовязково зробити перетворення
            if (Check_OnPromotion(move))
            {
                Start_Promotion(move);
                return;
            }

            //Перевірити умови кінця гри, якщо кінець то вивести повідомлення
            if (CheckEndGame())
            {
                FinishGame();
                return;
            }

            //якщо не було ні кінця гри ні перетворення фігури, то програємо звук і змінюємо гравця
            Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);

            //зробити затримку, якщо потрібно, перед зміною гравця (використовується в ПвП в тумані війни)
            await SwapPlayer_WithDelay_IfFogOfWarOn(token);

            //оновити інтерфейс
            Update_GUI();
        }
        #endregion




        //=================================================================================================================
        #region CONSTRUCTOR

        //For quick creation
        public PVP_GameSession(byte rows = 8, byte cols = 8) : base(rows, cols)
        {
        }

        //For more deteiled creation
        public PVP_GameSession(GameBoard board, GameStartupSettings gameStartupSettings) : base(board, gameStartupSettings)
        {
        }

        //For deteiled creation
        public PVP_GameSession
        (
        GameBoard board,
        GameStartupSettings gameStartupSettings,
        Color currentPlayerMove = Color.White,
        short movesLeft = -1,
        short whiteSideTimeLeft = -1,
        short blackSideTimeLeft = -1,
        (byte, byte)? cellThatMustBePromoted = null

        ) : base(board, gameStartupSettings, currentPlayerMove, movesLeft, whiteSideTimeLeft, blackSideTimeLeft, cellThatMustBePromoted)
        {
        }
        #endregion





        //=================================================================================================================
        #region MOVES
        public override async Task<List<IMove>> Get_AllPossibleMoves_FromCell(GameCell cell)
        {
            //семафор потрібний для синхронізації процесів
            await semaphoreSlim.WaitAsync();
            try
            {
                //check possibility to get move
                if (!IsGameStarted || IsGameOver || IsPromotionNow ||
                    cell == null || cell.Piece == null ||
                    !cell.Piece.Has_SameColor_With(CurrentPlayerMove))
                {
                    return EmptyMoveList;
                }

                //Якщо треба лише ходи, які не загрожуватимуть королю
                else if (IsKingInDangerShouldBeShown)
                {
                    return Board.Get_AllPossibleMoves_WithKingsFiguresProtection_ForCell(cell);
                }
                else return Board.Get_AllPossibleMoves_ForCell(cell);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        #endregion







        //=================================================================================================================
        #region START/PAUSE/DISPOSE GAME
        public override void PauseGame()
        {
            if(IsGameDisposed) throw new InvalidOperationException("Game is already disposed!");

            if (IsGameOver) return;

            StopTimer();
            IsGameStarted = false;
        }



        public override async Task StartGame(CancellationToken token)
        {
            if (IsGameDisposed)
            {
                throw new InvalidOperationException("Game is already disposed!");
            }
            if(IsGameOver) return;  

            if (CurrentGameBoardSettings.IsFogOfWarOn)
            {
                await (GUI_Handler?.Show_DelayScreen(this, token, "Starting game", 3) ?? Task.Delay(100));
                GUI_Handler?.Show_AllVisibleCells_ForPlayer(this, CurrentPlayerMove);
            }

            if (IsTimerOn) StartTimer();

            IsGameStarted = true;
        }


        public override void DisposeGame()
        {
            StopTimer();
            Unsubscribe_Handlers();
            Clear_Redo_And_Undo_Lists();
            IsGameDisposed = true;
            IsGameStarted = false;
        }
        #endregion


        //=================================================================================================================
        #region GUI
        protected override void Update_GUI()
        {
            if (CurrentGameBoardSettings.IsFogOfWarOn && !IsGameOver)
            {
                GUI_Handler?.Show_AllVisibleCells_ForPlayer(this, CurrentPlayerMove);
            }
            else
            {
                GUI_Handler?.Redraw_GameGrid();
                GUI_Handler?.Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(this);
            }
            OnPropertyChanged(nameof(WhiteSide_Score));
            OnPropertyChanged(nameof(BlackSide_Score));

        }
        #endregion






    }
}
