using Chess_game.Behaviour;
using Chess_game.Controls;
using Chess_game.Factories;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = Chess_game.Models.Color;
using Path = System.IO.Path;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Game_Data;
using Chess_game.Controls.Handlers;
using Chess_game;
using System.IO;
using Chess_game.Models.Info_objects;
using System.Windows.Data;
using System.Reflection;
using Chess_game.Controls.GameSessions;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;
//using static System.Net.Mime.MediaTypeNames;

namespace Chess_game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<WindowsThemeInfo> Themes { get; private set; } = new();
        private Binding DarkCellBackgroundBinding { get; } = new Binding($"{nameof(CurrentTheme)}.{nameof(WindowsThemeInfo.DarkBoardCellColor)}");
        private Binding LightCellBackgroundBinding { get; } = new Binding($"{nameof(CurrentTheme)}.{nameof(WindowsThemeInfo.LightBoardCellColor)}");
        private BrushConverter BrushConverter { get; } = new();




        //themes
        private static WindowsThemeInfo LightTheme { get; } = new(
            "Light Theme",
            Brushes.White,
            Brushes.LightYellow,
            Brushes.SandyBrown,
            Brushes.Black,
            Brushes.LightGoldenrodYellow,
            Brushes.Black
            );

        private WindowsThemeInfo _currentTheme = LightTheme;
        public WindowsThemeInfo CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                _currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }


        //game
        public bool IsMovesUndoListShouldBeShown => !_gameSession.IsFogOfWarOn || (_gameSession.IsFogOfWarOn && _gameSession.IsGameOver);
        private byte Rows => GameSession.Rows;
        private byte Columns => GameSession.Columns;

        private AbstractGameSession _gameSession;
        public AbstractGameSession GameSession
        {
            get { return _gameSession; }
            set
            {
                _gameSession = value;
                OnPropertyChanged(nameof(GameSession));
            }
        }

        #region  Startup settings
        public bool IsTimerOn { get; set; } = false;
        public bool IsMovesLimitOn { get; set; } = false;
        public bool IsFogOfWarOn { get; set; } = false;
        public bool IsSoundOn { get; set; } = false;
        private Color MainPlayerColor { get; set; } = Color.None;
        private GameModeType GameMode { get; set; } = GameModeType.None;
        public Difficulty WhiteSideComputerDifficulty { get; set; } = Difficulty.Easy;
        public Difficulty BlackSideComputerDifficulty { get; set; } = Difficulty.Easy;
        public bool IsGameStarted { get; set; } = false;
        #endregion
        

        //grid
        private Border[,] GridBorders { get; set; }
        private Image[,] GridImages { get;set; }



        //cells
        private ICell? SellectedCell { get; set; }
        private List<IMove> HightlightedPossibleMoves { get; set; } = new();
        private List<ICell> DangerousCells { get; set; } = new();

        //else
        private MediaPlayer MediaPlayer { get; } = new();
        private CancellationTokenSource? CancellationTokenSource { get; set; } = new CancellationTokenSource();


        //для сихронізації процесів
        private static SemaphoreSlim semaphoreSlim { get; } = new SemaphoreSlim(1, 1);













        //=================================================================================================================
        // EVENTS
        //=================================================================================================================
        #region EVENTS
        private async void OnMouseDown_GameGridBorder(object sender, MouseEventArgs e)
        {
            if (semaphoreSlim.CurrentCount == 0) return;

            if (sender is Border border && IsGameStarted && !GameSession.IsGameOver)
            {
                On_SoundPlay("move.mp3");

                if (border.DataContext is GameCell gameCell && gameCell.Piece != null)
                {
                    if (gameCell == SellectedCell || GameSession.CurrentGameMode == GameModeType.EvE ||
                        !gameCell.Piece.Has_SameColor_With(GameSession.CurrentPlayerMove) ||
                        (!gameCell.Piece.Has_SameColor_With(GameSession.MainPlayerColor) && GameSession.CurrentGameMode == GameModeType.PvE)) return;

                    //Стару клітину слід перефарбувати в старий колів, і якщо потрібно в червоний, якщо це король в небезпеці
                    Clear_SellectedCell();
                    Clear_PossibleMoves();
                    Paint_CellsInRedColor_WhereKingsFiguresInDanger();

                    SellectedCell = gameCell;
                    GridBorders[SellectedCell.X, SellectedCell.Y].Background = Brushes.Yellow;

                    HightlightedPossibleMoves = await GameSession.Get_AllPossibleMoves_FromCell(gameCell);
                    Draw_PossibleMoves();

                }
            }
        }

        private void On_GameOver()
        {
            Dispatcher.Invoke(() =>
            {
                Clear_PossibleMoves();
                Clear_SellectedCell();
                OnPropertyChanged(nameof(IsMovesUndoListShouldBeShown));

                StartGame_Button.Visibility = Visibility.Hidden;

                switch (GameSession.EndGameInfo.GameOverType)
                {
                    case GameOverType.White_Win:
                        GameWinner.Text = "Winner: WHITE side";
                        break;
                    case GameOverType.Black_Win:
                        GameWinner.Text = "Winner: BLACK side";
                        break;
                    case GameOverType.Stalemate:
                        GameWinner.Text = "Winner: STALEMATE";
                        break;
                    case GameOverType.Draw:
                        GameWinner.Text = "Winner: DRAW";
                        break;
                    default:
                        GameWinner.Text = "";
                        break;
                }
                MessageBox.Show(GameSession.EndGameInfo.Message);
            });
        }

        private void On_DangerForKingsDetected(IEnumerable<ICell> list)
        {
            Dispatcher.Invoke(() =>
            {
                Clear_PossibleMoves();
                Paint_CellsInoriginalColor_WhereKingsFiguresInDanger();
                DangerousCells = list.ToList();
                Paint_CellsInRedColor_WhereKingsFiguresInDanger();
            });
        }

        private void On_PromotionBegin(Color color)
        {
            Dispatcher.Invoke(() =>
            {
                //get piece that must be promoted now
                //=================================================================================================================
                IPlayableChessPiece? piece = GameSession?.CurrentCellThatNustBePromoted?.Piece;
                Promotion_Stackpanel.Children.Clear();

                //begin procces
                //=================================================================================================================
                if (piece != null)
                {
                    //get all pieces id to which we can promote
                    //=================================================================================================================
                    foreach (ushort promotion_id in PromotionLists_Factory.Get_PromotionList_ByID(piece.PromotionListID))
                    {

                        //With their distinguishing name get images and add make promotion event on click
                        //=================================================================================================================
                        string distingushingName = Pieces_Factory.Get_PieceDistinguishingName_UsingID_ForColor(promotion_id, color);
                        Image image = new()
                        {
                            Source = PreparedImages.PieceNameToBitmap[distingushingName],
                            Width = 45,
                            Height = 45,
                            DataContext = promotion_id,
                            Margin = new Thickness(10)
                        };
                        image.MouseLeftButtonDown += OnMouseLeftButtonDown_PromotionImage;
                        image.MouseEnter += OnMouseEnter_PromotionImage;
                        image.MouseLeave += OnMouseLeave_PromotionImage;

                        //add small tool tip so that player could know to which figure he promote
                        //=================================================================================================================
                        ToolTip tip = new ToolTip();
                        tip.Content = Pieces_Factory.Get_PieceName_UsingID(promotion_id);
                        if (Application.Current.TryFindResource("PromotionLargeToolTipStyle") is Style style)
                        {
                            tip.Style = style;
                        }
                        image.ToolTip = tip;

                        //add image to stack panel to see it
                        //=================================================================================================================
                        Promotion_Stackpanel.Children.Add(image);
                    }
                }
            });
        }

        private void On_MoveMade()
        {
            Dispatcher.Invoke(() =>
            {
                Clear_PossibleMoves();
                Clear_SellectedCell();
                ReDraw_GameGrid();
            });
        }



        private void OnClick_ShowMessage(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is string message)
            {
                MessageBox.Show(message);
            }
        }
        private void OnChecked_PlaySound(object sender, RoutedEventArgs e)
        {
            On_SoundPlay("move.mp3");
        }
        private void OnClick_Exit(object sender, EventArgs e)
        {
            Close();
        }
        private void On_SoundPlay(string fileName)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsSoundOn)
                {
                    string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", "Sounds", fileName);

                    MediaPlayer.Open(new Uri(soundFilePath));
                    MediaPlayer.Play();
                }
            });
        }
        private void On_PromotionMade()
        {
            Dispatcher.Invoke(() =>
            {
                Promotion_Stackpanel.Children.Clear();
            });
        }
        #endregion



        private ToolTip Create_ToolTip_WithDescription_UsingStyle(string description, string style)
        {
            ToolTip tip = new ToolTip()
            {
                Content = description,
            };
            if (Application.Current.TryFindResource(style) is Style createdStyle)
            {
                tip.Style = createdStyle;
            }
            return tip;
        }



        //=================================================================================================================
        #region DRAWS/GUI
        private void Draw_PossibleMoves()
        {
            foreach (IMove move in HightlightedPossibleMoves)
            {
                Ellipse ellipse = new()
                {
                    Fill = Brushes.Gray,
                    Opacity = 0.6,
                    DataContext = move
                };

                //make ellipse smaller and centralised
                ScaleTransform scaleTransform = new(0.8, 0.8);
                TransformGroup myTransformGroup = new();
                myTransformGroup.Children.Add(scaleTransform);
                ellipse.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ellipse.RenderTransform = myTransformGroup;

                //add events
                ellipse.MouseEnter += OnMouseEnter_Elipse;
                ellipse.MouseLeave += OnMouseLeave_Elipse;
                ellipse.MouseLeftButtonDown += OnMouseLeftButtonDown_Elipse;


                //add small tool tip so that player could know to which move he make
                //=================================================================================================================
                ToolTip tip = Create_ToolTip_WithDescription_UsingStyle(move.Get_MoveType, "LargeToolTipStyle");
                ellipse.ToolTip = tip;

                (GridBorders[move.To_Cell.X, move.To_Cell.Y].Child as Grid)?.Children.Add(ellipse);
            }
            
        }
        private void Clear_PossibleMoves()
        {
            if (HightlightedPossibleMoves != null)
            {
                foreach (IMove move in HightlightedPossibleMoves)
                {
                    int x = move.To_Cell.X;
                    int y = move.To_Cell.Y;

                    if (GridBorders[x, y].Child is Grid grid)
                    {
                        for (int i = grid.Children.Count - 1; i >= 0; i--)
                        {
                            if (grid.Children[i] is Ellipse)
                            {
                                grid.Children.RemoveAt(i);
                            }
                        }
                    }
                }
                HightlightedPossibleMoves.Clear();
            }
        }


        private void Paint_CellsInRedColor_WhereKingsFiguresInDanger()
        {
            if (DangerousCells != null)
            {
                foreach (ICell cell in DangerousCells)
                {
                    int x = cell.X;
                    int y = cell.Y;

                    GridBorders[x, y].Background = Brushes.Red;
                }
            }
        }
        private void Paint_CellsInoriginalColor_WhereKingsFiguresInDanger()
        {
            if (DangerousCells != null)
            {
                foreach (ICell cell in DangerousCells)
                {
                    int x = cell.X;
                    int y = cell.Y;

                    GridBorders[x, y].SetBinding(Border.BackgroundProperty, (((x + y) & 1) == 1) ? DarkCellBackgroundBinding : LightCellBackgroundBinding);
                }
                DangerousCells.Clear();
            }
        }

        private void Swap_Timers_And_BeatenLists_DependingOnMainPlayer()
        {
            //відєднумо дочірні елементи від їх контейнерів, щоб не було помилки
            MainPlayer_Timer_Container.Child= null;
            SecondPlayer_Timer_Container.Child = null;
            MainPlayer_BeatenPiecesContainer.Child = null;
            SecondPlayer_BeatenPiecesContainer.Child = null;

            if (GameSession.MainPlayerColor == Color.White)
            {
                MainPlayer_Timer_Container.Child = Timer_WhiteTeam;
                SecondPlayer_Timer_Container.Child = Timer_BlackTeam;
                MainPlayer_BeatenPiecesContainer.Child = BeatenPieces_Black;
                SecondPlayer_BeatenPiecesContainer.Child = BeatenPieces_White;
            }
            else if (GameSession.MainPlayerColor == Color.Black)
            {
                MainPlayer_Timer_Container.Child = Timer_BlackTeam;
                SecondPlayer_Timer_Container.Child = Timer_WhiteTeam;
                MainPlayer_BeatenPiecesContainer.Child = BeatenPieces_White;
                SecondPlayer_BeatenPiecesContainer.Child = BeatenPieces_Black;
            }
        }
        private void Clear_SellectedCell_PossibleMoves_CellswhereKingsInDanger_And_GameWinnerText()
        {
            Clear_SellectedCell();
            Paint_CellsInoriginalColor_WhereKingsFiguresInDanger();
            Clear_PossibleMoves();
            GameWinner.Text = EndGameInfo.GameContinues.Message;
        }
        private void ReDraw_GameGrid()
        {
            if (GridImages != null)
            {
                for (byte i = 0; i < GameSession.Rows; i++)
                {
                    for (byte j = 0; j < GameSession.Columns; j++)
                    {
                        GridImages[i, j].Source = PreparedImages.PieceNameToBitmap[GameSession[i, j].Piece?.GetDistinguishName ?? "None"];
                    }
                }
            }
        }
        private void Draw_Fog()
        {
            Dispatcher.Invoke(() =>
            {            
                if (GridImages != null)
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            GridImages[i, j].Source = PreparedImages.PieceNameToBitmap["Fog"];
                        }
                    }
                }
            });
        }
        private void Draw_VisibleCells(bool[][] visibleCells)
        {
            Dispatcher.Invoke(() =>
            {
                Clear_PossibleMoves();

                for (byte i = 0; i < visibleCells.Length; i++)
                {
                    for (byte j = 0; j < visibleCells[i].Length; j++)
                    {
                        if (visibleCells[i][j])
                        {
                            GridImages[i, j].Source = PreparedImages.PieceNameToBitmap[GameSession[i, j].Piece?.GetDistinguishName ?? "None"];
                        }
                        else GridImages[i, j].Source = PreparedImages.PieceNameToBitmap["Fog"];
                    }
                }
            });
        }
        private void Clear_SellectedCell()
        {
            if (SellectedCell != null)
            {
                //return to origin color
                GridBorders[SellectedCell.X, SellectedCell.Y].
                    SetBinding(Border.BackgroundProperty, 
                    (((SellectedCell.X + SellectedCell.Y) & 1) == 1) ? DarkCellBackgroundBinding : LightCellBackgroundBinding);
                SellectedCell = null;
            }
        }
        #endregion













        //=================================================================================================================
        #region START GAME
        private void OnClick_StartNewGame_UsingGameBoardInfo(object sender, RoutedEventArgs e)
        {
            if (!IsInputDataForGameStartupIsValid())
            {
                Show_MessageAboutInvalidInputDataForGameStartup();
            }
            else if(sender is MenuItem menuItem && menuItem.DataContext is BoardInfo info)
            {
                Start_NewGame_UsingBoardInfo(info);   
            }
        }
        private void Start_NewGame_UsingBoardInfo(BoardInfo info)
        {
            GameStartupSettings settings = Generate_StartupSettingsBasedOnCurrentInputInMenu();

            GameSession.DisposeGame();

            GC.Collect();

            Set_ActiveGameSessionAs(GameSession_Factory.Create_GameSession_UsingPath(info.FilePath, settings));
        }
        //===================================
        private void OnClick_StartClassicBoardGame(object sender, RoutedEventArgs e)
        {

            if (!IsInputDataForGameStartupIsValid())
            {
                Show_MessageAboutInvalidInputDataForGameStartup();
            }
            else
            {
                Start_ClassicBoardGame();
            }
        }
        private void Start_ClassicBoardGame()
        {
            GameStartupSettings settings = Generate_StartupSettingsBasedOnCurrentInputInMenu();

            GameSession.DisposeGame();

            GC.Collect();

            Set_ActiveGameSessionAs(GameSession_Factory.Create_ClassicBoard_GameSession(settings));
        }
        //===================================
        private void Show_MessageAboutInvalidInputDataForGameStartup()
        {
            if (!IsPlayAsInputDataIsValid())
            {
                MessageBox.Show("Chose your side (black/white) to start!");
                return;
            }

            if (!IsGameModeInputDataIsValid())
            {
                MessageBox.Show("Chose game mode (PvP/PvE/EvE) to start!");
                return;
            }


            int movesLimit = 0;
            if (IsMovesLimitOn)
            {
                if (MovesLimit_TextBox.Text == "")
                {
                    MessageBox.Show("Type in moves limit if you want to play with it!");
                    return;
                }
                if (!int.TryParse(MovesLimit_TextBox.Text, out movesLimit))
                {
                    MessageBox.Show("Type in correct data to moves limit!");
                    return;
                }
                if (movesLimit == 0)
                {
                    MessageBox.Show("Moves limit can not be less or equal 0!");
                    return;
                }
            }

            if (IsTimerOn)
            {
                if (White_Timer_TextBox.Text == "" || Black_Timer_TextBox.Text == "")
                {
                    MessageBox.Show("Type in time for both players!");
                    return;
                }
                if (!int.TryParse(White_Timer_TextBox.Text, out int white_time) || !int.TryParse(Black_Timer_TextBox.Text, out int black_time))
                {
                    MessageBox.Show("Type in correct data to timer duration!");
                    return;
                }
                if (white_time == 0 || black_time == 0)
                {
                    MessageBox.Show("Are you fucking seriously?! 0 second timer?!!!");
                    return;
                }
            }
        }
        private void Set_ActiveGameSessionAs(AbstractGameSession gameSession)
        {
            GameSession = gameSession;

            GameSession.UpdateGUIForNewGame();

            DelayScreen_Border.Visibility = Visibility.Hidden;

            Initialize_GameGrid();
            Initialize_StartGameButton();
            Swap_Timers_And_BeatenLists_DependingOnMainPlayer();
            Initialize_NewCancellationTokenSource_And_DisposeOld();
            Clear_SellectedCell_PossibleMoves_CellswhereKingsInDanger_And_GameWinnerText();
            OnPropertyChanged(nameof(IsMovesUndoListShouldBeShown));

            if (GameSession.IsFogOfWarOn) Draw_Fog();
        }
        private GameStartupSettings Generate_StartupSettingsBasedOnCurrentInputInMenu()
        {
            return new GameStartupSettings()
            {
                MainPlayerColor = MainPlayerColor,
                GameMode = GameMode,
                WhitesDifficulty = WhiteSideComputerDifficulty,
                BlacksDifficulty = BlackSideComputerDifficulty,
                IsTimerOn = IsTimerOn,
                WhitesTimerDuration = int.Parse(White_Timer_TextBox.Text),
                BlacksTimerDuration = int.Parse(Black_Timer_TextBox.Text),
                IsMovesLimitOn = IsMovesLimitOn,
                MovesLimit = int.Parse(MovesLimit_TextBox.Text),
                IsFogOfWarOn = IsFogOfWarOn
            };
        }
        //===================================
        private bool IsPlayAsInputDataIsValid() =>
          Play_As_White_RadioButton.IsChecked == true ||
          Play_As_Black_RadioButton.IsChecked == true ||
          Play_As_Random_RadioButton.IsChecked == true;
        private bool IsGameModeInputDataIsValid() =>
            GameType_PvP.IsChecked == true ||
            GameType_PvE.IsChecked == true ||
            GameType_EvE.IsChecked == true;
        private bool IsMovesLimitInputDataIsValid() =>
            !IsMovesLimitOn ? true :
            (MovesLimit_TextBox.Text != "" &&
            int.TryParse(MovesLimit_TextBox.Text, out int movesLimit) &&
            movesLimit != 0);
        private bool IsTimesLimitInputDataIsValid() =>
            !IsTimerOn ? true :
            (White_Timer_TextBox.Text != "" &&
            Black_Timer_TextBox.Text != "" &&
            int.TryParse(White_Timer_TextBox.Text, out int white_time) &&
            int.TryParse(Black_Timer_TextBox.Text, out int black_time) &&
            white_time > 0 && black_time > 0);
        private bool IsInputDataForGameStartupIsValid() =>
            IsPlayAsInputDataIsValid() &&
            IsGameModeInputDataIsValid() &&
            IsMovesLimitInputDataIsValid() &&
            IsTimesLimitInputDataIsValid();
        //===================================
        private async void OnClick_StartGame(object sender, RoutedEventArgs e)
        {
            IsGameStarted = true;

            if (GameSession?.CurrentGameBoardSettings?.GameMode == GameModeType.EvE)
            {
                StartGame_Button.Content = "Stop AI battle";
                StartGame_Button.Click -= OnClick_StartGame;
                StartGame_Button.Click += OnClick_StopAI_Battle;
            }
            else
            {
                StartGame_Button.Visibility = Visibility.Hidden;
            }

            await Task.Run(async () =>
            {
                await (GameSession?.StartGame(CancellationTokenSource!.Token) ?? Task.Delay(100));
            });
        }

        private void OnClick_StopAI_Battle(object sender, RoutedEventArgs e)
        {
            if (!GameSession.IsGameOver)
            {
                StartGame_Button.Visibility = Visibility.Visible;
                StartGame_Button.Content = "Start AI battle";
                StartGame_Button.Click -= OnClick_StopAI_Battle;
                StartGame_Button.Click -= OnClick_StartGame;
                StartGame_Button.Click += OnClick_StartGame;
                GameSession.PauseGame();
                Initialize_NewCancellationTokenSource_And_DisposeOld();
            }
        }
        #endregion






        //=================================================================================================================
        #region SAVE/LOAD

        private void OnClick_SaveGame(object sender, RoutedEventArgs e)
        {
            if (GameSession.IsGameOver)
            {
                Show_Message("Can not svae after game over!");
                return;
            }
            if (GameSession.IsPromotionNow)
            {
                Show_Message("Can not save while promotion!");
                return;
            }



            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", "Saved boards"),
                Filter = "Saved games (*.json)|*.json"  //first part is displayed text and second is target 
            };


            if(GameSession.CurrentGameMode == GameModeType.EvE)
            {
                OnClick_StopAI_Battle(this, new RoutedEventArgs());
            }
            else
            {
                GameSession.PauseGame();
                Initialize_StartGameButton();
            }
            Initialize_NewCancellationTokenSource_And_DisposeOld();

            if(saveFileDialog.ShowDialog() == true)
            {
                SaveService.Save_GameSession(GameSession, saveFileDialog.FileName);
            }

           // GameSession.StartGame(CancellationTokenSource!.Token);
        }


        private void OnClick_LoadGame(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() 
            {
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", "Saved boards"),
                Filter = "Saved games (*.json)|*.json"  //first part is displayed text and second is target 
            };


            if (GameSession.CurrentGameMode == GameModeType.EvE)
            {
                OnClick_StopAI_Battle(this, new RoutedEventArgs());
            }
            else
            {
                GameSession.PauseGame();
            }
            Initialize_NewCancellationTokenSource_And_DisposeOld();


            if (openFileDialog.ShowDialog() == true)
            {
                Set_ActiveGameSessionAs(LoadService.Load_Game_UsingPath(openFileDialog.FileName));
                return;
            }

            Initialize_StartGameButton();


            //GameSession.StartGame(CancellationTokenSource!.Token);
        }

        #endregion





        //=================================================================================================================
        #region INITIALIZATION
        private void Initialize_MenuItems_AndBackgroundForIt_UsingBoards_LocatedInFolder
            (MenuItem containerMenuItem, string folderName, string HEXMenuItemBackGround)
        {
            Brush color = (BrushConverter.ConvertFrom(HEXMenuItemBackGround) as Brush) ?? Brushes.YellowGreen;

            foreach (var boardInfo in LoadService.Load_AllBoardsInfo_UsingPath(folderName))
            {
                //create menu item
                MenuItem menuItem = new()
                {
                    Header = boardInfo.Name,
                    Background = color,
                    DataContext = boardInfo,
                    ToolTip = Create_ToolTip_WithDescription_UsingStyle(boardInfo.Description, "LargeToolTipStyleForBoardMenuItem")
                };

                menuItem.Click += OnClick_StartNewGame_UsingGameBoardInfo;
                containerMenuItem.Items.Add(menuItem);
            }
        }
        private void Initialize_MenuItems_ForHelpMenuItem()
        {
            string mainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", "Game documents");

            if (!Directory.Exists(mainPath))
            {
                throw new FileNotFoundException(mainPath);
            }

            try
            {

                Action<MenuItem, string> initializeHelpMenuItem = (initializedMenuItem, fileName) =>
                {
                    string path = Path.Combine(mainPath, fileName);

                    if (System.IO.File.Exists(path))
                    {
                        initializedMenuItem.DataContext = System.IO.File.ReadAllText(path);
                    }
                    else
                    {
                        initializedMenuItem.DataContext = "Information file is not presented in Game documents folder!";
                    }
                    initializedMenuItem.Click += OnClick_ShowMessage;
                };


                initializeHelpMenuItem(GameRules_MenuItem, "Game rules.txt");
                initializeHelpMenuItem(NewGameHelp_MenuItem, "New game help.txt");
                initializeHelpMenuItem(ConstructorHelp_MenuItem, "Constructor help.txt");
                initializeHelpMenuItem(FileHelp_MenuItem, "File help.txt");

            }
            catch (Exception)
            {
                throw new Exception("Error occured while reading files for help menu items!");
            }
        }
        private void Initialize_MenuItems_ForAboutMenuItem()
        {
            Application_MenuItem.Click += OnClick_ShowMessage;
            Application_MenuItem.DataContext =
                "Current version: 1.0 \n" +
                "Available features:\n" +
                "-Completly done classic chess;\n" +
                "-Switch main player color and gamemode, including EvE;\n" +
                "-AI and 3 difficulty to play;\n" +
                "-Set timer and moves limit;\n" +
                "-Fog of war (only for PvP);\n" +
                "-Load boards from file;\n" +
                "-Change application and board themes (4 themes ready);\n" +
                "-Small help documentation;\n";

            Contacts_MenuItem.Click += OnClick_ShowMessage;
            Contacts_MenuItem.DataContext =
                "Developer: Oleh Bashtovyi\n" +
                "Email: olehbashtovyi@gmail.com";
        }
        private void Initialize_ThemesComboobx()
        {
            Themes = new List<WindowsThemeInfo>()
            {
             new("Light Theme",
            Brushes.White,
            Brushes.LightYellow,
            Brushes.SandyBrown,
            Brushes.Black,
            Brushes.LightGoldenrodYellow,
            Brushes.Black
            ),
             new("Dark Theme",
                "#252d64",
                "#daddf0",
                "#2c8edd",
                "#ffffff",
                "#158cd2",
                "#ffffff"
            ),
             new("Red Theme",
            Brushes.White,
            Brushes.FloralWhite, //Fffaf0
            Brushes.IndianRed,   //"#CD5C5C"
            Brushes.Black,       //#000000"
            Brushes.IndianRed,
            Brushes.White        //#000000"
            ),
             //"#3d3d3d"
             //"#f8fe6d"
             new("Green Theme",
            "#3e493d",   //background  "#3d3d3d"  #3f3f3f" "#555555"
            "#fffdc6",   //light cell
            "#56a74e",   //dark cell   "#4d9c45"
             "#ffffff",  //Text
             "#dcf79d",   //tool tip background
             "#000000"
            )
            };

            Theme_ComboBox.ItemsSource = Themes;
            Theme_ComboBox.SelectedItem = Themes[0];
            CurrentTheme = Theme_ComboBox.SelectedItem as WindowsThemeInfo ?? LightTheme;
        }
        private void Initialize_GameGrid()
        {
            Check_RowsAndColumnsValidation();
            Initialize_GameGridCellsSize();
            Initialize_BoardSize();
            Initalize_GridBordersAndImagesMatrixes();

            //rotate grid if main player color is black
            RotateTransform rotateTransForm = Get_RotateTransformForCurrentBoard();
            GameGrid.LayoutTransform = rotateTransForm;

            //font size adjusting for letters and numbers
            byte letterFornSize = Get_LetterFontSize_DependingOnGridSize();


            //initialisation cycle
            //=================================================================================================================
            for (byte i = 0; i < Rows; i++)
            {
                for (byte j = 0; j < Columns; j++)
                {
                    //get chess piece that locate in current cell and create it
                    Image image = Create_ImageForPiece_LocatedOnCell(GameSession[i, j]);

                    //create grid, that can hold pieces, numbers, letters, and circles that contains moves
                    Grid grid = new();
                    grid.Children.Add(image);


                    //create border that will hold grid as child. Also add on click event to get moves of clicked cell
                    Border border = new()
                    {
                        BorderThickness = new Thickness(1),
                        LayoutTransform = rotateTransForm,
                        BorderBrush = Brushes.Black,
                        DataContext = GameSession[i, j],
                        Child = grid,
                    };
                    border.SetBinding(Border.BackgroundProperty, (((i + j) & 1) == 1) ? DarkCellBackgroundBinding : LightCellBackgroundBinding);
                    border.MouseLeftButtonDown += OnMouseDown_GameGridBorder;


                    //add letters to cells
                    Add_LetterToGrid(grid, i, j, letterFornSize);
                    //add numbers to cells
                    Add_NumberToGrid(grid, i, j, letterFornSize);


                    //add created images and borders to matrixes
                    GridImages[i, j] = image;
                    GridBorders[i, j] = border;
                    GameGrid.Children.Add(border);
                }
            }
        }
        private void Initialize_Handlers()
        {
            GUI_Handler handler = GUI_Handler.Get_Instance();
            handler.OnMoveMade_ReDrawGrid += On_MoveMade;
            handler.OnPromotionBegin_ShowPromotionScreen += On_PromotionBegin;
            handler.OnPromotionMade_HidePromotionScreen += On_PromotionMade;
            handler.OnKingInDanger_ShowAtackedCells += On_DangerForKingsDetected;
            handler.OnMoveMadeShowAllVisisbleCells += Draw_VisibleCells;
            handler.OnDelayAppear += Show_DelayScreen;

            Notification_Handler notification_Handler = Notification_Handler.Get_Instance();
            notification_Handler.On_NotificationRaised += Show_Message;
            notification_Handler.On_GameOver_NotificationRaised += On_GameOver;

            Sound_Handler sound_Handler = Sound_Handler.Get_Instance();
            sound_Handler.OnSoundPlay += On_SoundPlay;
        }
        private void Initialize_NewCancellationTokenSource_And_DisposeOld()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
            }
            CancellationTokenSource = new CancellationTokenSource();
        }
        private void Initialize_StartGameButton()
        {
            //зробити відписку подій кнопки, щоб уникнути помилко
            StartGame_Button.Click -= OnClick_StopAI_Battle;
            StartGame_Button.Click -= OnClick_StartGame;

            //зробити потрібну підписку заново
            StartGame_Button.Click += OnClick_StartGame;

            //встановити назву кнопки початку гри
            if (GameSession.CurrentGameMode == GameModeType.EvE)
            {
                StartGame_Button.Content = "Start AI battle";
            }
            else StartGame_Button.Content = "Start game";

            StartGame_Button.Visibility = Visibility.Visible;
            IsGameStarted = false;
        }

        private void Show_Message(string message) => MessageBox.Show(message);


        #region Board initialization helpers
        private void Check_RowsAndColumnsValidation()
        {
            if (Rows > 16 || Columns > 16 || Rows <= 0 || Columns <= 0)
            {
                throw new Exception("Rows or Columns count can not be lass than 0 or bigger than 16!");
            }
        }
        private void Initialize_BoardSize()
        {

            GameGrid.Children.Clear();
            GameGrid.Rows = Rows;
            GameGrid.Columns = Columns;
        }
        private void Initialize_GameGridCellsSize()
        {
            if (Rows == Columns)
            {
                GameGrid_Border.Width = 320;
                GameGrid_Border.Height = 320;
            }
            else
            {
                double oneCellWidth = 320.0 / (Math.Max(Rows, Columns));
                GameGrid_Border.Height = Rows * oneCellWidth;
                GameGrid_Border.Width = Columns * oneCellWidth;
            }
        }
        private void Initalize_GridBordersAndImagesMatrixes()
        {
            GridBorders = new Border[Rows, Columns];
            GridImages = new Image[Rows, Columns];
        }
        private byte Get_LetterFontSize_DependingOnGridSize()
        {
            int biggestValue = Math.Max(Rows, Columns);
            if (biggestValue > 12) return 8;
            else if (biggestValue <= 5) return 15;
            else return 12;
        }
        private RotateTransform Get_RotateTransformForCurrentBoard()
        {
            if (GameSession.MainPlayerColor == Color.Black) return new RotateTransform(180);
            else return new RotateTransform(0);
        }
        private Image Create_ImageForPiece_LocatedOnCell(ICell cell)
        {
            return new Image()
            {
                Source = (!cell.IsEmpty) ? PreparedImages.PieceNameToBitmap[cell.Piece!.GetDistinguishName] : null,

                //щоб трансформація відбувалась відносно центра(повороти, масштабування і т.д.)
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
                //масштаб картинки = 80%
                RenderTransform = new ScaleTransform(0.8, 0.8)
            };
        }
        private void Add_LetterToGrid(Grid grid, byte x, byte y, byte letterFontSize)
        {
            if ((x == Rows - 1 && GameSession.MainPlayerColor == Color.White) || (x == 0 && GameSession.MainPlayerColor == Color.Black))
            {
                TextBlock letter = new()
                {
                    Margin = new Thickness(0, 4, 0.5, 0),
                    Text = GameBoard.IntegerToLetter[y],
                    FontSize = letterFontSize,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                };
                grid.Children.Add(letter);
            }
        }
        private void Add_NumberToGrid(Grid grid, byte x, byte y, byte letterFontSize)
        {
            if ((y == 0 && GameSession.MainPlayerColor == Color.White) || (y == Columns - 1 && GameSession.MainPlayerColor == Color.Black))
            {
                TextBlock number = new()
                {
                    Text = (Rows - x).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    FontSize = letterFontSize,
                    Margin = new Thickness(0, 0, 0, 7)
                };
                grid.Children.Add(number);
            }
        }
        #endregion
        #endregion



        //=================================================================================================================
        #region Undo_Redo_Buttons
        private void OnClick_Undo(object sender, MouseButtonEventArgs e)
        {
            if (GameSession.CurrentGameBoardSettings.GameMode == GameModeType.EvE)
            {
                OnClick_StopAI_Battle(this, new RoutedEventArgs());
            }

            Paint_CellsInoriginalColor_WhereKingsFiguresInDanger();
            Clear_PossibleMoves();
            Clear_SellectedCell();
            GameSession.Undo();
        }
        private void OnClick_Redo(object sender, MouseButtonEventArgs e)
        {
            if (GameSession.CurrentGameBoardSettings.GameMode == GameModeType.EvE)
            {
                OnClick_StopAI_Battle(this, new RoutedEventArgs());
            }

            Paint_CellsInoriginalColor_WhereKingsFiguresInDanger();
            Clear_PossibleMoves();
            Clear_SellectedCell();
            GameSession.Redo();
        }
        #endregion




        





        //=================================================================================================================
        #region ELIPSES
        private async void OnMouseLeftButtonDown_Elipse(object sender, MouseButtonEventArgs e)
        {
            //синхронызуэмо процеси
            if (semaphoreSlim.CurrentCount == 0) return;
            await semaphoreSlim.WaitAsync();

            
            try
            {
                if (IsGameStarted && sender is Ellipse ellipse && ellipse.DataContext is Move move)
                {
                    Paint_CellsInoriginalColor_WhereKingsFiguresInDanger();
                    Clear_PossibleMoves();
                    Clear_SellectedCell();
                    await Task.Run(async () =>
                    {
                        await GameSession.Make_Move(move, CancellationTokenSource!.Token);
                    });
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
            
        }
        private void OnMouseLeave_Elipse(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                ellipse.Fill = Brushes.Gray;
            }
        }
        private void OnMouseEnter_Elipse(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                ellipse.Fill = Brushes.IndianRed;
            }
        }
        #endregion


        //=================================================================================================================
        #region CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            GameSession = new PVP_GameSession();
            DarkCellBackgroundBinding.Source = this;
            LightCellBackgroundBinding.Source = this;
            IsSoundOn = true;
            DataContext = this;

            Initialize_MenuItems_AndBackgroundForIt_UsingBoards_LocatedInFolder(CustomBoards_MenuItem, "Custom boards", "#9bf743");
            Initialize_MenuItems_AndBackgroundForIt_UsingBoards_LocatedInFolder(TestingBoards_MenuItem, "Testing boards", "#ee81e2");
            Initialize_MenuItems_ForHelpMenuItem();
            Initialize_MenuItems_ForAboutMenuItem();
            Initialize_ThemesComboobx();
            Initialize_GameGrid();
            Initialize_Handlers();
        }
        #endregion


        //=================================================================================================================
        #region PROMOTION BUTTONS
        private async void OnMouseLeftButtonDown_PromotionImage(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ushort pieceID)
            {
                await GameSession.Make_Promotion(pieceID, CancellationTokenSource!.Token);
            }
        }
        private void OnMouseEnter_PromotionImage(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                image.Opacity = 0.6;
            }
        }
        private void OnMouseLeave_PromotionImage(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                image.Opacity = 1;
            }
        }
        #endregion


        //=================================================================================================================
        #region DELAY WINDOW
        private async Task Show_DelayScreen(string message, int delay, CancellationToken token)
        {
            await Dispatcher.Invoke(async () =>
            {
                if (delay <= 0) return;

                Overlay_Textblock.Text = $"{message} in: {delay}";
                DelayScreen_Border.Visibility = Visibility.Visible;

                while (delay > 0)
                {
                    Overlay_Textblock.Text = $"{message} in: {delay}";
                    await Task.Delay(900);
                    delay--;

                    if (token.IsCancellationRequested)
                    {
                        DelayScreen_Border.Visibility = Visibility.Hidden;
                        return;
                    }
                }

                On_SoundPlay("castle.mp3");
                DelayScreen_Border.Visibility = Visibility.Hidden;
            });
        }
        #endregion


        //=================================================================================================================
        #region NOTIFY PROPERTY CHANGES
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        //=================================================================================================================
        #region COMBOBOXES
        private void OnSelectionChanged_Difficulty_Combobox(object sender, SelectionChangedEventArgs e)
        {
            if (Play_As_Random_RadioButton.IsChecked == true && GameType_PvE.IsChecked == true)
            {
                if (sender == BlackSide_Difficulty_ComboBox)
                {
                    WhiteSide_Difficulty_ComboBox.SelectedIndex = BlackSide_Difficulty_ComboBox.SelectedIndex;
                }
                else BlackSide_Difficulty_ComboBox.SelectedIndex = WhiteSide_Difficulty_ComboBox.SelectedIndex;
            }
        }
        private void OnSelectionChanged_Themes_ComboBox(object sender, SelectionChangedEventArgs e)
        {
            if (Theme_ComboBox.SelectedItem is WindowsThemeInfo info)
            {
                CurrentTheme = info;
            }
            else CurrentTheme = LightTheme;
        }
        #endregion


        //=================================================================================================================
        #region START NEW GAME RADIOBUTTONS

        /// <summary>
        /// WE DONT NEED TO CHOSE AI DIFFICULT IF WE WANT TO PLAY WITH FRIEND
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameType_PvP_Checked(object sender, RoutedEventArgs e)
        {
            GameMode = GameModeType.PvP;

            BlackSide_Difficulty_ComboBox.IsEnabled = false;
            WhiteSide_Difficulty_ComboBox.IsEnabled = false;
        }
        /// <summary>
        /// IF CHECKED PLAYER VS COMPUTER, UNLOCK PROPRIATE COMBOBOX FOR AI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameType_PvE_Checked(object sender, RoutedEventArgs e)
        {
            GameMode = GameModeType.PvE;

            if (Play_As_Random_RadioButton.IsChecked == false)
            {
                if (Play_As_White_RadioButton.IsChecked == true)
                {
                    BlackSide_Difficulty_ComboBox.IsEnabled = true;
                }
                else BlackSide_Difficulty_ComboBox.IsEnabled = false;

                if (Play_As_Black_RadioButton.IsChecked == true)
                {
                    WhiteSide_Difficulty_ComboBox.IsEnabled = true;
                }
                else WhiteSide_Difficulty_ComboBox.IsEnabled = false;
            }
            else
            {
                WhiteSide_Difficulty_ComboBox.IsEnabled = true;
                BlackSide_Difficulty_ComboBox.IsEnabled = true;

                BlackSide_Difficulty_ComboBox.SelectedIndex = WhiteSide_Difficulty_ComboBox.SelectedIndex;
            }
        }
        /// <summary>
        /// IF EVE CHECKED, THEN ALL CIMBOBOX SHOULD BE ENABLED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameType_EvE_Checked(object sender, RoutedEventArgs e)
        {
            GameMode = GameModeType.EvE;

            BlackSide_Difficulty_ComboBox.IsEnabled = true;
            WhiteSide_Difficulty_ComboBox.IsEnabled = true;
        }



        /// <summary>
        /// ENABLE PROPRIATE COMBOBOXES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayAsWhite_RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            MainPlayerColor = Color.White;

            if (GameType_PvE.IsChecked == true)
            {
                BlackSide_Difficulty_ComboBox.IsEnabled = true;
                WhiteSide_Difficulty_ComboBox.IsEnabled = false;
            }
            else if (GameType_EvE.IsChecked == true)
            {
                BlackSide_Difficulty_ComboBox.IsEnabled = true;
                WhiteSide_Difficulty_ComboBox.IsEnabled = true;
            }
        }
        /// <summary>
        /// ENABLE PROPRIATE COMBOBOXES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayAsBlack_RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            MainPlayerColor = Color.Black;

            if (GameType_PvE.IsChecked == true)
            {
                BlackSide_Difficulty_ComboBox.IsEnabled = false;
                WhiteSide_Difficulty_ComboBox.IsEnabled = true;
            }
            else if (GameType_EvE.IsChecked == true)
            {
                BlackSide_Difficulty_ComboBox.IsEnabled = true;
                WhiteSide_Difficulty_ComboBox.IsEnabled = true;
            }
        }
        /// <summary>
        /// ENABLE ALL OMBOBOXES IF GAME MODE IS PvE AN MAKE DIFFICULT SAME FOR BOTH SIDES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayAsRandom_RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            MainPlayerColor = Color.Random;

            if (GameType_PvE.IsChecked == true)
            {
                WhiteSide_Difficulty_ComboBox.IsEnabled = true;
                BlackSide_Difficulty_ComboBox.IsEnabled = true;

                BlackSide_Difficulty_ComboBox.SelectedIndex = WhiteSide_Difficulty_ComboBox.SelectedIndex;
            }

        }
        #endregion


        //=================================================================================================================
        #region START NEW GAME TEXT BOXES
        private void PreviewTextInput_Timer_TextBox(object sender, TextCompositionEventArgs e)
        {
            // Перевірка, чи введений символ є цифрою
            Regex regex = new("[^0-9]+");
            //Regex regex = new(@"^[0-9]+$");
            e.Handled = regex.IsMatch(e.Text);

            // Перевірка, чи довжина тексту не перевищує 4 символи
            if (((TextBox)sender).Text.Length >= 4 || e.Text.Contains(' '))
            {
                e.Handled = true; // Заборона введення символу
            }
        }
        private void PreviewTextInput_MovesLimit_TextBox(object sender, TextCompositionEventArgs e)
        {
            // Перевірка, чи введений символ є цифрою
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            // Перевірка, чи довжина тексту не перевищує 4 символи
            if (((TextBox)sender).Text.Length >= 3)
            {
                e.Handled = true; // Заборона введення символу
            }
        }
        private void PreviewKeyDown_IgnoreSpaces_TextBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Заборонити обробку події (ігнорувати введений пробіл)
            }
        }
        #endregion
    }
}
