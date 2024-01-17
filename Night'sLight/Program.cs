/*
    Programmed by the following: 
        Adriel E. Groyon
        Bea Bianca R. Ancheta
        Carl Daniel O. De Guzman
        Jasper John M. Raguin
    Date: 11/18/23
    Program Description: This C# program initializes a game menu using multi-threading to run in the 
    background, capturing closing events and logging game data. It also provides a method to retrieve 
    the current time in a specific format.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Night_sLight
{
    internal class Program
    {
        private static Thread thread;
        private static GameMenu gameMenu;
        public static string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        static void Main(string[] args)
        {
            thread = new Thread(() =>
            {
                gameMenu = new GameMenu();

                // Used for catching the closing event game log.
                Application.ApplicationExit += new EventHandler(gameMenu.OnApplicationExit);
                Application.Run(gameMenu);
            });
            thread.IsBackground = false;
            thread.Start();

            Console.WriteLine("GAME LOGS");
        }

        public static GameMenu getGameMenuForm()
        {
            return gameMenu;
        }

        public static string GetCurrentTime()
        {
            DateTime time = DateTime.Now;
            string currTime = time.ToString("MM/dd/yyyy HH:mm:ss");
            return currTime;
        }
    }
}
