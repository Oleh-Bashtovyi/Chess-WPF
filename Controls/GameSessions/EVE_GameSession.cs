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
    public class EVE_GameSession : AbstractGameSession
    {
        //=================================================================================================================
        #region AI SETTINGS PROPERTIES
        [JsonIgnore] private AI BlackSide_AI { get; set; } = new AI(Color.Black);
        [JsonIgnore] private AI WhiteSide_AI { get; set; } = new AI(Color.White);
        #endregion







        //=================================================================================================================
        // START/PAUSE/DISPOSE GAME
        //=================================================================================================================
        #region START GAME
        public override async Task StartGame(CancellationToken token)
        {
            if (IsTimerOn) StartTimer();

            if (IsFogOfWarOn) GUI_Handler?.Show_AllVisibleCells_ForPlayer(this, CurrentPlayerMove);

            IsGameStarted= true;

            try
            {
                while (!IsGameOver && IsGameStarted)
                {
                    await AI_Play(token);
                }
            }
            catch (OperationCanceledException)
            {
                StopTimer();
            }
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
        private void Internal_MakeMove(IMove? move, CancellationToken token)
        {
            if (!IsGameStarted || IsGameOver || move == null ) return;

            token.ThrowIfCancellationRequested();

            //Зробити хід
            Board.Make_Move(move);
            MovesLeft--;

            //Опрацювати усі списки (Список побитих фігур, undo, redo а також хеш список)
            Procces_AllLists_ForMove(move);

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
        #region AI
        private async Task AI_Play(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!IsGameOver && IsGameStarted)
            {
                await Task.Delay(100);
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
            WhiteSide_AI = new AI(Color.White);
            BlackSide_AI = new AI(Color.Black);
            WhiteSide_AI.SetDifficulty(settings.WhitesDifficulty);
            BlackSide_AI.SetDifficulty(settings.BlacksDifficulty);
            WhiteSide_AI.ClearAnalyzedBoardsCount();
            BlackSide_AI.ClearAnalyzedBoardsCount();
        }
        #endregion





        //=================================================================================================================
        #region GUI
        protected override void Update_GUI()
        {
            GUI_Handler?.Redraw_GameGrid();
            GUI_Handler?.Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(this);
            /*            if (CurrentGameBoardSettings.IsFogOfWarOn)
                        {
                            GUI_Handler.Show_AllVisibleCells_ForPlayer(this, CurrentPlayerMove);
                        }
                        else
                        {
                            GUI_Handler.Redraw_GameGrid();
                            GUI_Handler.Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(this);
                        }*/
            OnPropertyChanged(nameof(WhiteSide_Score));
            OnPropertyChanged(nameof(BlackSide_Score));
            OnPropertyChanged(nameof(AnalizedBoardsCount));
        }
        #endregion









        //=================================================================================================================
        #region CONSTRUCTOR

        //For quick creation
        public EVE_GameSession(byte rows = 8, byte cols = 8) : base(rows, cols)
        {
            AiSetup_Helper(CurrentGameBoardSettings);
        }


        //For hand creation
        public EVE_GameSession(GameBoard board, GameStartupSettings gameStartupSettings) : base(board, gameStartupSettings)
        {
            AiSetup_Helper(CurrentGameBoardSettings);
        }


        //For deteiled creation
        public EVE_GameSession
        (
        GameBoard board,
        GameStartupSettings gameStartupSettings,
        Color currentPlayerMove = Color.White,
        short movesLeft = -1,
        short whiteSideTimeLeft = -1,
        short blackSideTimeLeft = -1,
        (byte, byte)? cellThatMustBePromoted = null
        )
        : base(board, gameStartupSettings, currentPlayerMove, movesLeft, whiteSideTimeLeft, blackSideTimeLeft, cellThatMustBePromoted)
        {
            AiSetup_Helper(CurrentGameBoardSettings);
        }
        #endregion



        public override async Task<List<IMove>> Get_AllPossibleMoves_FromCell(GameCell cell)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return EmptyMoveList;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public override async Task Make_Promotion(ushort promoteTo_ID, CancellationToken token) => await Task.Delay(100);
        public override async Task Make_Move(Move move, CancellationToken token) => await Task.Delay(100);
    }
}
