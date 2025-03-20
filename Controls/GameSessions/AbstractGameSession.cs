using Chess_game.Behaviour;
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
using System.Windows.Documents;
using System.Windows.Threading;

namespace Chess_game.Controls.GameSessions
{
    public abstract class AbstractGameSession : INotifyPropertyChanged
    {


        //=================================================================================================================
        // FIELDS
        //=================================================================================================================
        #region FIELDS
        protected int _movesLeft = 1000;
        protected bool _isGameOver = false;
        protected int _timeLeftForBlack = 600;
        protected int _timeLeftForWhite = 600;
        protected Color _currentPlayerMove = Color.None;
        protected static readonly Random Random = new Random();
        protected static readonly ushort CollectionLimit = 1001;

        protected LinkedList<IMove> _undoList = new();
        protected LinkedList<IMove> _redoList = new();
        #endregion


        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================
        #region HANDLERS
        [JsonIgnore] protected GUI_Handler? GUI_Handler { get; set; } = GUI_Handler.Get_Instance();
        [JsonIgnore] protected Sound_Handler? Sound_Handler { get; set; } = Sound_Handler.Get_Instance();
        [JsonIgnore] protected Notification_Handler? Notification_Handler { get; set; } = Notification_Handler.Get_Instance();
        #endregion

        #region BEATEN PIECES PROPERTIES
        [JsonIgnore] public ObservableCollection<IPlayableChessPiece> BeatenPieces_White { get; } = new();
        [JsonIgnore] public ObservableCollection<IPlayableChessPiece> BeatenPieces_Black { get; } = new();
        #endregion

        #region TIMER PROPERTIES
        [JsonIgnore] protected DispatcherTimer Timer { get; set; }
        public int TimeLeftForBlack
        {
            get
            {
                return _timeLeftForBlack;
            }
            protected set
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
            protected set
            {
                if (value < 0) _timeLeftForWhite = 0;
                else if (value > 9999) _timeLeftForWhite = 9999;
                else _timeLeftForWhite = value;
                OnPropertyChanged();
            }
        }
        #endregion




        #region BOARD PROPERTIES
        //=================================================================================================================
        [JsonIgnore] public byte Columns => Board.Columns;
        [JsonIgnore] public byte Rows => Board.Rows;
        [JsonIgnore] public int WhiteSide_Score => Board.Estimate_ScoreOfCurrentBoard_ForColor(Color.White);
        [JsonIgnore] public int BlackSide_Score => Math.Abs(Board.Estimate_ScoreOfCurrentBoard_ForColor(Color.Black));
        [JsonIgnore] public ulong AnalizedBoardsCount => AI.AnalyzedBoardsCount;
        [JsonIgnore] public bool IsTimerOn => CurrentGameBoardSettings.IsTimerOn;
        [JsonIgnore] public bool IsMovesLimitOn => CurrentGameBoardSettings.IsMovesLimitOn;
        [JsonIgnore] public bool IsFogOfWarOn => CurrentGameBoardSettings.IsFogOfWarOn;
        [JsonIgnore] public bool IsKingInDangerShouldBeShown => !CurrentGameBoardSettings.IsFogOfWarOn;
        [JsonIgnore] public Color MainPlayerColor => CurrentGameBoardSettings.MainPlayerColor;
        [JsonIgnore] public GameModeType CurrentGameMode => CurrentGameBoardSettings.GameMode;


        //=================================================================================================================
        public GameStartupSettings CurrentGameBoardSettings { get; protected set; } = new();
        public GameBoard Board { get; protected set; }
        #endregion


        #region UNDO/REDO
        /*        public Stack<IMove> UndoList
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
                }*/

        [JsonIgnore] public IEnumerable<IMove> UndoList => _undoList.ToList();
        [JsonIgnore] public IEnumerable<IMove> RedoList => _redoList.ToList();
        #endregion


        #region GAME PROCCES PROPERTIES
        [JsonIgnore] public string CurrentPlayerImageSource => $"pack://application:,,,/Assets/{CurrentPlayerMove}_King.png";
        [JsonIgnore] protected static List<IMove> EmptyMoveList { get; } = new();
        [JsonIgnore] protected static GameBoard EmptyBoard { get; } = new(4, 4);

        //lists
        [JsonIgnore] protected LinkedList<uint> _undo_hashList = new LinkedList<uint>();
        [JsonIgnore] public LinkedList<uint> Undo_BoardHashList
        {
            get
            {
                return _undo_hashList;
            }
            protected set
            {
                _undo_hashList = value;
                OnPropertyChanged(nameof(Undo_BoardHashList));
                OnPropertyChanged();
            }
        }
        [JsonIgnore] public LinkedList<uint> Redo_BoardHashList { get; protected set; } = new LinkedList<uint>();


        //important
        [JsonRequired] protected List<ICheckGameOverCommand> GameOverCommands { get; set; } = new List<ICheckGameOverCommand>();
        [JsonIgnore] public ICell? CurrentCellThatNustBePromoted => Board?.CurrentCellThatNustBePromoted;
        [JsonIgnore] protected ZobristHashGenerator? ZobristHashGenerator { get; set; }


        [JsonIgnore] public bool IsPromotionNow
        {
            get
            {
                return Board.IsPromotionNow;
            }
            protected set
            {
                Board.IsPromotionNow = value;
                OnPropertyChanged(nameof(IsPromotionNow));
            }
        }
        [JsonIgnore] public bool IsGameOver
        {
            get { return _isGameOver; }
            protected set
            {
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }
        public Color CurrentPlayerMove
        {
            get { return _currentPlayerMove; }
            protected set
            {
                _currentPlayerMove = value;
                OnPropertyChanged(nameof(CurrentPlayerImageSource));
            }
        }
        public EndGameInfo EndGameInfo { get; protected set; } = new EndGameInfo(GameOverType.None, "In procces");
        public int MovesLeft
        {
            get { return _movesLeft; }
            protected set
            {
                if (value > 1000) _movesLeft = 1000;
                else if (value < 0) _movesLeft = 0;
                else _movesLeft = value;
                OnPropertyChanged(nameof(MovesLeft));
            }
        }


        //для сихронізації процесів
        [JsonIgnore] protected static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        #endregion







        //=================================================================================================================
        #region USER METHODS-INTERFACES
        public abstract Task Make_Move(Move move, CancellationToken token);
        public abstract Task Make_Promotion(ushort promoteTo_ID, CancellationToken token);
        public abstract Task<List<IMove>> Get_AllPossibleMoves_FromCell(GameCell cell); 
        public abstract void DisposeGame();
        public void Undo()
        {
            if (IsPromotionNow)
            {
                Notification_Handler?.NotifyUser("Can not undo while promotion!");
            }

            if (_undoList.Count > 0 && !IsPromotionNow)
            {
                IMove move = _undoList.Last();
                _undoList.RemoveLast();
                _redoList.AddLast(move);

                Board.Undo_Move(move);
                MovesLeft++;

                Remove_BeatenPiecesInMove_FromBeatenList(move);
                UndoHashList();

                Set_CurrentPlayer_As(move.Executor_Color);
                OnPropertyChanged(nameof(UndoList));
                Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
            }
            Update_GUI();
        }
        public void Redo()
        {
            if (_redoList.Count > 0 && !IsPromotionNow)
            {
                IMove move = _redoList.Last();
                _redoList.RemoveLast();
                _undoList.AddLast(move);

                Board.Make_Move(move);
                MovesLeft--;

                Add_BeatenPiecesInMove_ToBeatenList(move);
                RedoHashList();

                if (!IsPromotionNow) SwapPlayer();
                OnPropertyChanged(nameof(UndoList));
                Check_CollectionLimit(RedoList, nameof(RedoList));
                Sound_Handler?.PlaySound_UsingMoveType(this, move.MainMoveType);
            }
            Update_GUI();
        }
        
        public void StartTimer() => Timer?.Start();
        public void StopTimer() => Timer?.Stop();
        #endregion


        [JsonIgnore] protected bool IsGameDisposed { get; set; } = false;
        [JsonIgnore] public bool IsGameStarted
        {
            get; 
            protected set;
        }
        public abstract void PauseGame();
        public abstract Task StartGame(CancellationToken token);
        


       /* public void PauseTimer()
        {
            if(IsTimerOn) Timer?.Stop();
        }
        public void LaunchTimer()
        {
            if(IsTimerOn && !IsGameOver) Timer?.Start();
        }*/




        //=================================================================================================================
        #region CONSTRUCTOR

        // For quick creation
        protected AbstractGameSession(byte rows = 8, byte cols = 8)
        {
            //board and settings
            Board = new(rows, cols);
            InstallSettings(new());

            //timer setup
            Timer = new DispatcherTimer();
            Timer.Tick += CountdownTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
        }

        // More deteiled creation
        protected AbstractGameSession(GameBoard board, GameStartupSettings gameStartupSettings)
        {
            //main properties
            Board = board;
            InstallSettings(gameStartupSettings);

            //timer setup
            Timer = new DispatcherTimer();
            Timer.Tick += CountdownTimerTick;
            Timer.Interval = TimeSpan.FromSeconds(1);
        }

        // For deteiled creation
        public AbstractGameSession
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
        #region UNDO/REDO
        protected void Add_Move_ToUndoList_And_Clear_RedoList(IMove move)
        {
            if (_redoList.Count > 0) _redoList.Clear();
            _undoList.AddLast(move);

            OnPropertyChanged(nameof(UndoList));
            Check_CollectionLimit(UndoList, nameof(UndoList));
        }
        #endregion


        //=================================================================================================================
        // HASH LISTS
        //=================================================================================================================
        #region HASH LISTS
        protected void UndoHashList()
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
        protected void RedoHashList()
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
        protected void Add_BoardHash_ToList(IMove move)
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
            TimerSetup_Helper(settings, whiteSideTimeLeft, blackSideTimeLeft);
            MovesLimitSetup_Helper(settings, movesLeft);

            //gameplay
            SetupCellThatMustBePromoted_Helper(cellThatMustBePromoted);
            GameOverCommandsSetup_Helper(settings);
            GeneralSettingsSetup_Helper(settings);
            ZobristHashInstalation_Helper();
        }


        //settings helpers
        protected void Unsubscribe_Handlers()
        {
            Sound_Handler = null;
            GUI_Handler = null;
            Notification_Handler = null;
        }
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
            if (ZobristHashGenerator == null)
            {
                //hash initialization
                ZobristHashGenerator = new ZobristHashGenerator(Rows, Columns, Board.GameGrid);
                Undo_BoardHashList.AddLast(ZobristHashGenerator.GetHashForBoard(Board.GameGrid));
            }
        }
        #endregion




        //=================================================================================================================
        // TIMER
        //=================================================================================================================
        #region TIMER
        protected virtual void CountdownTimerTick(object? sender, EventArgs? e)
        {
            if (CurrentPlayerMove == Color.White)
            {
                TimeLeftForWhite--;
                if (TimeLeftForWhite <= 0)
                {
                    EndGameInfo = new EndGameInfo(GameOverType.Black_Win, $"Unforunately time ended up for WHITE side.\nSo, the winner is BLACK side!");
                    FinishGame();
                }
            }
            else if (CurrentPlayerMove == Color.Black)
            {
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
        // MOVES
        //=================================================================================================================
        #region MOVES
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
        // BEATEN LISTS
        //=================================================================================================================
        #region BEATEN LISTS
        protected void Add_BeatenPiecesInMove_ToBeatenList(IMove move)
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
        protected void Remove_BeatenPiecesInMove_FromBeatenList(IMove move)
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
        #region PROCCES LISTS
        protected void Procces_AllLists_ForMove(IMove move)
        {
            Add_BoardHash_ToList(move);
            Add_Move_ToUndoList_And_Clear_RedoList(move);
            Add_BeatenPiecesInMove_ToBeatenList(move);
        }
        protected void Check_CollectionLimit<T>(IEnumerable<T> collection, string collectionName)
        {
            if (collection.Count() > CollectionLimit)
            {
                throw new OverflowException($"Reached maximul limit for collection: {collectionName}");
            }
        }
        protected void Clear_Redo_And_Undo_Lists()
        {
            foreach(IMove move in _undoList)
            {
                move.ClearMove();
            }
            _undoList.Clear();


            foreach (IMove move in _redoList)
            {
                move.ClearMove();
            }
            _redoList.Clear();
        }
        #endregion


        //=================================================================================================================
        #region COLORS
        protected void SwapPlayer()
        {
            CurrentPlayerMove = (CurrentPlayerMove == Color.White) ? Color.Black : Color.White;
        }
        protected void Set_CurrentPlayer_As(Color player)
        {
            if (player != Color.White && player != Color.Black)
            {
                throw new ArgumentException("Player color should be black or white to setup it!");
            }
            else CurrentPlayerMove = player;

        }
        #endregion


        //=================================================================================================================
        #region ENDGAME
        protected bool CheckEndGame()
        {
            GameBoard boardCopy = Board.Clone();
            foreach (var gameOverCommand in GameOverCommands)
            {
                EndGameInfo = gameOverCommand.CheckGameOver(this, boardCopy);
                if (EndGameInfo != EndGameInfo.GameContinues)
                {
                    return true;
                }
            }
            return false;
        }
        protected void FinishGame()
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
        #region GRAPHIC INTERFACE
        protected abstract void Update_GUI();
        #endregion



        //=================================================================================================================
        #region NOTIFY PROPERTY CHANGES
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        //=================================================================================================================
        #region INDEXATORS
        public ICell this[byte row, byte col]
        {
            get { return Board[row, col]; }
        }
        #endregion
    }
}

