using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Controls.Handlers
{
    public class Notification_Handler
    {
        public event Action<string>? On_NotificationRaised;
        public event Action? On_GameOver_NotificationRaised;



        private static readonly Notification_Handler _instance = new ();
        private Notification_Handler() { }

        public static Notification_Handler Get_Instance() => _instance;


        public void NotifyUser(string message) => On_NotificationRaised?.Invoke(message);

        public void NotifyUser_AboutGameOver() => On_GameOver_NotificationRaised?.Invoke();
    }
}
