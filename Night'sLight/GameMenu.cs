/*
    Programmed by the following: 
        Adriel E. Groyon
        Bea Bianca R. Ancheta
        Carl Daniel O. De Guzman
        Jasper John M. Raguin
    Date: 11/18/23
    Program Description: This C# program defines a graphical game menu using Windows Forms, 
    presenting buttons for playing the game, viewing leaderboards, and exiting. It handles button 
    clicks to transition between menu options, read and display leaderboard information from a file, 
    and logs the application's start and termination times. Additionally, it sets up the UI elements,
    styles the buttons, and loads background images to create an immersive interface for the game.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Night_sLight
{
    internal class GameMenu : Form
    {
        Button btPlay = new Button();
        Button btLeaderboards = new Button();
        Button btExit = new Button();
        Label titleLabel = new Label();
        Label subtitleLabel = new Label();
        Image bgImage = new Bitmap("images/Game Project BG (2).png");

        // constructor
        public GameMenu()
        {
            // Used for game logs.
            OnApplicationStart();

            // Set the size of the form
            this.Text = "Night's Light";
            this.Width = 1000;
            this.Height = 600;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = bgImage;
            this.MaximizeBox = false;

            // Set the background color to a fantasy-style background color
            this.BackColor = System.Drawing.Color.FromArgb(33, 32, 59);

            btPlay.Text = "Play";
            btPlay.Font = new System.Drawing.Font("Arial", 13, System.Drawing.FontStyle.Italic);
            btPlay.ForeColor = System.Drawing.Color.White;
            btPlay.FlatStyle = FlatStyle.Flat; // Set the button style to flat
            btPlay.BackColor = Color.Transparent; // Set the background color to transparent
            btPlay.FlatAppearance.BorderSize = 0; // Remove the button border
            btPlay.Width = 200;
            btPlay.Height = 50;
            btPlay.Left = (500 - btPlay.Width) / 2;
            btPlay.Top = subtitleLabel.Bottom + 175;
            this.Controls.Add(btPlay);

            btLeaderboards.Text = "Leaderboards";
            btLeaderboards.Font = new System.Drawing.Font("Arial", 13, System.Drawing.FontStyle.Italic);
            btLeaderboards.ForeColor = System.Drawing.Color.White;
            btLeaderboards.FlatStyle = FlatStyle.Flat;
            btLeaderboards.BackColor = Color.Transparent;
            btLeaderboards.FlatAppearance.BorderSize = 0;
            btLeaderboards.Width = 200;
            btLeaderboards.Height = 50;
            btLeaderboards.Left = (500 - btLeaderboards.Width) / 2;
            btLeaderboards.Top = btPlay.Bottom + 20;
            this.Controls.Add(btLeaderboards);

            btExit.Text = "Exit";
            btExit.Font = new System.Drawing.Font("Arial", 13, System.Drawing.FontStyle.Italic);
            btExit.ForeColor = System.Drawing.Color.White;
            btExit.FlatStyle = FlatStyle.Flat;
            btExit.BackColor = Color.Transparent;
            btExit.FlatAppearance.BorderSize = 0;
            btExit.Width = 200;
            btExit.Height = 50;
            btExit.Left = (500 - btExit.Width) / 2;
            btExit.Top = btLeaderboards.Bottom + 20;
            this.Controls.Add(btExit);

            btPlay.Click += new EventHandler(btPlay_Click);
            btLeaderboards.Click += new EventHandler(btLeaderboards_Click);
            btExit.Click += new EventHandler(btExit_Click);
        }

        // events
        private void OnApplicationStart()
        {
            Console.WriteLine(Program.GetCurrentTime() + " - Game application is started.");
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            Console.WriteLine(Program.GetCurrentTime() + " - Game application is terminated.");
        }

        public void btPlay_Click(object sender, EventArgs e)
        {
            // When the "Play" button is clicked, create and show the Level1 form
            Level1 level1Form = new Level1();
            level1Form.Show();

            // Optionally, you can hide the GameMenu form when transitioning to the Level1 form
            this.Hide();
        }

        public void btLeaderboards_Click(object sender, EventArgs e)
        {
            try
            {
                // Open the Leaderboard.txt file using FileStream
                using (FileStream leaderboardFileStream = new FileStream("leaderboard/Leaderboard.txt", FileMode.Open))
                {
                    string leaderboardInfo = GetLeaderboardInfo(leaderboardFileStream);
                    leaderboardFileStream.Close();

                    // Check if the file is empty
                    if (string.IsNullOrWhiteSpace(leaderboardInfo))
                    {
                        MessageBox.Show("Leaderboard.txt is empty.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display information in MessageBox
                        MessageBox.Show(leaderboardInfo, "Leaderboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("LeaderBoard.txt file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void btExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Method to get Leaderboard information from the Leaderboard.txt file
        private static string GetLeaderboardInfo(FileStream fileName)
        {
            StreamReader leaderboardDataReader = new StreamReader(fileName);
            string leaderboardDataLine = leaderboardDataReader.ReadLine();
            List<(string username, TimeSpan time)> players = new List<(string, TimeSpan)>();

            // Read and process each line of leaderboard data from the file.
            while (leaderboardDataLine != null)
            {
                string[] playerInfoPerLine = leaderboardDataLine.Split('|');

                // Parse the time into TimeSpan
                if (TimeSpan.TryParseExact(playerInfoPerLine[1], "m\\:ss", null, out TimeSpan time))
                {
                    players.Add((playerInfoPerLine[0], time));
                }

                leaderboardDataLine = leaderboardDataReader.ReadLine();
            }

            // Sort players based on Level 4 Time
            players = players.OrderBy(p => p.time).ToList();

            // Generate the leaderboard string for the top 10 players
            string leaderboardInfo = "Top 10 Players\n";
            int count = Math.Min(players.Count, 10); // Consider only top 10, or actual count if less than 10

            for (int i = 0; i < count; i++)
            {
                leaderboardInfo += $"Top{i + 1}: \t Username: {players[i].username} \t Level 4 Time: {players[i].time:mm\\:ss}\n";
            }

            leaderboardDataReader.Close();
            return leaderboardInfo;
        }
    }
}
