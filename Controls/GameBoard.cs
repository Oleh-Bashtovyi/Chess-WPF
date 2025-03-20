using Chess_game.Behaviour;
using Chess_game.Factories;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Chess_game.Extensions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using Chess_game.Behaviour.Interfaces;
using Newtonsoft.Json;
using Chess_game.Behaviour.Decorators.Get_Moves_Decorator;
using Chess_game.Models.Comparers;

namespace Chess_game.Controls
{
    public class GameBoard : IDisposable
    {
        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================


        public SortedSet<IGetMovesDecorator> GetMoves_Decorators { get; set; } = new(new GetMovesDecoratorComparer());



        #region BOARD
        [JsonIgnore] private GameCell[][] _board;
        [JsonIgnore] private GameCell[][] Board
        {
            get { return _board; }
            set { _board = value; }
        }
        [JsonIgnore] public ICell[][] GameGrid => _board;

        [JsonIgnore] public static readonly Dictionary<int, string> IntegerToLetter = new()
        {
            {0, "a" },
            {1, "b" },
            {2, "c" },
            {3, "d" },
            {4, "e" },
            {5, "f" },
            {6, "g" },
            {7, "h" },
            {8, "i" },
            {9, "j" },
            {10, "k" },
            {11, "l" },
            {12, "m" },
            {13, "n" },
            {14, "o" },
            {15, "p" },
        };
        public byte Rows { get; }
        public byte Columns { get;}
        public string[] BoardEncoding => Get_BoardEncoding();
        #endregion




        #region GAME PROCCES
        public bool IsPromotionNow { get; set; } = false;
        public bool IsWhiteTeamCastled { get; set; } = false;
        public bool IsBlackTeamCastled { get; set; } = false;
        public short MoveNumber { get; set; } = 0;
        public ICell? CurrentCellThatNustBePromoted { get; set; } = null;
        #endregion





        //=================================================================================================================
        // CONSTRUCTORS
        //=================================================================================================================
        #region CONSTRUCTORS
        public GameBoard(byte rows, byte columns)
        {
            if(rows <= 0 || rows > 16) throw new ArgumentException("Rows can not be less or equal 0 or bigger than 16!");
            if(columns <= 0 || columns > 16) throw new ArgumentException("Columns can not be less or equal 0 or bigger than 16!");        

            Rows = rows;
            Columns = columns;
            _board = new GameCell[rows][];


            //initialize board
            for(byte i = 0; i < rows; i++)
            {
                Board[i] = new GameCell[columns];

                for(byte j = 0; j < columns; j++)
                {
                    Board[i][j] = new GameCell(i, j, false, Get_AbreviationName_ForCell(i,j, Rows) );
                }
            }

        }
        public GameBoard( GameCell[][] grid)
        {
            //check grid validation
            if(grid == null || grid.Length == 0 || grid.Length > 16)
            {
                throw new ArgumentException("Problems with received grid. Problems in rows. It is null , equal zero or bigger than 16!");
            }
            if(grid[0] == null || grid[0].Length == 0 || grid[0].Length > 16)
            {
                throw new ArgumentException("Problems with received grid. Problems in columns. It is null, equal zero or bigger than 16!");
            }

            //apply grid sizes
            Rows = ((byte)grid.Length);
            Columns = ((byte)grid[0].Length);
            _board = grid;

            //fill every cell with abreviation like a1 or c8...
            for (byte i = 0; i < Rows; i++)
            {
                //check row validation
                if (grid[i] == null || grid[i].Length == 0)
                {
                    throw new ArgumentException($"Problems with grid row number: {i}. It is null or not matches with columns!");
                }

                for (byte j = 0; j < Columns; j++)
                {
                    //check every cell validation
                    if (_board[i][j] == null) throw new ArgumentException($"Grid is not filled with game cells enought! error at cell: ({i};{j})");

                    _board[i][j].Abreviation = Get_AbreviationName_ForCell(i, j, Rows);
                }
            }
        }
        #endregion




        //=================================================================================================================
        // MAKE/UNDO MOVE
        //=================================================================================================================
        #region MAKE/UNDO MOVE
        public void Make_Move(IMove move)
        {
            if (move == null) return;
            MoveNumber++;

            //помічаємо фігуру виконавця як зіграну фігуру (це особливо потрібно для короля, пішака чи тури)
            move.Executor_Piece.MarkAsPlayed(MoveNumber);
            move.Executed_OnMove = MoveNumber;

            for (byte i = 0; i < move.UpdatesCount; i++)
            {
                IMoveUpdate update = move.Updates[i]!;

                //у потрібну клітину вставляємо нову фігуру, чи  робимо її пустою
                Board[update.Cell.X][update.Cell.Y].Piece = update.New_Piece;

                //якщо є додаткова дія, то зробимо її
                update.AdditionAction?.DoAction(move, this);

            }
        }

        public void Undo_Move(IMove? move)
        {
            if (move == null) return;

            //починаємо повертати попередній стан з кінця
            for (int i = move.UpdatesCount - 1; i >= 0; i--)
            {
                IMoveUpdate update = move.Updates[i]!;

                //у потрібну клітину повертаємо стару фігуру, чи  робимо її пустою
                Board[update.Cell.X][update.Cell.Y].Piece = update.Old_Piece;

                //відмінюємо дію, яку зробили
                update.AdditionAction?.UndoAction(move, this);
            }


            //повертаємо початковий стан до фігури (якщо це можливо, то вона знову стане незіграною)
            //!!!!!!!!!!!!!!!порядок має значення, оскільки треба зробити усе задом аперед!!!!!!!!!!!!!!
            //==========================================================================================
            move.Executor_Piece.UnMarkAsPlayed(MoveNumber);
            MoveNumber--;
        }
        #endregion

        //=================================================================================================================
        // PROMOTION
        //=================================================================================================================
        #region PROMOTION
        public bool MakePromotion(ushort promoteTo_ID, out IPlayableChessPiece? promotedFromPiece)
        {
            if (CurrentCellThatNustBePromoted != null && CurrentCellThatNustBePromoted.Piece != null)
            {
                byte x = CurrentCellThatNustBePromoted.X;
                byte y = CurrentCellThatNustBePromoted.Y;
                promotedFromPiece = CurrentCellThatNustBePromoted.Piece;
                Board[x][y].Piece = Pieces_Factory.Create_ChessPiece_UsingID_ForColor(promoteTo_ID, promotedFromPiece.PieceColor);
                return true;
            }

            promotedFromPiece = null;
            return false;
        }
        public bool Check_OnPromotion(IMove move, out IMoveUpdate? updateWhenPromotionStarted)
        {
            updateWhenPromotionStarted = null;
            if (move == null || move.MainMoveType == MoveType.Promotion || move.MainMoveType == MoveType.CapturePromotion) return false;


            for (byte i = 0; i < move.UpdatesCount; i++)
            {
                IMoveUpdate update = move.Updates[i]!;

                //перевіряємо нову фігуру на клітині, що оновлюється.
                //Якщо нова фігура не пуста(це було переміщення на клітину, а не видалення)
                //цю фігуру можна продвинути, клітина та, яка перетворює фігури
                //а також у заготовленому списку фігур існують фігури з тим індексом, на яку можна перетворитись
                if (update.New_Piece != null &&
                    update.New_Piece.IsPromotable &&
                    update.Cell.IsPromotionCell &&
                    PromotionLists_Factory.Get_PromotionList_ByID(update.New_Piece.PromotionListID).Count() > 0)
                {
                    updateWhenPromotionStarted = update;  
                    return true;
                }
            }
            return false;
        }
        #endregion

        //=================================================================================================================
        // GET MATRIX
        //=================================================================================================================
        #region GET MATRIX
        public bool[][] Get_MatrixOfCellThatVisibleInFog_ForColor(Color color)
        {
            bool[][] visible = new bool[Rows][];
            for (byte i = 0; i < Rows; i++)
            {
                visible[i] = new bool[Columns];
            }


            foreach (ICell cell in Get_AllCellsWherePieces_OfColor(color))
            {
                visible[cell.X][cell.Y] = true;
            }
            foreach (Move move in Get_AllPossibleMoves_ForColor(color))
            {
                visible[move.To_Cell.X][move.To_Cell.Y] = true;
            }
            return visible;
        }
        public bool[,] Get_MatrixOfCellsThatUnderAtackByPieces_OfColor(Color color, ICell? victimCell = null)
        {

            bool[,] DangerCellsForColor = new bool[Rows, Columns];

            //пройти по кожній фігурі ( непустій клітині заданого кольору) 
            foreach (var cell in Get_AllCellsWherePieces_OfColor(color))
            {

                // отримати клітини, які під атакою поточної фігури, Кожну клітину зі списку помічаємо як небезечну
                foreach (var dangerCell in Get_AllCellsThatUnderAtack_ByCell(cell, victimCell))
                {
                    DangerCellsForColor[dangerCell.X, dangerCell.Y] = true;
                }
            }

            return DangerCellsForColor;
        }
        #endregion

        //=================================================================================================================
        // GET MOVES
        //=================================================================================================================
        #region GET MOVES




        public List<IMove> Get_AllPossibleMoves_WithDecoratorsApplied_ForCell(ICell cell)
        {
            if (cell == null || cell.Piece == null || CurrentCellThatNustBePromoted != null) return new List<IMove>();

            List<IMove> moves = cell.Piece.Get_AllPossibleMoves_ForCell(cell, this);

            foreach (var decorator in GetMoves_Decorators)
            {
                moves = decorator.Execute(moves, this, cell.Piece.PieceColor);
            }

            return moves;
        }







        /// <summary>
        /// ДЛЯ ВКАЗАНОЇ КЛІТИНИ ПОВЕРНУТИ ВСІ МОЖЛИВІ ХОДИ. ЯКЩО В КЛІТИНІ 
        /// НЕМАЄ ФІГУРИ, ЩО ЗМОЖЕ ЇХ ВИКОНАТИ, ТО ПОВЕРТАЄМО ПУСТИЙ СПИСОК
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public List<IMove> Get_AllPossibleMoves_ForCell(ICell cell)
        {
            if (cell == null || cell.Piece == null || CurrentCellThatNustBePromoted != null) return new List<IMove>();
            return cell.Piece.Get_AllPossibleMoves_ForCell(cell, this);
        }
        /// <summary>
        /// ДЛЯ ВКАЗАНОЇ КЛІТИНИ ПОВЕРНУТИ ХОДИ, ЯКІ МОЖЕ ВИКОНАТИ ФІГУРА, ЩО СТОЇТЬ НА НІЙ, 
        /// З ВИДАЛЕННЯМ ХОДІВ, ЩО ПОСТАВЛЯТЬ КОРОЛІВСЬКУ ФІГУРУ ПІД ЗАГРОЗУ
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public List<IMove> Get_AllPossibleMoves_WithKingsFiguresProtection_ForCell(ICell cell)
        {
            //if cell is not initialized or cell is empty or it is gameover or we doing promotion curently or it is selcted oponent piece
            if (cell == null || cell.Piece == null || IsPromotionNow) return new List<IMove>();

            //отримуємо спочатку ходи, які фігура банально може зробити (псевдо-легальні ходи)
            List<IMove> moves = cell.Piece.Get_AllPossibleMoves_ForCell(cell, this);

            //після отримання ходів слід видалити ходи, виконання яких поставитть під загрозу одного з королів (легальні ходи)
            return Delete_MovesThatWillNotProtectKing_ForColor(moves, cell.Piece.PieceColor);
        }

        public List<IMove> Get_AllPossibleMoves_WithKingsFiguresProtection_ForColor(Color color)
        {
            List<IMove> moves = new();
            IEnumerable<ICell> cells = Get_AllCellsWherePieces_OfColor(color);

            foreach (var cell in cells)
            {
                moves.AddRange(Get_AllPossibleMoves_WithKingsFiguresProtection_ForCell(cell));
            }

            return moves;
        }
        public List<IMove> Get_AllPossibleMoves_ForColor(Color color)
        {
            List<IMove> moves = new();

            foreach (var cell in Get_AllCellsWherePieces_OfColor(color))
            {
                moves.AddRange(Get_AllPossibleMoves_ForCell(cell));
            }
            return moves;
        }


        public List<IMove> Delete_MovesThatWillNotProtectKing_ForColor(List<IMove> moves, Color color)
        {
            //Проходимось по усім ходам з кінця (щоб можна було легко видалити хід зі списку)
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                //робимо хід
                Make_Move(moves[i]);

                //отримуємо список клітин, які перебувають під атакою після виконаного ходу
                bool[,] dangerouseCells = Get_MatrixOfCellsThatUnderAtackByPieces_OfColor(color.GetOpositeColor());

                //дивимось, чи хоча б 1 король залишився під атакою після виконаного ходу, якщо так, то переходимо до іншого можливого ходу
                bool isAtLeastOneKingUnderAttack = false;


                foreach (ICell kingCell in Get_AllCellsWherePiecesIsKingFigure_OfColor(color))
                {
                    if (dangerouseCells[kingCell.X, kingCell.Y])
                    {
                        isAtLeastOneKingUnderAttack = true;
                        break;
                    }
                }

                Undo_Move(moves[i]);

                //Якщо після виконання ходу хоча б 1 з королів залишиться під шахом, то такий хід не можна робити, видаляємо його
                if (isAtLeastOneKingUnderAttack)
                {
                    moves.RemoveAt(i);
                }
            }
            
            return moves;
        }


        public List<IMove> Get_AllPossibleMoves_WithPromotionAction_ForColor(Color color)
        {
            List<IMove> moves = new();

            //отримуємо список усіх ходів
            foreach(IMove move in Get_AllPossibleMoves_ForColor(color))
            {
                //якщо фігура повинна перетворитись, то отримуємо копії ходу з усіма перетвореннями
                if(Check_OnPromotion(move, out IMoveUpdate? updateWhenPromotionBegins))
                {
                    //ітеруємось по кожному id з списку перетворень, щоб перетворитись на фігуру з тим самим id
                    foreach(int id in PromotionLists_Factory.Get_PromotionList_ByID(updateWhenPromotionBegins!.New_Piece!.PromotionListID))
                    {
                        IMove copy = move.Clone();

                        copy.Add_Update(new MoveUpdate(updateWhenPromotionBegins.Cell, updateWhenPromotionBegins.New_Piece, Pieces_Factory.Create_ChessPiece_UsingID_ForColor(id, updateWhenPromotionBegins.New_Piece.PieceColor), MoveType.Promotion));

                        if (move.MainMoveType == MoveType.Capture) copy.MainMoveType = MoveType.CapturePromotion;
                        else copy.MainMoveType = MoveType.Promotion;

                        moves.Add(copy);
                    }

                }
                else moves.Add(move);   
            }

            return moves.OrderByDescending(x => x.MainMoveType).ToList();
        }


        public List<IMove> Get_AllPossibleMoves_WithPromotionAction_WithKingFiguresProtection_ForColor(Color color)
        {
            List<IMove> moves = new();

            //отримуємо список усіх ходів
            foreach (IMove move in Get_AllPossibleMoves_WithKingsFiguresProtection_ForColor(color))
            {
                //якщо фігура повинна перетворитись, то отримуємо копії ходу з усіма перетвореннями
                if (Check_OnPromotion(move, out IMoveUpdate? updateWhenPromotionBegins))
                {
                    //ітеруємось по кожному id з списку перетворень, щоб перетворитись на фігуру з тим самим id
                    foreach (int id in PromotionLists_Factory.Get_PromotionList_ByID(updateWhenPromotionBegins!.New_Piece!.PromotionListID))
                    {
                        IMove copy = move.Clone();

                        copy.Add_Update(new MoveUpdate(updateWhenPromotionBegins.Cell, updateWhenPromotionBegins.New_Piece, Pieces_Factory.Create_ChessPiece_UsingID_ForColor(id, updateWhenPromotionBegins.New_Piece.PieceColor), MoveType.Promotion));

                        if (move.MainMoveType == MoveType.Capture) copy.MainMoveType = MoveType.CapturePromotion;
                        else copy.MainMoveType = MoveType.Promotion;

                        moves.Add(copy);
                    }
                }
                else moves.Add(move);
            }
            return moves.OrderByDescending(x => x.MainMoveType).ToList();
        }





        #endregion









        //=================================================================================================================
        // GET CELLS
        //=================================================================================================================
        #region GET CELLS
        public IEnumerable<ICell> Get_AllCellsWherePieces_OfColor(Color color)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    //якщо колір збігається із заданим
                    if (Board[i][j].Piece?.PieceColor == color)
                    {
                        yield return Board[i][j];
                    }
                }
            }
        }
        public IEnumerable<ICell> Get_AllCellsWherePiecesIsKingFigure_OfColor(Color color)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if ((Board[r][c]?.Piece?.IsKingFigure ?? false) && Board[r][c]?.Piece?.PieceColor == color)
                    {
                        yield return Board[r][c];
                    }
                }
            }
        }
        public IEnumerable<ICell> Get_AllCellsWithKingFiguresThatInDanger_ForColor(Color color)
        {
            bool[,] dangerousCells = Get_MatrixOfCellsThatUnderAtackByPieces_OfColor(color.GetOpositeColor());

            foreach (ICell cell in Get_AllCellsWherePiecesIsKingFigure_OfColor(color))
            {
                int x = cell.X;
                int y = cell.Y;
                if (dangerousCells[x, y]) yield return cell;
            }
        }
        public IEnumerable<ICell> Get_AllCellsWithKingFiguresThatInDanger_ForBothPlayers()
        {
            List<ICell> list = new List<ICell>();

            list.AddRange(Get_AllCellsWithKingFiguresThatInDanger_ForColor(Color.White));
            list.AddRange(Get_AllCellsWithKingFiguresThatInDanger_ForColor(Color.Black));

            return list;
            
        }
        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_ByCell(ICell? cell, ICell? victim = null)
        {
            return cell?.Piece?.Get_AllCellThatUnderAtack_ByCell(cell, this, victim) ?? new List<ICell>();

        }


        public bool TryGet_Cell_AtPosition(out ICell? cell, int x, int y)
        {
            if (y < 0 || y >= Columns || x < 0 || x >= Rows)
            {
                cell = null;
                return false;
            }

            cell = GameGrid[x][y];
            return true;
        }
        public static string Get_AbreviationName_ForCell(int x, int y, int Rows)
        {
            return $"{IntegerToLetter[y]}{Rows - x}";
        }
        #endregion

        //=================================================================================================================
        // SCORE ESTIMATION
        //=================================================================================================================
        #region SCORE ESTIMATION
        public int Estimate_ScoreOfCurrentBoard_ForColor(Color color, bool estimateForAvailableMoves = false)
        {
            int score = 0;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Board[i][j].Piece != null && Board[i][j]?.Piece?.PieceColor == color) score += Board[i][j]?.Piece?.GetValuability() ?? 0;
                }
            }


            if (estimateForAvailableMoves)
            {
                List<IMove> allMoves = Get_AllPossibleMoves_WithKingsFiguresProtection_ForColor(color);
                //if (allMoves.Count == 0) return 0;
                score += 10 * allMoves.Count;
            }
            return score;
        }
        public int Estimate_ScoreForCurrentBoard(bool estimateForAvailableMoves = false)
        {
            int score = 0;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Board[i][j].Piece != null)
                    {
                        score += Board[i][j]?.Piece?.GetValuability() ?? 0;
                    }
                }
            }

            if (estimateForAvailableMoves)
            {
                List<IMove> allMoves_White = Get_AllPossibleMoves_WithKingsFiguresProtection_ForColor(Color.White);
                List<IMove> allMoves_Black = Get_AllPossibleMoves_WithKingsFiguresProtection_ForColor(Color.Black);
                score += allMoves_White.Count * 10;
                score -= allMoves_Black.Count * 10;
            }
            return score;
        }
        #endregion

        //=================================================================================================================
        // CLONE
        //=================================================================================================================
        #region CLONE
        public GameBoard Clone()
        {
            GameBoard board = new GameBoard(Rows, Columns);
            for (byte i = 0; i < Rows; i++)
            {
                for (byte j = 0; j < Columns; j++)
                {
                    board[i,j] = Board[i][j].Clone();

                }
            }
            board.IsWhiteTeamCastled= IsWhiteTeamCastled;
            board.IsBlackTeamCastled= IsBlackTeamCastled;
            board.CurrentCellThatNustBePromoted= CurrentCellThatNustBePromoted;
            board.MoveNumber= MoveNumber;

            foreach(var decorator in GetMoves_Decorators)
            {
                board.GetMoves_Decorators.Add(decorator.Clone());
            }

            return board;
        }
        #endregion

        //=================================================================================================================
        //ADD PIECES
        //=================================================================================================================
        #region ADD PIECES
        public void AddPieceToBoard(byte x, byte y, ushort piece_ID, Color color)
        {
            if (x < 0 || x >= Rows) throw new ArgumentOutOfRangeException("x is out of board range!");
            if (y < 0 || y >= Columns) throw new ArgumentOutOfRangeException("y is out of board range!");

            Board[x][y].Piece = Pieces_Factory.Create_ChessPiece_UsingID_ForColor(piece_ID, color);
        }
        #endregion

        //=================================================================================================================
        // ENCODING
        //=================================================================================================================
        #region ENCODING
        public string[] Get_BoardEncoding()
        {
            string[] boardEncoding = new string[Rows * Columns];

            for (int i = 0; i < Rows; i++)
            {

                for (int j = 0; j < Columns; j++)
                {
                    boardEncoding[i*Columns + j] = GameGrid[i][j].Get_Encoding();

                }
            }
            return boardEncoding;
        }
        #endregion

        //=================================================================================================================
        // CASTLE
        //=================================================================================================================
        #region CASTLE
        public void Mark_SideAsCastled_ForColor(Color color)
        {
            if (color == Color.White) IsWhiteTeamCastled = true;
            if (color == Color.Black) IsBlackTeamCastled = true;
        }
        public void Unmark_SideAsCastled_ForColor(Color color)
        {
            if (color == Color.White) IsWhiteTeamCastled = false;
            if (color == Color.Black) IsBlackTeamCastled = false;
        }
        #endregion

        //=================================================================================================================
        // DISPOSE
        //=================================================================================================================
        #region DISPOSE
        public void Dispose()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _board[i][j].Dispose();
                }
            }
        }
        #endregion

        //=================================================================================================================
        // INDEXATORS
        //=================================================================================================================
        #region INDEXATORS
        public GameCell this[byte row, byte col]
        {
            get { return Board[row][col]; }
            set { Board[row][col]= value; }
        }
        #endregion
    }
}
