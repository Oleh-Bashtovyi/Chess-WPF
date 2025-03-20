using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Extensions;
using Chess_game.Factories;
using Chess_game.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Chess_game.Game_Data
{
    public static class LoadService
    {







        //private static readonly Dictionary<string, string> ConvertSettingsPropertyName_ToReadableView





        public static List<BoardInfo> Load_AllBoardsInfo_UsingPath(string pathInGameDataFolder)
        {
            //подивимось чи існує папка для збереження ігрових даних
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", pathInGameDataFolder);
            //якщо її немає, то створимо її і вийдемо з метода
            if (File.Exists(path))
            {
                Directory.CreateDirectory(path);
                return new List<BoardInfo>();
            }


            string[] files = Directory.GetFiles(path, "*.json");

            List<BoardInfo> boards = new List<BoardInfo>();

            foreach (string jsonFile in files)
            {

                try
                {
                    using StreamReader reader = new StreamReader(jsonFile); // Відкриваємо файл для читання
                    using JsonTextReader jsonReader = new JsonTextReader(reader); // Створюємо JSON Text Reader
                    JObject jobj = JObject.Load(jsonReader); // Завантажуємо JSON об'єкт з Text Reader


                    if (Check_BoardSavedFormatValidation(jobj))
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        //спочатку спробуємо отримати назву сесії
                        if (!jobj.TryGet_StringValue("Name", out string boardName))
                        {
                            boardName = "No name";
                        }


                        //далі спробуємо  отримати опис ігрової сесії, якщо він є
                        if (jobj.TryGet_StringValue("Description", out string descriptionResult))
                        {
                            stringBuilder.Append($"Description:\n{descriptionResult}\n\n");
                        }
                        else stringBuilder.Append("Description:\n-No Description.\n");
                        stringBuilder.Append("========================\n");

                        //далі спробуємо отримати заготовлені налаштування, тобто їх назву та значення, щоб додати в опис
                        JToken? token = jobj[nameof(AbstractGameSession.CurrentGameBoardSettings)];
                        stringBuilder.Append("Startup settings:\n");

                        if (token != null && token.Children().Any())
                        {
                            PopulateDescription_ForBoardInfo(stringBuilder, token);
                        }
                        else stringBuilder.Append("-No settings.\n");
                        stringBuilder.Append("========================\n");

                        //створюємо обєк інформації та додаємо в список
                        boards.Add(
                            new BoardInfo(
                                boardName,
                                jsonFile,
                                stringBuilder.ToString()
                            ));

                    }
                }
                catch (Exception) { }

            }
            return boards;
        }



        private static void PopulateDescription_ForBoardInfo(StringBuilder sb, JToken token)
        {
            //main player color
            //==============================================================================================
            if (token.TryGet_EnumValue(nameof(GameStartupSettings.MainPlayerColor), out Color mainPlyerColor))
            {
                sb.AppendLine($"-Main player: {mainPlyerColor.ToString()}");
            }
            else
            {
                mainPlyerColor = Color.None;
                sb.AppendLine($"-Main player: ANY");
            }
            //gamemode
            //==============================================================================================
            if (token.TryGet_EnumValue(nameof(GameStartupSettings.GameMode), out GameModeType gameModeType))
            {
                sb.AppendLine($"-Game mode: {gameModeType.ToString()}");
            }
            else
            {
                sb.AppendLine($"-Game mode: ANY");
                gameModeType= GameModeType.None;
            }
            sb.AppendLine($"--------------------");


            //ai difficulty
            //==============================================================================================
            if(gameModeType == GameModeType.None || gameModeType == GameModeType.EvE ||
                (gameModeType == GameModeType.PvE && mainPlyerColor != Color.White))
            {

                if(token.TryGet_EnumValue(nameof(GameStartupSettings.WhitesDifficulty), out Difficulty whiteDifficulty) )
                {
                    sb.AppendLine($"-White AI difficulty: {whiteDifficulty}");
                }
                else sb.AppendLine($"-White AI difficulty: ANY");
            }
            else sb.AppendLine($"-White AI difficulty: -");

            if (gameModeType == GameModeType.None || gameModeType == GameModeType.EvE ||
                 (gameModeType == GameModeType.PvE && mainPlyerColor != Color.Black))
            {

                if (token.TryGet_EnumValue(nameof(GameStartupSettings.BlacksDifficulty), out Difficulty blackDifficulty))
                {
                    sb.AppendLine($"-Black AI difficulty: {blackDifficulty}");
                }
                else sb.AppendLine($"-Black AI difficulty: ANY");
            }
            else sb.AppendLine($"-Black AI difficulty: -");





            /*            if (gameModeType == GameModeType.None || gameModeType == GameModeType.EvE ||
                mainPlyerColor == Color.None || mainPlyerColor == Color.Random)
            {
                //white
                if (token.TryGet_EnumValue(nameof(GameStartupSettings.WhitesDifficulty), out Difficulty whiteDifficulty))
                {
                    sb.AppendLine($"-White AI difficulty: {whiteDifficulty}");
                }
                else sb.AppendLine($"-White AI difficulty: ANY");

                //black
                if (token.TryGet_EnumValue(nameof(GameStartupSettings.BlacksDifficulty), out Difficulty blackDifficulty))
                {
                    sb.AppendLine($"-Black AI difficulty: {blackDifficulty}");
                }
                else sb.AppendLine($"-Black AI difficulty: ANY");
            }
            else
            {
                //white
                if (token.TryGet_EnumValue(nameof(GameStartupSettings.WhitesDifficulty), out Difficulty whiteDifficulty))
                {
                    sb.AppendLine($"-White AI difficulty: {whiteDifficulty}");
                }
                else sb.AppendLine($"-White AI difficulty: -");

                //black
                if (token.TryGet_EnumValue(nameof(GameStartupSettings.BlacksDifficulty), out Difficulty blackDifficulty))
                {
                    sb.AppendLine($"-Black AI difficulty: {blackDifficulty}");
                }
                else sb.AppendLine($"-Black AI difficulty: ANY");
            }*/

            /*            if ((gameModeType == GameModeType.EvE || mainPlyerColor == Color.Black) &&
                            token.TryGet_EnumValue(nameof(GameStartupSettings.WhitesDifficulty), out Difficulty whiteDifficulty))
                        {
                            sb.AppendLine($"-White AI difficulty: {whiteDifficulty}");
                        }
                        else sb.AppendLine($"-White AI difficulty: ANY");*/

            /*            if ((gameModeType == GameModeType.EvE || mainPlyerColor == Color.White) &&
                            token.TryGet_EnumValue(nameof(GameStartupSettings.BlacksDifficulty), out Difficulty blackDifficulty))
                        {
                            sb.AppendLine($"-Black AI difficulty: {blackDifficulty}");
                        }
                        else sb.AppendLine($"-Black AI difficulty: ANY");*/
            sb.AppendLine($"--------------------");


            //moves limit
            //==============================================================================================
            bool isMovesLimitOnParsed = token.TryGet_BoolValue(nameof(GameStartupSettings.IsMovesLimitOn), out bool isMovesLimitOn);
            if (isMovesLimitOnParsed)
            {
                 sb.AppendLine($"-Moves limit on: " + (isMovesLimitOn ? "Yes" : "No")); 
            }
            else sb.AppendLine($"-Moves limit: ANY");


            if (isMovesLimitOnParsed)
            {
                if (isMovesLimitOn)
                {
                    if(token.TryGet_IntValue(nameof(GameStartupSettings.MovesLimit), out int movesLimit))
                    {
                        sb.AppendLine($"-Moves limit: " + movesLimit);
                    }
                    else sb.AppendLine($"-Moves limit: ANY");
                }
                else sb.AppendLine($"-Moves limit: -");
            }
            else
            {
                if (token.TryGet_IntValue(nameof(GameStartupSettings.MovesLimit), out int movesLimit))
                {
                    sb.AppendLine($"-Moves limit: " + movesLimit);
                }
                else sb.AppendLine($"-Moves limit: ANY");
            }

            /*            if (isMovesLimitOnParsed)
                        {
                            if(isMovesLimitOn &&token.TryGet_IntValue(nameof(GameStartupSettings.MovesLimit), out int movesLimit))
                            {
                                sb.AppendLine($"-Moves limit: " + movesLimit);

                            }
                            else sb.AppendLine($"-Moves limit: ANY");
                        }
                        else sb.AppendLine($"-Moves limit: ANY");*/
            sb.AppendLine($"--------------------");


            //timer
            //==============================================================================================
            bool isTimerOnParsed = token.TryGet_BoolValue(nameof(GameStartupSettings.IsTimerOn), out bool isTimerOn);
            if (isTimerOnParsed)
            {
                sb.AppendLine($"-Timer on: " + (isTimerOn ? "Yes" : "No"));
            }
            else sb.AppendLine($"-Timer on: ANY");


            if(isTimerOnParsed)
            {
                if(isTimerOn)
                {
                    if (token.TryGet_IntValue(nameof(GameStartupSettings.WhitesTimerDuration), out int whiteTimerDuration))
                    {
                        sb.AppendLine($"-White timer duration: {whiteTimerDuration}");
                    }
                    else sb.AppendLine($"-White timer duration: ANY");

                    if (token.TryGet_IntValue(nameof(GameStartupSettings.BlacksTimerDuration), out int blackTimerDuration))
                    {
                        sb.AppendLine($"-Black timer duration: {blackTimerDuration}");
                    }
                    else sb.AppendLine($"-Black timer duration: ANY");
                }
                else
                {
                    sb.AppendLine($"-White timer duration: -");
                    sb.AppendLine($"-Black timer duration: -");
                }
            }
            else
            {
                if (token.TryGet_IntValue(nameof(GameStartupSettings.WhitesTimerDuration), out int whiteTimerDuration))
                {
                    sb.AppendLine($"-White timer duration: {whiteTimerDuration}");
                }
                else sb.AppendLine($"-White timer duration: ANY");

                if (token.TryGet_IntValue(nameof(GameStartupSettings.BlacksTimerDuration), out int blackTimerDuration))
                {
                    sb.AppendLine($"-Black timer duration: {blackTimerDuration}");
                }
                else sb.AppendLine($"-Black timer duration: ANY");
            }
            sb.AppendLine($"--------------------");


            //fog of war
            //==============================================================================================
            if (token.TryGet_BoolValue(nameof(GameStartupSettings.IsFogOfWarOn), out bool isFogOfWarOn))
            {
                sb.AppendLine($"-Fog of war on: " + (isFogOfWarOn ? "Yes" : "No"));
            }
            else sb.AppendLine($"-Fog of war on: ANY");

        }
        //private void PopulateDescriptionForBoardInfo()







            /*        public static void Load_AllCustomBoards()
                    {
                        //подивимось чи існує папка для збереження ігрових даних
                        string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data", "Custom boards");
                        //якщо її немає, то створимо її і вийдемо з метода
                        if (File.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                            return;
                        }


                        string[] files = Directory.GetFiles(path, "*.json");

                        List<BoardInfo> boards = new List<BoardInfo>();

                        foreach (string jsonFile in files)
                        {

                            try
                            {
                                //using FileStream jsonContent = File.OpenRead(jsonFile);
                                //dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent.Read());

                                *//*                    using FileStream json = File.OpenRead(jsonFile);
                                                    JObject jobj = JsonSerializer.Deserialize(json);*//*

                                using StreamReader reader = new StreamReader(jsonFile); // Відкриваємо файл для читання
                                using JsonTextReader jsonReader = new JsonTextReader(reader); // Створюємо JSON Text Reader
                                JObject jobj = JObject.Load(jsonReader); // Завантажуємо JSON об'єкт з Text Reader


                                //string jsonContent = File.ReadAllText(jsonFile);
                                //dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent);
                            }
                            catch (Exception)
                            {

                            }



                           *//* using (FileStream fs = File.OpenRead(jsonFile))
                            {
                                try
                                {
                                    string jsonContent = File.ReadAllText(fs);
                                    *//*                        boards.Add
                                                                (
                                                                new BoardInfo
                                                                (

                                                                )
                                                                );
                                    *//*
                                }
                                catch (Exception)
                                {
                                }
                            }*//*
                        }*/






            /*                //throw new FileNotFoundException("Filename: " + path);

                        string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        path = System.IO.Path.Combine(path, "Saved boards");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }


                        path = System.IO.Path.Combine(path1, path);

                        return LoadService.Load_Game_UsingPath(path, gameStartupSettings);*/
        






        private static bool Check_BoardSavedFormatValidation(JObject data)
        {
            if (
                data.ContainsKey(nameof(AbstractGameSession.Board))
                //data.ContainsKey(nameof(GameSession.Rows)) &&
                //data.ContainsKey(nameof(GameSession.Columns)
                
                ) return true;


            return false;
        }




        /*        public static GameSession Load_Game_UsingPath(string path, GameStartupSettings settings)
                {
                    if (!File.Exists(path)) throw new FileNotFoundException("Filename: " + path);

             //       try
                    {
                        JObject data = JObject.Parse(File.ReadAllText(path));
                        return Create_GameSession(data, settings);
                    }
           //         catch (Exception)
                    {
               //         throw new FormatException($"Error reading file: {path}");
                    }
                }*/




        public static AbstractGameSession Load_Game_UsingPath(string path, GameStartupSettings? settings = null)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Filename: " + path);
            }

            JObject data = JObject.Parse(File.ReadAllText(path));
            return Create_GameSession(data, settings);
        }







        private static AbstractGameSession Create_GameSession(JObject data, GameStartupSettings? settings)
        {

            GameBoard gameBoard = PopulateBoard(data);

            GameStartupSettings gameStartupSettings = PopulateBoardStartupSettings(data, settings);




            switch (gameStartupSettings.GameMode)
            {
                case GameModeType.PvP:
                    return new PVP_GameSession
                                    (
                                        gameBoard,
                                        gameStartupSettings,
                                        data.TryGet_EnumValue(nameof(AbstractGameSession.CurrentPlayerMove), out Color color) ? color : Color.White,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.MovesLeft), out short movesLeft) ? movesLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForWhite), out short whiteTimeLeft) ? whiteTimeLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForBlack), out short blackTimeLeft) ? blackTimeLeft : (short)-1
                                    );
                case GameModeType.PvE:
                    return new PVE_GameSession
                                    (
                                        gameBoard,
                                        gameStartupSettings,
                                        data.TryGet_EnumValue(nameof(AbstractGameSession.CurrentPlayerMove), out color) ? color : Color.White,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.MovesLeft), out movesLeft) ? movesLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForWhite), out  whiteTimeLeft) ? whiteTimeLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForBlack), out  blackTimeLeft) ? blackTimeLeft : (short)-1
                                    );
                case GameModeType.EvE:
                    return new EVE_GameSession
                                    (
                                        gameBoard,
                                        gameStartupSettings,
                                        data.TryGet_EnumValue(nameof(AbstractGameSession.CurrentPlayerMove), out  color) ? color : Color.White,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.MovesLeft), out  movesLeft) ? movesLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForWhite), out  whiteTimeLeft) ? whiteTimeLeft : (short)-1,
                                        data.TryGet_ShortValue(nameof(AbstractGameSession.TimeLeftForBlack), out  blackTimeLeft) ? blackTimeLeft : (short)-1
                                    );
                default: throw new ArgumentException("No information about such gamemode!");
            }
        }





        /*        private static GameSession Create_GameSession(JObject data, GameStartupSettings? settings = null)
                {

                    GameBoard gameBoard = PopulateBoard(data);

                    GameStartupSettings gameStartupSettings = PopulateBoardStartupSettings(data, settings);

                    return new GameSession
                        (
                            gameBoard,
                            gameStartupSettings,
                            data.TryGet_EnumValue(nameof(GameSession.CurrentPlayerMove), out Color color) ? color : Color.White,
                            data.TryGet_ShortValue(nameof(GameSession.MovesLeft), out short movesLeft) ? movesLeft : (short)-1,
                            data.TryGet_ShortValue(nameof(GameSession.TimeLeftForWhite), out short whiteTimeLeft) ? whiteTimeLeft : (short)-1,
                            data.TryGet_ShortValue(nameof(GameSession.TimeLeftForBlack), out short blackTimeLeft) ? blackTimeLeft : (short)-1
                        );
                }*/






        /// <summary>
        /// СТВОРИТИ САМУ ДОШКУ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static GameBoard PopulateBoard(JObject data)
        { 
            //check file validation
            JToken? boardToken = data["Board"];

            if (boardToken == null)
            {
                throw new Exception("Board property is not presented in json file");
            }

            JToken? gridEncoding = boardToken[nameof(GameBoard.BoardEncoding)];

            if (gridEncoding == null)
            {
                throw new Exception("\"BoardEncoding\" property is not presented in json file");
            }

            if (!boardToken.TryGet_IntValue(nameof(GameBoard.Rows), out int rows) || !boardToken.TryGet_IntValue(nameof(GameBoard.Columns), out int cols))
            {
                throw new Exception("Rows or Columns property is not presented in json file");
            }

            //populate grid
            GameCell[][] populatedGameGrid = PopulateGrid(gridEncoding, rows, cols);

            //game board creation
            GameBoard gameBoard = new GameBoard(populatedGameGrid);

            //game board details
            gameBoard.IsBlackTeamCastled = boardToken.Get_BoolValue(nameof(GameBoard.IsBlackTeamCastled));
            gameBoard.IsWhiteTeamCastled = boardToken.Get_BoolValue(nameof(GameBoard.IsWhiteTeamCastled));
            //gameBoard.MoveNumber = (short)boardToken.Get_IntValue(nameof(GameBoard.MoveNumber));
            checked
            {
                gameBoard.MoveNumber = (short)boardToken.Get_IntValue(nameof(GameBoard.MoveNumber));
            }

            return gameBoard;
        }





        /// <summary>
        /// ЗАГРУЗИТИ МАТРИЦЮ ФІГУР
        /// </summary>
        /// <param name="tokenGrid"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static GameCell[][] PopulateGrid(JToken tokenGrid, int rows, int columns)
        {
            //create matrix
            GameCell[][] grid = new GameCell[rows][];
            for (int i = 0; i < rows; i++)
            {
                grid[i] = new GameCell[columns];
            }

            //fill matrix
            if(tokenGrid is JArray jArray)
            {
                foreach(var cellToken in jArray)
                {
                    if(cellToken != null)
                    {
                        GameCell cell = GameCells_Factory.Create_Cell_UsingCode(cellToken.ToString());

                        grid[cell.X][cell.Y] = cell;
                    }
                    else throw new Exception("Failed to read cell info!");
                }
            }
            else throw new Exception("Board encoding information is not presented or in wrong format!");


            //check matrix fillness
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (grid[i][j] == null)
                    {
                        throw new Exception($"Not all game cells where filled! some problems in encoding! \nEmpty encoding at: ({i};{j})");
                    }
                }
            }
            return grid;
        }




        /// <summary>
        /// ЗЧИТАТИ ІНФОРМАЦІЮ ПРО НАСТРОЙКИ ДОШКИ. ЯКЩО НАСТРОЙКИ ВІДСУТНІ, ТО ПРИСВОЇТИ НАСТРОЙКИ КОРИСТУВАЧА. 
        /// ЯКЩО НЕМА НАСТРОЙОК КОРИСТУВАЧА, ТО СТВОРИТИ СТАНДАРТНІ НАСТРОЙКИ
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userSettings"></param>
        /// <returns></returns>
        private static GameStartupSettings PopulateBoardStartupSettings(JObject data, GameStartupSettings? userSettings = null)
        {
            JToken? token = data[nameof(AbstractGameSession.CurrentGameBoardSettings)];

            //якщо токен не пустий, тобто інформація про настройки міститься у файлі
            if (token != null)
            {
                GameStartupSettings settings = new();

                //gamemode
                //==============================================================================================
                if (token.TryGet_EnumValue(nameof(GameStartupSettings.GameMode), out GameModeType gameModeType))
                {
                    settings.GameMode = gameModeType;
                }
                else settings.GameMode = userSettings?.GameMode ?? GameModeType.PvP;


                //main player color
                //==============================================================================================
                if(token.TryGet_EnumValue(nameof(GameStartupSettings.MainPlayerColor), out Color color))
                {
                    settings.MainPlayerColor = color;
                }
                else settings.MainPlayerColor = userSettings?.MainPlayerColor ?? Color.White;


                //ai difficulty
                //==============================================================================================
                if (settings.GameMode == GameModeType.PvE || settings.GameMode == GameModeType.EvE)
                {
                    if (token.TryGet_EnumValue(nameof(GameStartupSettings.WhitesDifficulty), out Difficulty whiteDifficulty))
                    {
                        settings.WhitesDifficulty = whiteDifficulty;
                    }
                    else settings.WhitesDifficulty = userSettings?.WhitesDifficulty ?? Difficulty.Easy;

                    if (token.TryGet_EnumValue(nameof(GameStartupSettings.BlacksDifficulty), out Difficulty blackDifficulty))
                    {
                        settings.BlacksDifficulty = blackDifficulty;
                    }
                    else settings.BlacksDifficulty = userSettings?.BlacksDifficulty ?? Difficulty.Easy;
                }


                //moves limit
                //==============================================================================================
                if(token.TryGet_BoolValue(nameof(GameStartupSettings.IsMovesLimitOn), out bool isMovesLimitOn))
                {
                    settings.IsMovesLimitOn= isMovesLimitOn;
                }
                else settings.IsMovesLimitOn = userSettings?.IsMovesLimitOn ?? false;

                if(settings.IsMovesLimitOn && token.TryGet_IntValue(nameof(GameStartupSettings.MovesLimit), out int movesLimit))
                {
                    settings.MovesLimit= movesLimit;
                }
                else settings.MovesLimit= userSettings?.MovesLimit ?? 1000;


                //timer
                //==============================================================================================
                if(token.TryGet_BoolValue(nameof(GameStartupSettings.IsTimerOn), out bool isTimerOn))
                {
                    settings.IsTimerOn= isTimerOn;
                }
                else settings.IsTimerOn = userSettings?.IsTimerOn ?? false;

                if (settings.IsTimerOn)
                {
                    if(token.TryGet_IntValue(nameof(GameStartupSettings.WhitesTimerDuration), out int whiteTimerDuration))
                    {
                        settings.WhitesTimerDuration= whiteTimerDuration;
                    }
                    else settings.WhitesTimerDuration = userSettings?.WhitesTimerDuration ?? -1;

                    if (token.TryGet_IntValue(nameof(GameStartupSettings.BlacksTimerDuration), out int blackTimerDuration))
                    {
                        settings.BlacksTimerDuration = blackTimerDuration;
                    }
                    else settings.BlacksTimerDuration = userSettings?.BlacksTimerDuration ?? -1;
                }

                //fog of war
                //==============================================================================================
                if(token.TryGet_BoolValue(nameof(GameStartupSettings.IsFogOfWarOn), out bool isFogOfWarOn))
                {
                    settings.IsFogOfWarOn= isFogOfWarOn;
                }
                else settings.IsFogOfWarOn = userSettings?.IsFogOfWarOn ?? false;

                //finaly return
                //==============================================================================================
                return settings;
            }
            //==============================================================================================
            else if (userSettings == null)
            {
                return new GameStartupSettings();
            }
            else return userSettings;
        }
    }
}
