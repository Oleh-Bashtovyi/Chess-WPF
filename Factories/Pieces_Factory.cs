using Chess_game.Behaviour;
using Chess_game.Behaviour.Commands;
using Chess_game.Controls;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Chess_game.Factories
{
    public static class Pieces_Factory
    {






        //create list of piece, so that we can clone it by ID



        public static Dictionary<int, IChessPieceInfo_ForCreation> ID_To_ChessPieceInfo_Storage;

        //=================================================================================================================
        // CONSTRUCCTORS
        //=================================================================================================================
        static Pieces_Factory()
        {
            ID_To_ChessPieceInfo_Storage = new();

            IPlayableChessPiece piece = CreateKing(Color.None, 1);
            ChessPieceInfo_ForCreation info = new(piece);
            ID_To_ChessPieceInfo_Storage.Add(1, info);

            piece = CreatePawn(Color.None, Direction.Up, 2);
            info = new(piece);
            info.Description = "Pawn that moves up ahead";
            ID_To_ChessPieceInfo_Storage.Add(2, info);

            piece = CreatePawn(Color.None, Direction.Down, 3);
            info = new(piece);
            info.Description = "Pawn that moves downside";
            ID_To_ChessPieceInfo_Storage.Add(3, info);

            piece = CreateKnight(Color.None, 4);
            info = new(piece);
            ID_To_ChessPieceInfo_Storage.Add(4, info);

            piece = CreateBishop(Color.None);
            info = new(piece);
            ID_To_ChessPieceInfo_Storage.Add(5, info);

            piece = CreateRock(Color.None);
            info = new(piece);
            ID_To_ChessPieceInfo_Storage.Add(6, info);

            piece = CreateQueen(Color.None);
            info = new(piece);
            ID_To_ChessPieceInfo_Storage.Add(7, info);

            /*            //...
                        int id = 0;
                        if (ID_To_ChessPieceInfo_Storage.ContainsKey(id))
                        {
                            throw new ArgumentException("Saved pieces contains pieces with same ID, further program run can not be continued!");
                        }
                        else
                        {
                            //....
                            ChessPieceInfo_ForCreation pieceCopy = new(id);
                            pieceCopy.IsOriginalPieceFromGame= true;
                            ID_To_ChessPieceInfo_Storage.Add(id, pieceCopy);
                        }*/
        }

        












        public static IPlayableChessPiece? Create_ChessPiece_UsingID_ForColor(int id, Color color)
        {

            if (!ID_To_ChessPieceInfo_Storage.ContainsKey(id))
            {
                return null;
            }
            else
            {
                IChessPieceInfo_ForCreation info = ID_To_ChessPieceInfo_Storage[id];

                ChessPiece piece = new(info, color);

                return piece;
            }
        }


        public static bool Contains_Piece_WithID(int id)
        {
            return ID_To_ChessPieceInfo_Storage.ContainsKey(id);
        }




        /*        /// <summary>
                /// РОЗШИФРОВУЄ КОДУВАННЯ, ЩО ЗАСТОСОВУЄТЬСЯ ДЛЯ ЕФЕКТИВНОГО ЗБЕРЕЖЕННЯ ФІГУР
                /// </summary>
                /// <param name="encoding"></param>
                /// <returns></returns>
                /// <exception cref="Exception"></exception>
                public static IPlayableChessPiece? Create_ChessPiece_UsingEncoding(string encoding)
                {
                    //перевіримо чи не пусте кодування. Якщо пусте, значить там не було фігури
                    if (encoding.ToLower() == "null") return null;

                    //Розділимо наше кодування
                    List<string> codes = encoding.Split(":", StringSplitOptions.None).ToList();

                    //спочатку намагаємось декодувати перші дві цифри. Перша повинна означати id, а друга колір
                    if(codes.Count < 2 || 
                        !short.TryParse(codes[0], out short id) ||
                        !Enum.TryParse(typeof(Color), codes[1], out object? pieceColor))
                    {
                        throw new Exception("Wrong piece encoding!");
                    }

                    //перевіримо наявність фігури з таким id у списку фігур, якщо її нема, то створити фігуру неможливо.
                    //Також перевіримо правильність кодування кольору. Колір повинен бути або чорним, або білим.
                    //якщо жодна з умов не виконується, то повертаємо пусту фігуру
                    if (!ID_To_ChessPieceInfo_Storage.TryGetValue(id, out IChessPieceInfo_ForCreation? info) ||
                        !(pieceColor is Color color) || 
                        (color != Color.White && color != Color.Black) ) return null;

                    //створимо фігуру
                    ChessPiece piece = new(info, color);



                    //далі йдутьт деталі. Третя цифра - коли фігура була вперше зіграна
                    if(codes.Count> 2)
                    {
                        if (short.TryParse(codes[2], out short wasFirstPlayed))
                        {
                            piece.WasFirstlyPlayed = wasFirstPlayed;
                        }

                        //en passant cell
                        if(codes.Count > 3)
                        {
                            if (codes[3].Length == 2 && GameBoard.IntegerToLetter.ContainsValue(codes[3][0].ToString()) && char.IsDigit(codes[3][1]))
                            {
                                //дороби en passant
                            }

                        }
                    }

                    return piece;
                }*/














        private static Dictionary<string, Action<ChessPiece, string[]>> EncodedProperty_ToValueAssignMethod = new()
        {
            {"fp", MarkWhenWasFirstPlayed },
            {"en", AddEnPassantCellToBeatPiece }
        };




        /// <summary>
        /// РОЗШИФРОВУЄ КОДУВАННЯ, ЩО ЗАСТОСОВУЄТЬСЯ ДЛЯ ЕФЕКТИВНОГО ЗБЕРЕЖЕННЯ ФІГУР
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// 
        //3:1|fpl=16;enp=b2

        public static IPlayableChessPiece? Create_ChessPiece_UsingEncoding(string encoding)
        {
            //перевіримо чи не пусте кодування. Якщо пусте, значить там не було фігури
            if (encoding.ToLower() == "null") return null;

            //Розділимо наше кодування, вийде щось подібне:
            //3:1|fp=16;en=b2   =>     [ "3:1", "fp=16;en=b2" ]   або  [ "3:1" ], якщо ддодаткова додаткова інформація відсутня
            string[] parts = encoding.Split("|", StringSplitOptions.RemoveEmptyEntries).ToArray();

            if(parts.Length != 2 && parts.Length != 1) throw new Exception(
                $"Wrong piece encoding in \"{encoding}\"!\n " +
                "Possible reasons:\n" +
                "1) Code have more than 1 \"|\" character\n"+
                "2) Code dont have encoding (check whether somethisng is written in saved file)\n"+
                "3) \"|\" should split code into 2 parts like:   2:1|fp=3   where first part is main info and second - addition info (optionaly). If There is no addition info, deleate  \"|\" character");

            //Розділимо першу головну частину:
            //"3:1"  => ["3", "1"]
            string[] mainCodes = parts[0].Split(":", StringSplitOptions.RemoveEmptyEntries).ToArray();

            if (mainCodes.Length != 2) throw new Exception(
                $"Wrong piece encoding in \"{parts[0]}\"! \n" +
                "\nPossible reasons:\n" +
                "1) main part of encoding should look like  \"2:1\" - where first number is ID and second is color\n" +
                "2) encoding should not contain more than 1 \":\" character and not split it in more than 2 parts.");


            //спочатку намагаємось декодувати перші дві цифри. Перша повинна означати id, а друга колір
            //Також перевіримо правильність кодування кольору. Колір повинен бути або чорним, або білим.
            if (!byte.TryParse(mainCodes[0], out byte id) ||
                !Enum.TryParse(typeof(Color), mainCodes[1], out object? pieceColor) ||
                !(pieceColor is Color color) ||
                (color != Color.White && color != Color.Black))
            {
                throw new Exception(
                    $"Wrong piece encoding in main part \"{parts[0]}\"!\n " +
                    "Possible reasons:\n" +
                    "1) First number should be ID beetwean 0 and 65535. \n" +
                    "2) Second number represents color. It should be 0 - black or 1 - white\n");
            }

            //перевіримо наявність фігури з таким id у списку фігур, якщо її нема, то створити фігуру неможливо.
            if (!ID_To_ChessPieceInfo_Storage.TryGetValue(id, out IChessPieceInfo_ForCreation? info)) return null;

            //створимо фігуру
            ChessPiece piece = new(info, color);

            

            //далі йдутьт деталі.
            if(parts.Length == 2)
            {
                //розділимо строку деталей, вийде щось подібне:
                //fp=16;en=2,1   =>    [ "fp=16" , "en=2,1" ]
                string[] propertiesAndValues = parts[1].Split(";", StringSplitOptions.RemoveEmptyEntries);

                //кожну властивість розшифруємо
                foreach(string property in propertiesAndValues)
                {
                    //назву властивості та значення розділяє знак дорівнює
                    string[] twoValues = property.Split("=", StringSplitOptions.RemoveEmptyEntries).ToArray();

                    //перевіримо чи є у нас 2 частини
                    if (twoValues.Length != 2) throw new Exception(
                        $"Wrong piece encoding in \"{property}\"!\n" +
                        "Possible Reasons:\n" +
                        "1) Addition info about piece should be encoded in format \"fp=3\" or \"en=3,1\" e.t.c.\n" +
                        "2) Encoding should have only one \"=\" character and this character should split encoding on only two parts" +
                        "3) Format like \"=3\" or \"fp=\" or \"=\" leads to error.");

                    if (!EncodedProperty_ToValueAssignMethod.ContainsKey(twoValues[0]))
                    {
                        throw new Exception(
                            $"Error occured when assigning value to property \"{property}\"!\n" +
                            "Program doesnt have any information about such property.");
                    }
                    else
                    {
                        EncodedProperty_ToValueAssignMethod[twoValues[0]].Invoke(piece, twoValues);
                    }
                }
            }
            return piece;
        }




        private static void MarkWhenWasFirstPlayed(ChessPiece piece, string[] propertyAndValues)
        {
            if (short.TryParse(propertyAndValues[1], out short wasFirstPlayed))
            {
                piece.WasFirstlyPlayed = wasFirstPlayed;
            }
            else throw new Exception(
                $"Wrong encoding in property \"{propertyAndValues[0]}\" with value \"{propertyAndValues[1]}\"!\n" +
                "Possible reasons:\n" +
                "1) value should be value should be beetwean 0 and 65535.\n" +
                "2) value should contain only numbers.");
        }

        private static void AddEnPassantCellToBeatPiece(ChessPiece piece, string[] propertyAndValues)
        {
            //для en passant нам потрібно знати координати клітини для побиття фігури.
            //ці координати мають бути представлені у вигляді "3,1"
            string[] values = propertyAndValues[1].Split(',');
            if (values.Length == 2)
            {
                if (byte.TryParse(values[0], out byte x) && byte.TryParse(values[1], out byte y))
                {
                    piece.EnPassantCellToBeatPiece = (x, y);
                }
                else throw new Exception(
                    $"Wrong encoding in property \"{propertyAndValues[0]}\" with value \"{propertyAndValues[1]}\"!\n" +
                    "Both values should be represented as BYTE number beetwean 0 and 255!");
            }
            else throw new Exception(
                $"Wrong encoding in property \"{propertyAndValues[0]}\" with value \"{propertyAndValues[1]}\"!\n" +
                "Possible reasons:\n" +
                "1) Value should be represented like \"3,1\", where first number is X coordinate and second is Y coordinate.\n" +
                "2) This values should be separated with only one COMA." +
                "3) Code represented like \",1\" or \"3,\" or \",\" or \"\" or \"3,1,1\" et.c. leads to error.");
        }
        




        //=================================================================================================================
        // GET ATRIBUTES USING ID
        //=================================================================================================================
        public static string Get_PieceDistinguishingName_UsingID_ForColor(int id, Color color)
        {
            ID_To_ChessPieceInfo_Storage.TryGetValue(id, out IChessPieceInfo_ForCreation? value);

            return value?.Get_DistinguishingName_UsingColor(color) ?? string.Empty;
        }

        public static string Get_PieceName_UsingID(ushort id)
        {
            ID_To_ChessPieceInfo_Storage.TryGetValue(id, out IChessPieceInfo_ForCreation? value);

            return value?.Name ?? string.Empty;
        }

        public static int Get_PieceValuability_UsingID(ushort id)
        {
            ID_To_ChessPieceInfo_Storage.TryGetValue(id, out IChessPieceInfo_ForCreation? value);

            return value?.Valuability ?? 0;
        }




        //=================================================================================================================
        // CREATE PREPARED PIECES
        //=================================================================================================================
        #region Create prepared pieces
        private static IPlayableChessPiece CreateQueen(Color color)
        {
            ChessPiece piece = new ChessPiece(7, "Queen", color, 900);

            /*            Move_And_Atack_Command move_And_Atack_Command = new Move_And_Atack_Command(MoveDirections_Factory.GetQueenMovesDirections());

                        piece.MoveCommands.Add(move_And_Atack_Command);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(7);

            return piece;
        }
        private static IPlayableChessPiece CreateKnight(Color color, byte id)
        {
            ChessPiece piece = new ChessPiece(id, "Knight", color, 300);

            /*            Move_And_Atack_Command move_And_Atack_Command = new Move_And_Atack_Command(MoveDirections_Factory.GetKnightMovesDirections());

                        piece.MoveCommands.Add(move_And_Atack_Command);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(id);

            return piece;
        }
        private static IPlayableChessPiece CreateRock(Color color)
        {
            ChessPiece piece = new ChessPiece(6, "Rock", color, 500);

            /*            Move_And_Atack_Command move_And_Atack_Command = new Move_And_Atack_Command(MoveDirections_Factory.GetRockMovesDirections());

                        piece.MoveCommands.Add(move_And_Atack_Command);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(6);

            piece.IsCastleSource= true;

            return piece;
        }
        private static IPlayableChessPiece CreateBishop(Color color)
        {
            ChessPiece piece = new ChessPiece(5, "Bishop", color, 300);

            /*            Move_And_Atack_Command move_And_Atack_Command = new Move_And_Atack_Command(MoveDirections_Factory.GetBishopMovesDirections());

                        piece.MoveCommands.Add(move_And_Atack_Command);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(5);

            return piece;
        }
        private static IPlayableChessPiece CreatePawn(Color color, Direction walkDirection, byte id)
        {
            

            ChessPiece piece = new ChessPiece(id, "Pawn", color, 100);

            /*            Atack_Command atack_Command = new Atack_Command(MoveDirections_Factory.GetPawnAtacksDirection(walkDirection));
                        PawnMove_Command pawnMove_Command = new PawnMove_Command(walkDirection);
                        PawnEnPassant_Command pawnEnPassant = new PawnEnPassant_Command(MoveDirections_Factory.GetPawnAtacksDirection(walkDirection));

                        piece.MoveCommands.Add(atack_Command);
                        piece.MoveCommands.Add(pawnMove_Command);
                        piece.MoveCommands.Add(pawnEnPassant);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(id);

            piece.PromotionListID = 1;
            return piece;
        }
        private static IPlayableChessPiece CreateKing(Color color, byte id)
        {
            ChessPiece piece = new ChessPiece(id, "King", color, 20000);

            /*            Move_And_Atack_Command move_And_Atack_Command = new Move_And_Atack_Command(MoveDirections_Factory.GetKingMovesDirections());
                        CastleMove_Command castleMove_Command = new CastleMove_Command(MoveDirections_Factory.GetKingsCastleDirections());

                        piece.MoveCommands.Add(move_And_Atack_Command);
                        piece.MoveCommands.Add (castleMove_Command);*/

            piece.MoveCommands = PieceMoveCommands_Factory.Get_PieceMoveCommand_UsingID(1);

            piece.IsKingFigure = true;
            piece.IsAbleToCastle= true;

            return piece;
        }
    }
#endregion
}
