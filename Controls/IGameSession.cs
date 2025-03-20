using Chess_game.Behaviour;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess_game.Controls
{
    public interface IGameSession
    {
        bool IsWhiteAI_Playing { get; }
        bool IsBlackAI_Playing { get; }


        public ObservableCollection<IPlayableChessPiece> BeatenPieces_White { get; }
        public ObservableCollection<IPlayableChessPiece> BeatenPieces_Black { get; }

        bool IsTimerOn { get; }
        int TimeLeftForBlack { get; }
        int TimeLeftForWhite { get; }

        Color CurrentPlayerMove { get; }
        Color MainPlayerColor { get; }
        string CurrentPlayerImageSource { get; }

        byte Columns { get; }
        byte Rows { get; }
        GameStartupSettings? CurrentGameBoardSettings { get; }

/*        Stack<IMove> UndoList { get; }
        Stack<IMove> RedoList { get; }*/

        LinkedList<uint> Redo_BoardHashList { get; }
        LinkedList<uint> Undo_BoardHashList { get;}

        GameModeType CurrentGameMode { get; }
        bool IsGameOver { get; }
        ICell? CurrentCellThatNustBePromoted { get; }
        int MovesLeft { get; }
        bool IsMovesLimitOn { get; }

        int WhiteSide_Score { get; }
        int BlackSide_Score { get; }

        bool IsPromotionNow { get; }

        ulong AnalizedBoardsCount { get; }
        bool IsKingInDangerShouldBeShown { get; }













        Task Make_Move(Move move, CancellationToken token);
        Task Make_Promotion(ushort promoteTo_ID, CancellationToken token);
        //Task<List<IMove>> Get_AllPossibleMovesToPerform_ForCell(ICell cell);
        //Task Start_CurrentGame(CancellationToken token);
        //Task Stop_CurrentGame(CancellationToken token);
        void Undo();
        void Redo();

    }
}
