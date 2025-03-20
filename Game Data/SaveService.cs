using Chess_game.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Chess_game.Controls.GameSessions;
using System.Windows;
using Chess_game.Controls.Handlers;

namespace Chess_game.Game_Data
{
    public static class SaveService
    {

        private static Notification_Handler notification_handler = Notification_Handler.Get_Instance();

        public static void Save_GameSession(AbstractGameSession gameSession)
        {
            


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


            int i = 1;

            string fileName = System.IO.Path.Combine(path, $"Board_{i}.json");
            while (File.Exists(fileName))
            {
                i++;
                fileName = System.IO.Path.Combine(path, $"Board_{i}.json");
            }

            var json = JsonConvert.SerializeObject(gameSession, Formatting.Indented);
 
            File.WriteAllText(fileName, json);
        }



        public static void Save_GameSession(AbstractGameSession gameSession, string path)
        {
            if(gameSession.IsPromotionNow)
            {
                notification_handler.NotifyUser("Can not save while promotion!");
            }
            else if(gameSession.IsGameOver)
            {
                notification_handler.NotifyUser("Can not save after game over!");
            }
            else
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(gameSession, Formatting.Indented));

                notification_handler.NotifyUser("Saved succesfully!");
            }

        }
    }
}
