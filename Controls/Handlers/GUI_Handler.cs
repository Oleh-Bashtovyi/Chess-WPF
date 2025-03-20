using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Chess_game.Controls.GameSessions;

namespace Chess_game.Controls
{
    public class GUI_Handler
    {
        private static readonly GUI_Handler _instance = new GUI_Handler();


        #region GUI EVENTS
        public event Func<string, int, CancellationToken, Task>? OnDelayAppear;
        public event Action? OnPromotionMade_HidePromotionScreen;
        public event Action<Color>? OnPromotionBegin_ShowPromotionScreen;
        #endregion


        #region BOARD UPDATE EVENTS
        public event Action? OnMoveMade_ReDrawGrid;
        public event Action<IEnumerable<ICell>>? OnKingInDanger_ShowAtackedCells;
        public event Action<bool[][]>? OnMoveMadeShowAllVisisbleCells;
        #endregion


        private GUI_Handler() { }
        public static GUI_Handler Get_Instance() => _instance;




        //=================================================================================================================
        #region GRAPHIC INTERFACE


        public async Task Show_DelayScreen(AbstractGameSession sender, CancellationToken token, string message, int duration)
        {
            if (sender.IsTimerOn) sender.StopTimer();

            token.ThrowIfCancellationRequested();

            await (OnDelayAppear?.Invoke(message, duration, token) ?? Task.Delay(100));

            token.ThrowIfCancellationRequested();

            if (sender.IsTimerOn) sender.StartTimer();
        }

        public void Show_VisibleCells_IfFogOfWarOn_Or_ShowAllCells(AbstractGameSession sender)
        {

        }





        public void Show_AllVisibleCells_ForPlayer(AbstractGameSession gameSession, Color player)
        {
            OnMoveMadeShowAllVisisbleCells?.Invoke(gameSession.Board.Get_MatrixOfCellThatVisibleInFog_ForColor(player));
        }

        public void Redraw_GameGrid()
        {
            OnMoveMade_ReDrawGrid?.Invoke();
        }


        public void Show_CellsWhereKingsFiguresInDanger_ForBothPlayers(AbstractGameSession gameSessios)
        {
            OnKingInDanger_ShowAtackedCells?.Invoke(gameSessios.Board.Get_AllCellsWithKingFiguresThatInDanger_ForBothPlayers());
        }


        #endregion











        public void Show_PormotionScreen_ForColor(Color color) => OnPromotionBegin_ShowPromotionScreen?.Invoke(color);

        public void Hide_PromotionScreen() => OnPromotionMade_HidePromotionScreen?.Invoke();

    }
}
