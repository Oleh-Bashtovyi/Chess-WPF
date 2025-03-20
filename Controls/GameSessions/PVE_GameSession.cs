using Chess_game.Behaviour;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Extensions;
using Chess_game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Chess_game.Controls.GameSessions
{
    public class PVE_GameSession : AbstractGameSession
    {
        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================
        #region AI SETTINGS PROPERTIES
        [JsonIgnore] private AI BlackSide_AI { get; set; } = new AI(Color.Black);
        [JsonIgnore] private AI WhiteSide_AI { get; set; } = new AI(Color.White);
        [JsonIgnore] private Color AI_opponent { get; set; } = Color.White;
        #endregion






        //=================================================================================================================
        #region PROMOTION
        private void StartPromotion(IMove startedInMove)
        {
            Update_GUI();
            Sound_Handler?.PlaySound_UsingMoveType(this, startedInMove.MainMoveType); 
            GUI_Handler?.Show_PormotionScreen_ForColor(startedInMove.Executor_Color);
        }
        private bool CheckOnPromotion(IMove move)
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
                //IMove move = UndoList.Pop();
                IMove move = UndoList.Last();
                _undoList.RemoveLast();


                move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));

                if (move.MainMoveType == MoveType.Capture)
                {
                    move.MainMoveType = MoveType.CapturePromotion;
                }
                else move.MainMoveType = MoveType.Promotion;

                //UndoList.Push(move);
                _undoList.AddLast(move);
            }
            else
            {
                Move move = new Move(CurrentPlayerMove, promotedFromPiece, CurrentCellThatNustBePromoted!, CurrentCellThatNustBePromoted!, MoveType.Promotion);
                move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));
                //UndoList.Push(move);
                _undoList.AddLast(move);
            }
        }
        public override async Task Make_Promotion(ushort promoteTo_ID, CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                Internal_Make_Promotion(promoteTo_ID, token);
                AI_Play(token);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        private void Internal_Make_Promotion(ushort promoteTo_ID, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

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

                SwapPlayer();
                Update_GUI();
                OnPropertyChanged(nameof(UndoList));
                Sound_Handler?.PlaySound_UsingMoveType(this, MoveType.Promotion);
            }
        }
        #endregion




        //=================================================================================================================
        #region START/PAUSE/DISPOSE GAME
        public override async Task StartGame(CancellationToken token)
        {
            if (IsGameDisposed)
            {
                throw new InvalidOperationException("Game is already disposed!");
            }
            if (IsGameOver) return;

            if (IsFogOfWarOn)
            {
                await GUI_Handler!.Show_DelayScreen(this, token, "Starting game", 3);
            }

            if(IsTimerOn) StartTimer();

            IsGameStarted = true;

            //if pve and player play as black
            if (MainPlayerColor != CurrentPlayerMove)
            {
                //AI making first move
                try
                {
                    AI_Play(token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
            else if(IsFogOfWarOn) GUI_Handler?.Show_AllVisibleCells_ForPlayer(this, MainPlayerColor);


        }

        public override void PauseGame()
        {
            if (IsGameDisposed) throw new InvalidOperationException("Game is already disposed!");

            if (IsGameOver) return;

            StopTimer();
            IsGameStarted = false;
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
        #region MAKE MOVE
        public override async Task Make_Move(Move move, CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                Internal_MakeMove(move, token);
                AI_Play(token);
            }
            catch(OperationCanceledException) { }
            finally
            {
                semaphoreSlim.Release();
            }
        }


        private void Internal_MakeMove(IMove? move, CancellationToken token)
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
            if (CurrentPlayerMove == CurrentGameBoardSettings.MainPlayerColor && CheckOnPromotion(move))
            {
                StartPromotion(move);
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
            SwapPlayer();

            //оновити інтерфейс
            Update_GUI();
        }
        #endregion

        //=================================================================================================================
        // AI
        //=================================================================================================================
        #region AI
        private void AI_Play(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!IsGameOver && !IsPromotionNow && IsGameStarted)
            {
                if (CurrentPlayerMove == Color.White)
                {
                    Internal_MakeMove(WhiteSide_AI.Get_TheBestMove_ForColor(CurrentPlayerMove, Board.Clone(), token, IsKingInDangerShouldBeShown), token);
                }
                else if (CurrentPlayerMove == Color.Black)
                {
                    Internal_MakeMove(BlackSide_AI.Get_TheBestMove_ForColor(CurrentPlayerMove, Board.Clone(), token, IsKingInDangerShouldBeShown), token);
                }
                OnPropertyChanged(nameof(AnalizedBoardsCount));
            }
        }
        private void AiSetup_Helper(GameStartupSettings settings)
        {
            AI_opponent = settings.MainPlayerColor.GetOpositeColor();
            WhiteSide_AI = new AI(Color.White);
            BlackSide_AI = new AI(Color.Black);
            WhiteSide_AI.SetDifficulty(settings.WhitesDifficulty);
            BlackSide_AI.SetDifficulty(settings.BlacksDifficulty);
            WhiteSide_AI.ClearAnalyzedBoardsCount();
            BlackSide_AI.ClearAnalyzedBoardsCount();
        }
        #endregion

        //=================================================================================================================
        // GET MOVES
        //=================================================================================================================
        #region GET MOVES
        public override async Task<List<IMove>> Get_AllPossibleMoves_FromCell(GameCell cell)
        {
            //семафор потрібний для синхронізації процесів
            await semaphoreSlim.WaitAsync();
            try
            {
                //check possibility to get move
                if (IsGameOver || IsPromotionNow ||
                    cell == null || cell.Piece == null ||
                    cell.Piece.PieceColor != CurrentGameBoardSettings.MainPlayerColor)
                {
                    return new List<IMove>();
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
        // TIMER
        //=================================================================================================================
        #region TIMER
        protected override void CountdownTimerTick(object? sender, EventArgs? e)
        {
            if (CurrentPlayerMove == Color.White || (AI_opponent == Color.Black && _redoList.Count > 0))
            {
                if (AI_opponent == Color.White && _redoList.Count > 0) return;

                TimeLeftForWhite--;
                if (TimeLeftForWhite <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.Black_Win, $"Unforunately time ended up for WHITE side.\nSo, the winner is BLACK side!");
                    FinishGame();
                }
            }
            else if (CurrentPlayerMove == Color.Black || (AI_opponent == Color.White && _redoList.Count > 0))
            {
                if (AI_opponent == Color.Black && _redoList.Count > 0) return;

                TimeLeftForBlack--;
                if (TimeLeftForBlack <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.White_Win, $"Unforunately time ended up for BLACK side.\nSo, the winner is WHITE side!");
                    FinishGame();
                }
            }
        }
        #endregion


        //=================================================================================================================
        #region GUI
        protected override void Update_GUI()
        {
            if (IsFogOfWarOn)
            {
                if (!IsGameOver)
                {
                    GUI_Handler.Show_AllVisibleCells_ForPlayer(this, CurrentGameBoardSettings.MainPlayerColor);
                }
                else
                {
                    GUI_Handler.Redraw_GameGrid();
                    GUI_Handler.Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(this);
                }
            }
            else
            {
                GUI_Handler.Redraw_GameGrid();
                GUI_Handler.Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(this);
            }
            OnPropertyChanged(nameof(WhiteSide_Score));
            OnPropertyChanged(nameof(BlackSide_Score));
            OnPropertyChanged(nameof(AnalizedBoardsCount));
        }
        #endregion



        //=================================================================================================================
        #region CONSTRUCTOR

        //For quick creation
        public PVE_GameSession(byte rows = 8, byte cols = 8) : base(rows, cols)
        {
        }

        //For more deteiled creation
        public PVE_GameSession(GameBoard board, GameStartupSettings gameStartupSettings) : base(board, gameStartupSettings)
        {
        }

        //For deteiled creation
        // For deteiled creation
        public PVE_GameSession
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
            AiSetup_Helper(gameStartupSettings);
        }
        #endregion
    }
}
