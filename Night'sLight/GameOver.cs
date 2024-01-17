using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Night_sLight
{
    internal class GameOver : Form
    {
        Image bgImage = new Bitmap("images/Game Over BG (1).png");
        Button btPlay = new Button();

        public GameOver()
        {
            // Set the size of the form
            this.Text = "Game Over!";
            this.Width = 1000;
            this.Height = 600;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = bgImage;
            this.MaximizeBox = false;

            // Create label for "Game Over"
            Label gameOverLabel = new Label();
            gameOverLabel.Text = "Game Over";
            gameOverLabel.Font = new Font("Century Gothic", 45, FontStyle.Bold);
            gameOverLabel.BackColor = Color.Transparent;
            gameOverLabel.AutoSize = true;
            gameOverLabel.ForeColor = Color.White; // Optional: Set label text color
            gameOverLabel.Location = new Point(((this.Width - gameOverLabel.Width) / 2) - 130, ((this.Height - gameOverLabel.Height) / 2) - 100);
            this.Controls.Add(gameOverLabel);

            // Create label for "Hero Failed" message
            Label heroFailedLabel = new Label();
            heroFailedLabel.Text = "The hero has failed...\nTry again to succeed!";
            heroFailedLabel.Font = new Font("Century Gothic", 13, FontStyle.Regular);
            heroFailedLabel.BackColor = Color.Transparent;
            heroFailedLabel.AutoSize = true;
            heroFailedLabel.ForeColor = Color.White; // Optional: Set label text color
            heroFailedLabel.Location = new Point(((this.Width - heroFailedLabel.Width) / 2) - 40, gameOverLabel.Bottom + 20);
            this.Controls.Add(heroFailedLabel);

            // Create and configure the "Play Again" button
            btPlay.Text = "Play Again!";
            btPlay.Font = new Font("Century Gothic", 17, FontStyle.Bold);
            btPlay.FlatStyle = FlatStyle.Flat;
            btPlay.BackColor = Color.SlateGray;
            btPlay.ForeColor = Color.White;
            btPlay.FlatAppearance.BorderSize = 0;
            btPlay.Width = 200;
            btPlay.Height = 50;
            btPlay.Location = new Point((this.Width - btPlay.Width) / 2, heroFailedLabel.Bottom + 40);
            btPlay.Click += BtPlay_Click;
            this.Controls.Add(btPlay);
        }

        // Button click event handler to switch to the GameMenu form
        private void BtPlay_Click(object sender, EventArgs e)
        {
            this.Hide();
            GameMenu gameMenuForm = new GameMenu();
            gameMenuForm.Show();
        }
    }
}
