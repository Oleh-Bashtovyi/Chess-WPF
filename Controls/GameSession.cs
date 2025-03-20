/*using Chess_game.Behaviour;
using Chess_game.Behaviour.Commands.check_game_over_commands;
using Chess_game.Behaviour.Decorators.Get_Moves_Decorator;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls.Handlers;
using Chess_game.Extensions;
using Chess_game.Factories;
using Chess_game.Game_Data;
using Chess_game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Chess_game.Controls
{
    public class GameSession : INotifyPropertyChanged, IGameSession
    {
        //=================================================================================================================
        // FIELDS
        //=================================================================================================================
        #region FIELDS
        private int _movesLeft = 1000;
        private bool _isGameOver = false;
        private int _timeLeftForBlack = 600;
        private int _timeLeftForWhite = 600;
        private Color _currentPlayerMove = Color.None;
        private static readonly Random Random = new Random();
        private static readonly ushort CollectionLimit = 1001;




        *//*        private Stack<IMove> _undoList = new();
                private Stack<IMove> _redoList = new();*//*

        private LinkedList<IMove> _undoList = new();
        private LinkedList<IMove> _redoList = new();
        #endregion


        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================
        #region HANDLERS
        [JsonIgnore] private GUI_Handler GUI_Handler { get; } = GUI_Handler.Get_Instance();
        [JsonIgnore] private Sound_Handler Sound_Handler { get; } = Sound_Handler.Get_Instance();
        [JsonIgnore] private Notification_Handler Notification_Handler { get; } = Notification_Handler.Get_Instance();
        #endregion

        #region BEATEN PIECES PROPERTIES
        [JsonIgnore] public ObservableCollection<IPlayableChessPiece> BeatenPieces_White { get; } = new();
        [JsonIgnore] public ObservableCollection<IPlayableChessPiece> BeatenPieces_Black { get; } = new();
        #endregion

        #region TIMER PROPERTIES
        [JsonIgnore] private DispatcherTimer Timer { get; set; }
        public int TimeLeftForBlack 
        {
            get
            {
                return _timeLeftForBlack;
            }
            private set
            {
                if (value < 0) _timeLeftForBlack = 0;
                else if (value > 9999) _timeLeftForBlack = 9999;
                else _timeLeftForBlack = value;
                OnPropertyChanged();
            }
        }
        public int TimeLeftForWhite
        {
            get
            {
                return _timeLeftForWhite;
            }
            private set
            {
                if (value < 0) _timeLeftForWhite = 0;
                else if (value > 9999) _timeLeftForWhite = 9999;
                else _timeLeftForWhite = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region COLOR PROPERTIES
        [JsonIgnore] public string CurrentPlayerImageSource => $"pack://application:,,,/Assets/{CurrentPlayerMove}_King.png";
        public Color CurrentPlayerMove
        {
            get { return _currentPlayerMove; }
            private set
            {
                _currentPlayerMove = value;
                OnPropertyChanged(nameof(CurrentPlayerImageSource));
            }
        }
        #endregion

        #region AI SETTINGS PROPERTIES
        [JsonIgnore] private AI BlackSide_AI { get; set; } = new AI(Color.Black);
        [JsonIgnore] private AI WhiteSide_AI { get; set; } = new AI(Color.White);
        [JsonIgnore] public bool IsWhiteAI_Playing { get; private set; } = false;
        [JsonIgnore] public bool IsBlackAI_Playing { get; private set; } = false;
        *//*            (CurrentGameBoardSettings.GameMode == GameModeType.PvE ||
                    (CurrentGameBoardSettings.MainPlayerColor == Color.Black && CurrentGameBoardSettings.GameMode == GameModeType.PvE));*//*
        #endregion

        #region BOARD PROPERTIES
        //READONLY
        //=================================================================================================================
        [JsonIgnore] public byte Columns => Board.Columns;
        [JsonIgnore] public byte Rows => Board.Rows;
        [JsonIgnore] public int WhiteSide_Score => Board.Estimate_ScoreOfCurrentBoard_ForColor(Color.White);
        [JsonIgnore] public int BlackSide_Score => Math.Abs(Board.Estimate_ScoreOfCurrentBoard_ForColor(Color.Black));
        [JsonIgnore] public ulong AnalizedBoardsCount => AI.AnalyzedBoardsCount;
        [JsonIgnore] public bool IsTimerOn => CurrentGameBoardSettings.IsTimerOn;
        [JsonIgnore] public bool IsMovesLimitOn => CurrentGameBoardSettings.IsMovesLimitOn;
        [JsonIgnore] public bool IsKingInDangerShouldBeShown => !CurrentGameBoardSettings.IsFogOfWarOn;
        [JsonIgnore] public Color MainPlayerColor => CurrentGameBoardSettings.MainPlayerColor;
        [JsonIgnore] public GameModeType CurrentGameMode => CurrentGameBoardSettings.GameMode;


        //MAIN PROPERTIES
        //=================================================================================================================
        public GameStartupSettings CurrentGameBoardSettings { get; private set; } = new();
        public GameBoard Board { get; private set; }
        #endregion


        #region UNDO/REDO
        *//*        public Stack<IMove> UndoList
                {
                    get { return _undoList; }
                    set { _undoList = value; OnPropertyChanged(nameof(UndoList)); }
                }
                [JsonIgnore]
                public Stack<IMove> RedoList { 
                    get{return _redoList;} 
                    set { _redoList = value; OnPropertyChanged(nameof(RedoList)); }
                }*/
        /*        public ObservableLinkedList<IMove> UndoList
                {
                    get { return _undoList; }
                    set { _undoList = value; OnPropertyChanged(nameof(UndoList)); }
                }
                [JsonIgnore]
                public ObservableLinkedList<IMove> RedoList
                {
                    get { return _redoList; }
                    set { _redoList = value; OnPropertyChanged(nameof(RedoList)); }
                }*//*

        [JsonIgnore]
        public IEnumerable<IMove> UndoList => _undoList.ToList();
        [JsonIgnore]
        public IEnumerable<IMove> RedoList => _redoList.ToList();
        #endregion


        #region GAME PROCCES PROPERTIES

        [JsonIgnore] private LinkedList<uint> _undo_hashList= new LinkedList<uint>();
        [JsonIgnore] public LinkedList<uint> Undo_BoardHashList
        {
            get
            {
                return _undo_hashList;
            }
            private set
            {
                _undo_hashList = value;
                OnPropertyChanged(nameof(Undo_BoardHashList));
                OnPropertyChanged();
            }
        }


        //redo lists
        [JsonIgnore] public LinkedList<uint> Redo_BoardHashList { get; private set; } = new LinkedList<uint>();


        //important
        [JsonRequired] private List<ICheckGameOverCommand> GameOverCommands { get; set; } = new List<ICheckGameOverCommand>();
        [JsonIgnore] public ICell? CurrentCellThatNustBePromoted => Board?.CurrentCellThatNustBePromoted;
        [JsonIgnore] private ZobristHashGenerator? ZobristHashGenerator { get; set; }


        [JsonIgnore] public bool IsPromotionNow
        {
            get
            {
                return Board.IsPromotionNow;
            }
            private set
            {
                Board.IsPromotionNow = value;
                OnPropertyChanged(nameof(IsPromotionNow));
            }
        }
        [JsonIgnore] public bool IsGameOver
        {
            get { return _isGameOver; }
            set 
            {
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver)); 
            }
        }
        public EndGameInfo EndGameInfo { get; set; } = new EndGameInfo(GameOverType.None, "In procces");
        public int MovesLeft
        {
            get { return _movesLeft; }
            private set
            {
                if (value > 1000) _movesLeft = 1000;
                else if(value < 0) _movesLeft = 0;
                else _movesLeft= value;
                OnPropertyChanged(nameof(MovesLeft));
            }
        }


        //для сихронізації процесів
        [JsonIgnore] private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        #endregion






        //=================================================================================================================
        // MAKE MOVE
        //=================================================================================================================
        #region MAKE MOVE
        public async Task Make_Move(Move move, CancellationToken token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (IsPromotionNow || IsGameOver || move == null) return;
                await Internal_MakeMove(move, token);
                await AI_Play(token);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }



        private async Task Internal_MakeMove(IMove? move, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if(move == null) return;

            //Зробити хід
            Board.Make_Move(move);
            MovesLeft--;

            //Опрацювати усі списки (Список побитих фігур, undo, redo а також хеш список)
            ProccesAllLists(move);

            //перевірити чи потрібно перетворювати фігуру після виконання ходу
            //перетворення відбувається раніше, ніж перевірка на кінець гри, 
            //бо гравець повинен обовязково зробити перетворення
            if (CheckOnPromotion(move))
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
            await SwapPlayer_WithDelay_IfNeed(token);

            //оновити інтерфейс
            Update_GUI();
        }
        #endregion

        //=================================================================================================================
        // UNDO/REDO
        //=================================================================================================================
        #region UNDO/REDO
        public void Undo()
        {
            *//*            if (UndoList.Count > 0 && !IsPromotionNow)
                        {
                            *//*                IMove move = UndoList.Pop();
                                            RedoList.Push(move);*//*
                            IMove move = UndoList.Last();
                            UndoList.RemoveLast();
                            RedoList.AddLast(move);

                            Board.Undo_Move(move);
                            MovesLeft++;

                            RemoveBeatenPiecesInMoveInList(move);
                            UndoHashList();

                            SetPlayer(move.Executor_Color);
                            Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
                        }
                        Update_GUI();*//*
            if (_undoList.Count > 0 && !IsPromotionNow)
            {
                *//*                IMove move = UndoList.Pop();
                                RedoList.Push(move);*//*
                IMove move = _undoList.Last();
                _undoList.RemoveLast();
                _redoList.AddLast(move);

                Board.Undo_Move(move);
                MovesLeft++;

                RemoveBeatenPiecesInMoveInList(move);
                UndoHashList();

                Set_CurrentPlayer_As(move.Executor_Color);
                OnPropertyChanged(nameof(UndoList));
                Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
            }
            Update_GUI();
        }
        public void Redo()
        {
            *//*            if (RedoList.Count > 0 && !IsPromotionNow)
                        {
                            *//*                IMove move = RedoList.Pop();
                                            UndoList.Push(move);*//*

                            IMove move = RedoList.Last();
                            RedoList.RemoveLast();
                            UndoList.AddLast(move);

                            Board.Make_Move(move);
                            MovesLeft--;

                            Add_BeatenPiecesInMove_ToList(move);
                            RedoHashList();

                            if (!IsPromotionNow) SwapPlayer();

                            Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
                        }
                        Update_GUI();*//*
            if (_redoList.Count > 0 && !IsPromotionNow)
            {
                *//*                IMove move = RedoList.Pop();
                                UndoList.Push(move);*//*

                IMove move = _redoList.Last();
                _redoList.RemoveLast();
                _undoList.AddLast(move);

                Board.Make_Move(move);
                MovesLeft--;

                Add_BeatenPiecesInMove_ToList(move);
                RedoHashList();

                if (!IsPromotionNow) SwapPlayer();
                OnPropertyChanged(nameof(UndoList));
                Check_CollectionLimit(RedoList, nameof(RedoList));
                Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
            }
            Update_GUI();
        }




        private void Add_Move_ToUndoList_And_Clear_RedoList(IMove move)
        {
            *//*            if (RedoList.Count > 0) RedoList.Clear();
                        //UndoList.Push(move);
                        UndoList.AddLast(move);

                        OnPropertyChanged(nameof(UndoList));*//*

            if (_redoList.Count > 0) _redoList.Clear();
            _undoList.AddLast(move);

            OnPropertyChanged(nameof(UndoList));
            Check_CollectionLimit(UndoList, nameof(UndoList));
        }



        #endregion


        //=================================================================================================================
        // PROMOTION
        //=================================================================================================================
        #region PROMOTION
        public async Task Make_Promotion(ushort promoteTo_ID, CancellationToken token)
        {
            await Internal_Make_Promotion(promoteTo_ID, token);
            await AI_Play(token);
        }


        private async Task Internal_Make_Promotion(ushort promoteTo_ID, CancellationToken token)
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

                Sound_Handler?.PlaySound_UsingMoveType(this, MoveType.Promotion);
                await SwapPlayer_WithDelay_IfNeed(token);
                Update_GUI();
            }
        }


        private bool CheckOnPromotion(IMove move)
        {
            if (Board.Check_OnPromotion(move, out IMoveUpdate? moveUpdate))
            {
                IsPromotionNow = true;
                Board.CurrentCellThatNustBePromoted = moveUpdate!.Cell;
                GUI_Handler.Show_PormotionScreen_ForColor(move.Executor_Color);
                return true;
            }
            return false;
        }



        private void Edit_LastMove_ForPromotion(IPlayableChessPiece promotedFromPiece)
        {

            *//*            //change last move
                        if (UndoList.Count > 0 && UndoList.Last().Executor_Color == CurrentPlayerMove)
                        {
                            //IMove move = UndoList.Pop();
                            IMove move = UndoList.Last();
                            UndoList.RemoveLast();


                            move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));

                            if (move.MainMoveType == MoveType.Capture)
                            {
                                move.MainMoveType = MoveType.CapturePromotion;
                            }
                            else move.MainMoveType = MoveType.Promotion;

                            //UndoList.Push(move);
                            UndoList.AddLast(move);
                        }
                        else
                        {
                            Move move = new Move(CurrentPlayerMove, promotedFromPiece, CurrentCellThatNustBePromoted!, CurrentCellThatNustBePromoted!, MoveType.Promotion);
                            move.Add_Update(new MoveUpdate(CurrentCellThatNustBePromoted!, promotedFromPiece, CurrentCellThatNustBePromoted!.Piece, MoveType.Promotion));
                            //UndoList.Push(move);
                            UndoList.AddLast(move);
                        }*//*

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



        private void StartPromotion(IMove startedInMove)
        {
            Update_GUI();
            Sound_Handler?.PlaySound_UsingMoveType(this, startedInMove.MainMoveType);
            GUI_Handler.Show_PormotionScreen_ForColor(startedInMove.Executor_Color);
        }


        #endregion




        //=================================================================================================================
        // HASH LISTS
        //=================================================================================================================
        #region HASH LISTS
        private void UndoHashList()
        {
            if (Undo_BoardHashList.Count > 0)
            {
                uint lasHash = Undo_BoardHashList.Last();
                //Undo_BoardHashList.RemoveAt(Undo_BoardHashList.Count-1);
                Undo_BoardHashList.RemoveLast();
                Redo_BoardHashList.AddFirst(lasHash);
            }
            else if (Redo_BoardHashList.Count > 0) Redo_BoardHashList.Clear();
        }

        private void RedoHashList()
        {
            if (Redo_BoardHashList.Count > 0)
            {
                uint lasHash = Redo_BoardHashList.First();
                Redo_BoardHashList.RemoveFirst();
                //Undo_BoardHashList.Add(lasHash);
                Undo_BoardHashList.AddLast(lasHash);
            }
            else Undo_BoardHashList.AddLast(ZobristHashGenerator?.GetHashForBoard(Board.GameGrid) ?? 0);
            //else Undo_BoardHashList.Add(ZobristHashGenerator?.GetHashForBoard(GameBoard.Board) ?? 0);
        }

        private void Add_BoardHash_ToList(IMove move)
        {
            if (Undo_BoardHashList.Count == 0)
            {
                Undo_BoardHashList.AddLast(ZobristHashGenerator?.GetHashForBoard(Board.GameGrid) ?? 0);
                //Undo_BoardHashList.Add(ZobristHashGenerator?.GetHashForBoard(GameBoard.Board) ?? 0);
            }
            else if ((move.MainMoveType == MoveType.Capture || move.MainMoveType == MoveType.CapturePromotion))
            {
                uint oldHash = Undo_BoardHashList.Last();
                Undo_BoardHashList.Clear();
                Undo_BoardHashList.AddLast(ZobristHashGenerator?.GetHashForBoardUsingMove(oldHash, Board.GameGrid, move) ?? 0);
                //Undo_BoardHashList.Add(ZobristHashGenerator?.GetHashForBoardUsingMove(oldHash, GameBoard.Board, move) ?? 0);
            }
            else
            {
                //Undo_BoardHashList.Add(ZobristHashGenerator?.GetHashForBoardUsingMove(Undo_BoardHashList.Last(), GameBoard.Board, move) ?? 0);
                Undo_BoardHashList.AddLast(ZobristHashGenerator?.GetHashForBoardUsingMove(Undo_BoardHashList.Last(), Board.GameGrid, move) ?? 0);
            }
        }
        #endregion










        //=================================================================================================================
        // RESTART
        //=================================================================================================================
        #region RESTART AND START GAME
        public async Task StartCurrentGame(CancellationToken token)
        {
            token.Register(token.ThrowIfCancellationRequested); //???????????????????????

            if (IsTimerOn) StartTimer();

            if (CurrentGameBoardSettings.IsFogOfWarOn) GUI_Handler?.Show_VisibleCells_IfFogOfWarOn_Or_ShowAllCells(this);

            //if pve and player play as black
            if (CurrentGameMode == GameModeType.PvE && MainPlayerColor != CurrentPlayerMove)
            {
                //AI making first move
                await AI_Play(token);

            }
            else if(CurrentGameMode == GameModeType.EvE)
            {
                DispatcherTimer timer = Timer;
                try
                {
                    while (!IsGameOver)
                    {
                        await AI_Play(token);
                    }
                }
                catch (OperationCanceledException)
                {
                    timer.Stop();
                    token.ThrowIfCancellationRequested();
                }
            }
        }







*//*        private void Setup_GetMovesDecorators()
        {
            Board.GetMoves_Decorators.Clear();

            if(!CurrentGameBoardSettings.IsFogOfWarOn)
            {
                Board.GetMoves_Decorators.Add(new Delete_MovesThatWillNotProtectKing_Decorator());
            }

        }*//*




        public void DisposeGame()
        {
            Timer.Stop();
            Timer.Tick -= CountdownTimerTick;
        }


       


        #endregion





















        //=================================================================================================================
        // TIMER
        //=================================================================================================================
        #region TIMER
        public void StartTimer() => Timer?.Start();
        public void StopTimer() => Timer?.Start();
        


        *//*private void CountdownTimerTick(object? sender, EventArgs? e)
        {
            if (CurrentPlayerMove == Color.White || (IsBlackAI_Playing && RedoList.Count > 0))
            {
                if (IsWhiteAI_Playing && RedoList.Count > 0) return;

                TimeLeftForWhite--;
                if (TimeLeftForWhite <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.Black_Win, $"Unforunately time ended up for WHITE side.\nSo, the winner is BLACK side!");
                    FinishGame();
                }
            }
            else if (CurrentPlayerMove == Color.Black || (IsWhiteAI_Playing && RedoList.Count > 0))
            {
                if (IsBlackAI_Playing && RedoList.Count > 0) return;

                TimeLeftForBlack--;
                if (TimeLeftForBlack <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.White_Win, $"Unforunately time ended up for BLACK side.\nSo, the winner is WHITE side!");
                    FinishGame();
                }
            }
        }*//*
        private void CountdownTimerTick(object? sender, EventArgs? e)
        {
            if (CurrentPlayerMove == Color.White || (IsBlackAI_Playing && _redoList.Count > 0))
            {
                if (IsWhiteAI_Playing && _redoList.Count > 0) return;

                TimeLeftForWhite--;
                if (TimeLeftForWhite <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.Black_Win, $"Unforunately time ended up for WHITE side.\nSo, the winner is BLACK side!");
                    FinishGame();
                }
            }
            else if (CurrentPlayerMove == Color.Black || (IsWhiteAI_Playing && _redoList.Count > 0))
            {
                if (IsBlackAI_Playing && _redoList.Count > 0) return;

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
        // ENDGAME
        //=================================================================================================================
        #region ENDGAME
        private bool CheckEndGame()
        {
            GameBoard boardCopy = Board.Clone();
            foreach (var gameOverCommand in GameOverCommands)
            {
                //EndGameInfo = gameOverCommand.CheckGameOver(this, boardCopy);!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (EndGameInfo != EndGameInfo.GameContinues)
                {
                    return true;
                }
            }
            return false;
        }

        private void FinishGame()
        {
            Timer.Stop();
            IsGameOver = true;
            IsPromotionNow = false;
            Board.CurrentCellThatNustBePromoted = null;

            Update_GUI();
            Sound_Handler?.PlayEndGameSound();
            Notification_Handler?.NotifyUser_AboutGameOver();
        }
        #endregion

        //=================================================================================================================
        // SETINGS AND NEW GAME INSTALATION HELPERS
        //=================================================================================================================
        #region SETINGS AND NEW GAME INSTALATION HELPERS

        public void InstallSettings
        (
        GameStartupSettings settings,
        Color currentPlayerMove = Color.White,
        short movesLeft = -1,
        short whiteSideTimeLeft = -1,
        short blackSideTimeLeft = -1,
        (byte, byte)? cellThatMustBePromoted = null
        )
        {
            //validation
            CheckValidation_Helper(settings);
            //colors
            MainPlayerSetup_Helper(settings);
            SetupCurrentPlayerColor(currentPlayerMove);

            //game settings
            GameModeSetup_Helper(settings);
            TimerSetup_Helper(settings, whiteSideTimeLeft, blackSideTimeLeft);
            MovesLimitSetup_Helper(settings, movesLeft);
            AiSetup_Helper(settings);

            //gameplay
            SetupCellThatMustBePromoted_Helper(cellThatMustBePromoted);
            GameOverCommandsSetup_Helper(settings);
            GeneralSettingsSetup_Helper(settings);
            ZobristHashInstalation_Helper();
        }


        //settings helpers
        private void CheckValidation_Helper(GameStartupSettings settings)
        {
            if (settings.MainPlayerColor == Color.None)
            {
                throw new ArgumentException("Main player color can not be NONE!");
            }
            if (settings.GameMode == GameModeType.None)
            {
                throw new ArgumentException("Game modetype can not be NONE!");
            }
        }
        private void GameModeSetup_Helper(GameStartupSettings settings)
        {
            if (settings.GameMode == GameModeType.PvP)
            {
                IsBlackAI_Playing = false;
                IsWhiteAI_Playing = false;
            }
            else if (settings.GameMode == GameModeType.PvE)
            {
                IsBlackAI_Playing = (MainPlayerColor == Color.White);
                IsWhiteAI_Playing = (MainPlayerColor == Color.Black);
            }
            else if (settings.GameMode == GameModeType.EvE)
            {
                IsBlackAI_Playing = true;
                IsWhiteAI_Playing = true;
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
        private void GameOverCommandsSetup_Helper(GameStartupSettings settings)
        {
            GameOverCommands = (settings.IsFogOfWarOn) ? CheckGameOverCommands_Factory.GetFogOfWarGameGameOverCommands() : CheckGameOverCommands_Factory.GetClassicGameGameOverCommands();
        }
        private void GeneralSettingsSetup_Helper(GameStartupSettings settings)
        {
            EndGameInfo = EndGameInfo.GameContinues;
            CurrentGameBoardSettings = settings;
        }
        public void UpdateGUIForNewGame()
        {
            OnPropertyChanged(nameof(AnalizedBoardsCount));
            OnPropertyChanged(nameof(WhiteSide_Score));
            OnPropertyChanged(nameof(BlackSide_Score));
            OnPropertyChanged(nameof(UndoList));
        }

        //updated
        private void MainPlayerSetup_Helper(GameStartupSettings settings)
        {
            if (settings.MainPlayerColor == Color.Random)
            {
                Color[] arr = new Color[2] { Color.Black, Color.White };
                int temp = Random.Next(0, 1000) % 2;
                settings.MainPlayerColor = arr[temp];
            }
            else if (settings.MainPlayerColor == Color.None) throw new ArgumentException("Main player color could not be NONE!");
        }
        private void SetupCurrentPlayerColor(Color color) => Set_CurrentPlayer_As(color);
        private void TimerSetup_Helper(GameStartupSettings settings, short whiteSideTimeLeft = -1, short blackSideTimeLeft = -1)
        {
            //Установка часу для сторін
            if (settings.IsTimerOn)
            {
                //якщо передані значення менше за 0, то беремо стандартне значення з settings, інакше присвоюємо 
                TimeLeftForWhite = (whiteSideTimeLeft < 0) ? settings.WhitesTimerDuration : Math.Min(whiteSideTimeLeft, settings.WhitesTimerDuration);
                TimeLeftForBlack = (blackSideTimeLeft < 0) ? settings.BlacksTimerDuration : Math.Min(blackSideTimeLeft, settings.BlacksTimerDuration);
            }
            else
            {
                //якщо ліміту часу не вказано
                TimeLeftForWhite = 0;
                TimeLeftForBlack = 0;
            }
        }
        private void MovesLimitSetup_Helper(GameStartupSettings settings, short movesLeft = -1)
        {
            MovesLeft = (movesLeft < 0) ? settings.MovesLimit : Math.Min(movesLeft, settings.MovesLimit);
        }
        private void SetupCellThatMustBePromoted_Helper((byte, byte)? cellThatMustBePromoted)
        {
            if
                (
                cellThatMustBePromoted != null &&
                Board.TryGet_Cell_AtPosition(out ICell? cell, cellThatMustBePromoted.Value.Item1, cellThatMustBePromoted.Value.Item2) &&
                cell!.IsPromotionCell &&
                cell.Piece != null &&
                cell.Piece.IsPromotable &&
                PromotionLists_Factory.PromotionIDList_To_PromotionChessPiecesIDList.ContainsKey(cell.Piece.PromotionListID)
                )
            {
                IsPromotionNow = true;
                Board.CurrentCellThatNustBePromoted = cell;
            }
        }
        private void ZobristHashInstalation_Helper()
        {
            if(ZobristHashGenerator == null)
            {
                //hash initialization
                ZobristHashGenerator = new ZobristHashGenerator(Rows, Columns, Board.GameGrid);
                Undo_BoardHashList.AddLast(ZobristHashGenerator.GetHashForBoard(Board.GameGrid));
            }
        }
        #endregion


        //=================================================================================================================
        // CONSTRUCTORS
        //=================================================================================================================
        #region CONSTRUCTOR


        //For quick creation
        //=================================================
        public GameSession(byte rows = 8, byte cols = 8)
        {
            Board = new(rows, cols);
            InstallSettings(new());

            //timer setup
            Timer = new DispatcherTimer();
            Timer.Tick += CountdownTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
        }



        //For hand creation
        //=================================================
        public GameSession(GameBoard board, GameStartupSettings gameStartupSettings)
        {
            //main properties
            Board = board;
            InstallSettings(gameStartupSettings);

            //timer setup
            Timer = new DispatcherTimer();
            Timer.Tick += CountdownTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
        }




        //For deteiled creation
        //=================================================
        public GameSession
        (
        GameBoard board,
        GameStartupSettings gameStartupSettings,
        Color currentPlayerMove = Color.White,
        short movesLeft = -1,
        short whiteSideTimeLeft = -1,
        short blackSideTimeLeft = -1,
        (byte, byte)? cellThatMustBePromoted = null
        )
        {
            //ініціалізувати дошку та основні настройки гри
            Board = board;
            InstallSettings(gameStartupSettings, currentPlayerMove, movesLeft, whiteSideTimeLeft, blackSideTimeLeft, cellThatMustBePromoted);

            //ініціалізувати таймер
            Timer = new DispatcherTimer();
            Timer.Tick += CountdownTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
        }
        #endregion



        //=================================================================================================================
        // MOVES
        //=================================================================================================================
        #region MOVES
        public async Task<List<IMove>> Get_AllPossibleMovesToPerform_ForCell(GameCell cell)
        {
            //семафор потрібний для синхронізації процесів
            await semaphoreSlim.WaitAsync();
            try
            {
                //check possibility to get move
                if (IsGameOver || cell == null || cell.Piece == null || cell.Piece.PieceColor != CurrentPlayerMove || IsPromotionNow)
                {
                    return new List<IMove>();
                }
                //якщо грає штучний інтелект, то не можна давати ходи
                else if (cell.Piece.PieceColor == Color.White && IsWhiteAI_Playing || cell.Piece.PieceColor == Color.Black && IsBlackAI_Playing)
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
        public void Set_MovesLeft_As(int movesLeft)
        {
            if (movesLeft < 0 || movesLeft > 1000)
            {
                throw new ArgumentException("Moves left value can not be less than 0 or bigger than 1000!");
            }
            else MovesLeft = movesLeft;
        }
        #endregion

        //=================================================================================================================
        // AI
        //=================================================================================================================
        #region AI
        private async Task AI_Play(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!IsGameOver && !IsPromotionNow && (IsWhiteAI_Playing || IsBlackAI_Playing))
            {
                await Task.Delay(100);

                if (CurrentPlayerMove == Color.White)
                {
                    await Internal_MakeMove(WhiteSide_AI.Get_TheBestMove_ForColor(CurrentPlayerMove, Board.Clone(), IsKingInDangerShouldBeShown), token);
                }
                else if (CurrentPlayerMove == Color.Black)
                {
                    await Internal_MakeMove(BlackSide_AI.Get_TheBestMove_ForColor(CurrentPlayerMove, Board.Clone(), IsKingInDangerShouldBeShown), token);
                }
                OnPropertyChanged(nameof(AnalizedBoardsCount));
            }
        }
        #endregion

        //=================================================================================================================
        // BEATEN LISTS
        //=================================================================================================================
        #region BEATEN LISTS
        public void Add_BeatenPiecesInMove_ToList(IMove move)
        {
            //пройтись по списку оновлень у ході
            for (int i = 0; i < move.UpdatesCount; i++)
            {
                //отримати оновлення
                IMoveUpdate update = move.Updates[i]!;

                //якщо це оновлення захоплення, то додати захоплену фігуру у список
                if ((update.MoveType == MoveType.Capture || update.MoveType == MoveType.CapturePromotion) && update.Old_Piece != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (update.Old_Piece?.PieceColor == Color.Black) BeatenPieces_Black.Add(update.Old_Piece);
                        else if (update.Old_Piece?.PieceColor == Color.White) BeatenPieces_White.Add(update.Old_Piece);
                    });
                }
            }
            Check_CollectionLimit(BeatenPieces_White, nameof(BeatenPieces_White));
            Check_CollectionLimit(BeatenPieces_Black, nameof(BeatenPieces_Black));
        }


        private void RemoveBeatenPiecesInMoveInList(IMove move)
        {
            //пройтись по списку оновлень у ході
            for (int i = 0; i < move.UpdatesCount; i++)
            {
                //отримати оновлення
                IMoveUpdate update = move.Updates[i]!;

                //якщо це оновлення захоплення, то видалити захоплену фігуру з списку
                if ((update.MoveType == MoveType.Capture || update.MoveType == MoveType.CapturePromotion) && update.Old_Piece != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (update.Old_Piece?.PieceColor == Color.Black) BeatenPieces_Black.Remove(update.Old_Piece);
                        else if (update.Old_Piece?.PieceColor == Color.White) BeatenPieces_White.Remove(update.Old_Piece);
                    });
                }
            }
        }
        #endregion

        //=================================================================================================================
        // PROCCES LISTS
        //=================================================================================================================
        #region PROCCES LISTS
        private void ProccesAllLists(IMove move)
        {
            Add_BoardHash_ToList(move);
            Add_Move_ToUndoList_And_Clear_RedoList(move);
            Add_BeatenPiecesInMove_ToList(move);
        }

        private void Check_CollectionLimit<T>(IEnumerable<T> collection, string collectionName)
        {
            if(collection.Count() > CollectionLimit)
            {
                throw new OverflowException($"Reached maximul limit for collection: {collectionName}");
            }
        }
        #endregion

        //=================================================================================================================
        // COLORS (Players)
        //=================================================================================================================
        #region COLORS
        private void SwapPlayer()
        {
            CurrentPlayerMove = (CurrentPlayerMove == Color.White) ? Color.Black : Color.White;
        }
        public void Set_CurrentPlayer_As(Color player)
        {
            if(player != Color.White && player!= Color.Black) 
            {
                throw new ArgumentException("Player color should be black or white to setup it!");
            }
            else CurrentPlayerMove = player;

        }
        private async Task SwapPlayer_WithDelay_IfNeed(CancellationToken token)
        {
            await (GUI_Handler?.Show_DelayScreen_IfNeed(this, token) ?? Task.Delay(100));
            SwapPlayer();
        }
        #endregion

        //=================================================================================================================
        // GRAPHIC INTERFACE
        //=================================================================================================================
        #region GRAPHIC INTERFACE
        private void Update_GUI()
        {
            GUI_Handler?.Update_GUI(this);
            OnPropertyChanged(nameof(WhiteSide_Score));
            OnPropertyChanged(nameof(BlackSide_Score));
        }
        #endregion

        //=================================================================================================================
        // NOTIFY PROPERTY CHANGES
        //=================================================================================================================
        #region NOTIFY PROPERTY CHANGES
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //=================================================================================================================
        // INDEXATORS
        //=================================================================================================================
        #region INDEXATORS
        public ICell this[byte row, byte col]
        {
            get { return Board[row, col]; }
        }
        #endregion
    }
}
*/