/*
    Programmed by: Bea Bianca R. Ancheta
    Date: 11/18/23
    LEVEL 1:
    Program Description: This program is the first level of our game, where there will be a player 
    who will have to pick up power-ups in order to complete the level. Enemies will also spawn to try 
    and stop the player, and colliding with the enemies will be the way to defeat them. There will 
    also be a heart system, and once the player reaches 0 hearts, the game is over.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Media;

namespace Night_sLight
{
    internal class Level1 : Form
    {
        // Player-related variables
        private MeleePlayer player;
        private int playerHealth = 3;

        // Item-related variables
        private List<Item> items;
        private Random rand = new Random(); // used for random items
        private List<string> item_locations;
        private string[] itemNames = { "Red Sword", "Medium Armour", "Green Shoes", "Gold Lamp", "Red Potion", "Fast Sword", "Instruction Manual", "Giant Sword", "Warm Jacket", "Wizards Hat", "Red Bow and Arrow", "Red Spear", "Green Potion", "heavy armour", "Cursed Axe", "Gold Ring", "Purple Ring" }; // Define name of the each collected item
        private int spawnTimeLimit = 100; // spawn time limit of the item
        private int timeCounter = 0;

        // Enemy-related variables
        private List<PictureBox> enemies = new List<PictureBox>();
        private int enemySpeed = 5;

        private Label lblCollected;
        private Label lblPower;
        private Label lblHearts;
        private PictureBox[] heartPictureBoxes; // Add this variable to keep track of heart PictureBoxes

        // Level transition flag
        private bool level2FormShown = false;
        private bool gameOver = false;
        // Background music
        private SoundPlayer simpleSound;
        // Timer
        private Timer timer = new Timer();
        // For Dialogues
        private Panel dialoguePanel = new Panel();
        private Label dialogueLabel = new Label();
        private Label levelLabel = new Label();
        private Button continueButton = new Button();

        // Constructor
        public Level1()
        {
            Console.WriteLine(Program.GetCurrentTime() + " - Level 1 is loaded.");
            // Form properties
            this.Text = "Night's Light (Level 1)";
            this.Width = 1020;
            this.Height = 670;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = Image.FromFile("images/Lvl1Map.png");

            SetupDialoguePanel();
        }
        // Initialize Dialogues
        private void SetupDialoguePanel()
        {
            string dialogue1 = "\nNarrator: In a land far away, there existed a kingdom known as Celestia. Celestia was threatened by the demon " +
                "\nking Malgrim who came to invade the lands and bring forth eternal darkness, and the king offered up a hefty reward " +
                "\nto the person or people who would be able to defeat the demon king. This is where our hero, Yusha comes in. Our hero" +
                "\nis a very powerful and resilient warrior, but due to the loss of his family and his whole hometown because of an " +
                "\ninvasion by the demon king army, he was down in the dumps. This is his journey on how he will redeem himself, his " +
                "\nfamily, his hometown, and the whole kingdom by defeating the demon king, with some unexpected help along the way." +
                "\n\nKing: To all the brave souls who will try to attempt to defeat the demon king, go through the path of Netherway Lane," +
                "\nand infiltrate the Abyssal Fortress where the demon king resides. He has taken too many lives in our kingdom, and we " +
                "\nhave to make the sacrifices of the people who fought valiantly be worth it. Eliminate the one who is threatening the " +
                "\nsafety of our people, the safety of our kingdom, and the safety of the whole world, the demon king Malgrim! The hero " +
                "\nor heroes who defeat him will be rewarded with anything that I am able to grant as your king!" +
                "\n\nNarrator: Our hero, wanting to redeem himself, his family, and hometown, was part of the heroes who bravely volunteered " +
                "\nto fight the demon king. He knew of the shortest path to go to the Netherway Lane, so he quickly packed up his sword and " +
                "\nshield and went on his way to Netherway Lane to reach the Abyssal Fortress. When he arrived at the Netherway Lane, the " +
                "\ndemon king Malgrim's minions were guarding the whole path, so the hero had to take up his sword. Unexpectedly, " +
                "\nhe was gaining more and more power as the fight went on, as if something was guiding him to defeat the demon king." +
                "\n\n\nPlayer Movements: Up-Down-Left-Right Arrow Keys, defeat enemies by colliding with them." +
                "\n*Powerups will be in the form of random items spawning randomly around the map*";

            dialoguePanel.BackgroundImage = Image.FromFile("images/Dialogues BG (2).png");
            dialoguePanel.BackgroundImageLayout = ImageLayout.Stretch; // Adjust to fit the panel
            dialoguePanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            dialoguePanel.Location = new Point(0, 0);
            dialoguePanel.Visible = true;

            // To determine what level is the current panel
            levelLabel.Text = "Level 1: Collect 10 powerups while defeating the enemies.";
            levelLabel.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            levelLabel.ForeColor = Color.White;
            levelLabel.BackColor = Color.Transparent;
            levelLabel.Location = new Point(90, 30);
            levelLabel.AutoSize = true;

            // Create dialogue labels
            dialogueLabel.Text = dialogue1;
            dialogueLabel.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            dialogueLabel.ForeColor = Color.White;
            dialogueLabel.BackColor = Color.Transparent;
            dialogueLabel.Location = new Point(90, 90);
            dialogueLabel.AutoSize = true;

            // Add the label to the dialogue panel
            dialoguePanel.Controls.Add(levelLabel);
            dialoguePanel.Controls.Add(dialogueLabel);

            // Configure continueButton
            continueButton.Text = "Continue";
            continueButton.Font = new Font("Century Gothic", 13, FontStyle.Bold);
            continueButton.FlatStyle = FlatStyle.Flat;
            continueButton.FlatAppearance.BorderSize = 0;
            continueButton.BackColor = Color.FromArgb(50, 71, 73, 115);
            continueButton.ForeColor = Color.White;
            continueButton.Size = new Size(200, 50);
            continueButton.Location = new Point(90, (dialogueLabel.Location.Y + dialogueLabel.Height) + 20);
            continueButton.Click += ContinueButton_Click;

            // Add controls to the form
            this.Controls.Add(dialoguePanel);
            // Add continueButton to the dialoguePanel
            dialoguePanel.Controls.Add(continueButton);
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            // Remove the level label
            dialoguePanel.Controls.Remove(levelLabel);
            levelLabel.Dispose(); // Dispose the dialogueLabel object

            // Remove the dialogue label
            dialoguePanel.Controls.Remove(dialogueLabel);
            dialogueLabel.Dispose(); // Dispose the dialogueLabel object

            // Remove the continue button
            dialoguePanel.Controls.Remove(continueButton);
            continueButton.Dispose(); // Dispose the continueButton object

            // Hide and remove dialogue panel
            dialoguePanel.Visible = false;
            this.Controls.Remove(dialoguePanel);
            dialoguePanel.Dispose(); // Dispose the dialoguePanel object

            // Start the game by initializing the following
            InitializeForm();
            InitializeEvents();
            InitializeTimer();
            InitializeEnemies();
            this.DoubleBuffered = true; // to avoid double flashing of the screen
            gameOver = false; // Reset the game over flag

            // Set focus back to the game form
            this.Focus();
        }
        // Initialize the main form
        private void InitializeForm()
        {
            // Background music
            simpleSound = new SoundPlayer("audio/Lvl1Sound.wav"); // audio path for background music
            simpleSound.Play();
            // Player and item initialization
            player = new MeleePlayer();
            items = new List<Item>();

            item_locations = Directory.GetFiles("items", "*.png").ToList(); // item image location

            // UI Labels
            lblCollected = InitializeLabel("Collected : ", new Point(550, 600));
            lblPower = InitializeLabel("Power : ", new Point(400, 600));
            lblHearts = InitializeLabel("Hearts :", new Point(130, 600));
            InitializeHearts();
            this.Controls.Add(lblCollected);
            this.Controls.Add(lblPower);
            this.Controls.Add(lblHearts);
        }
        // Initialize a label with common properties
        private Label InitializeLabel(string text, Point location)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.AutoSize = true;
            label.Width = 110;
            label.Font = new Font("Arial", 16, FontStyle.Bold);
            label.ForeColor = Color.White;
            label.BackColor = Color.Transparent;
            return label;
        }
        // Initialize event handlers
        private void InitializeEvents()
        {
            this.Paint += new PaintEventHandler(FormPaintEvent);
            this.KeyDown += new KeyEventHandler(KeyDownMovement);
            this.KeyUp += new KeyEventHandler(KeyUpMovement);
        }
        // Initialize the game timer
        private void InitializeTimer()
        {
            //Timer timer = new Timer();
            timer.Tick += new EventHandler(TimerEvent);
            timer.Interval = 1000 / 30;
            timer.Start();
        }
        // Handle key down events
        private void KeyDownMovement(object sender, KeyEventArgs e)
        {
            player.HandleKeyDown(e.KeyCode);
        }
        // Handle key up events
        private void KeyUpMovement(object sender, KeyEventArgs e)
        {
            player.HandleKeyUp(e.KeyCode);
        }
        // Paint event handler for drawing player and items
        private void FormPaintEvent(object sender, PaintEventArgs e)
        {

            Graphics canvas = e.Graphics;
            player.Draw(canvas);

            foreach (Item item in items)
            {
                item.Draw(canvas);
            }
        }
        // Timer event handler for game updates
        private void TimerEvent(object sender, EventArgs e)
        {
            if (!gameOver)  // Check if the game is not over
            {
                player.Update();
                CheckItemCollisions();
                MoveEnemies();
                CheckEnemyCollisions();
                UpdatePlayerHealth();
                this.Invalidate();
            }
            else
            {
                // Stop the timer when the game is over
                ((Timer)sender).Stop();
            }
        }
        // Generate new items based on a timer
        private void MakeItems()
        {
            int i = rand.Next(0, item_locations.Count);
            Item newItem = new Item(Image.FromFile(item_locations[i]), itemNames[i]);
            timeCounter = spawnTimeLimit;
            items.Add(newItem);
        }
        // Check collisions with items
        private void CheckItemCollisions()
        {
            foreach (Item item in items.ToArray())
            {
                item.Update();

                if (item.CheckCollision(player))
                {
                    lblCollected.Text = "Collected : " + item.Name; // Used to display name of the item collected
                    items.Remove(item);

                    player.IncreasePower(2);
                    lblPower.Text = "Power : " + player.GetPower(); // Update the each power by 2
                }
                else if (item.IsExpired())
                {
                    items.Remove(item);
                }
            }

            if (timeCounter > 1)
            {
                timeCounter--;
            }
            else
            {
                MakeItems();
            }
        }
        // Move enemies to the left and handle collisions
        private void MoveEnemies()
        {
            foreach (PictureBox enemy in enemies)
            {
                enemy.Left -= enemySpeed; // Move enemies to the left

                if (enemy.Right <= 0)
                {
                    playerHealth--;
                    UpdatePlayerHealth();
                    enemy.Left = this.Width; // Reset the enemy position to the right side
                }
            }
        }
        // Initialize enemy PictureBoxes
        private void InitializeEnemies()
        {
            int enemySpacing = 100; // Spacing between enemies
            int enemyStartPositionX = this.Width - 150; // To set the starting X position of enemies
            int enemyStartPositionY = 250; // To set the starting Y position of enemies

            for (int i = 0; i < 3; i++)
            {
                PictureBox enemy = new PictureBox();
                enemy.BackColor = Color.Transparent;
                enemy.Size = new Size(70, 70);
                enemy.SizeMode = PictureBoxSizeMode.StretchImage; // Set the SizeMode to StretchImage
                enemy.Location = new Point(enemyStartPositionX, enemyStartPositionY + i * enemySpacing);
                enemy.Image = Image.FromFile("images/enemy.gif"); // Load the GIF image for the enemy

                enemies.Add(enemy);
                this.Controls.Add(enemy);
            }
        }
        // Check collisions with enemies and transition to the next level
        private void CheckEnemyCollisions()
        {
            Rectangle playerRect = new Rectangle(player.GetX(), player.GetY(), player.GetWidth(), player.GetHeight()); // Get the player's rectangle

            foreach (PictureBox enemy in enemies.ToList())
            {
                Rectangle enemyRect = new Rectangle(enemy.Left, enemy.Top, enemy.Width, enemy.Height); // Get the rectangle for the current enemy

                if (playerRect.IntersectsWith(enemyRect)) // Check if the player's rectangle intersects with the enemy's rectangle
                {
                    if (enemy.Left <= playerRect.Right && playerRect.Left <= enemy.Right) // Check if the player is hitting the enemy
                    {
                        enemy.Left = this.Width; // Reset the enemy position only when they reach the left edge
                    }
                }
            }
            if (!level2FormShown && player.GetPower() >= 10 && playerHealth > 1) // Check if all sets of enemies are defeated and the player meets the conditions
            {
                Console.WriteLine(Program.GetCurrentTime() + " - Level 1 completed.");
                timer.Stop();
                timer.Dispose();
                gameOver = true;
                StopMusic(); // Stop the music before proceeding to the next level
                this.Close();
                Level2 level2Form = new Level2(); // Proceed to the next level
                level2FormShown = true;
                level2Form.Show();
            }
        }
        // Initialize a heart picture boxes
        private void InitializeHearts()
        {
            heartPictureBoxes = new PictureBox[playerHealth]; // Create an array to store heart PictureBoxes

            // Add PictureBox controls for hearts
            for (int i = 0; i < playerHealth; i++)
            {
                heartPictureBoxes[i] = new PictureBox();
                heartPictureBoxes[i].Image = Image.FromFile("images/heart.png");
                heartPictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                heartPictureBoxes[i].BackColor = Color.Transparent;
                heartPictureBoxes[i].Size = new Size(30, 30);
                heartPictureBoxes[i].Name = "heartPictureBox" + i.ToString(); // Set the Name property
                heartPictureBoxes[i].Location = new Point(230 + i * 40, 595);
                this.Controls.Add(heartPictureBoxes[i]);
            }
        }
        private void UpdatePlayerHealth()
        {
            // Ensure player health is within valid bounds
            playerHealth = Math.Max(0, playerHealth);

            // Update player health in heart display
            for (int i = 0; i < heartPictureBoxes.Length; i++)
            {
                // Set the visibility of each heart PictureBox based on player health
                heartPictureBoxes[i].Visible = i < playerHealth;
            }

            if (playerHealth <= 0 && !gameOver) // Add a check to open the game menu only once
            {
                // Set the game over flag
                timer.Stop();
                timer.Dispose();
                gameOver = true;
                StopMusic(); // Stop the music before showing the game over message
                DialogResult result = MessageBox.Show("Game Over", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    Console.WriteLine(Program.GetCurrentTime() + " - Level 1 Game Over.");
                    // Navigate back to the game menu
                    GameOver gameOver = new GameOver();
                    gameOver.Show();
                    this.Close();
                }
            }
        }
        // Stop background music
        private void StopMusic()
        {
            simpleSound.Stop();
        }
        // Player class
        class MeleePlayer
        {
            // Player properties
            private Image playerImage;
            private List<string> playerMovements;
            private int steps;
            private int slowDownFrameRate;
            private int playerX;
            private int playerY;
            private int playerHeight;
            private int playerWidth;
            private int playerSpeed;
            private int playerAttackPower;
            // Directional flags
            private bool left, right, up, down;
            // Constructor
            public MeleePlayer()
            {
                playerMovements = Directory.GetFiles("player", "*.png").ToList();
                playerImage = Image.FromFile(playerMovements[0]);

                steps = 0;
                slowDownFrameRate = 0; // To slow down the player movements
                playerX = 380;
                playerY = 160;
                playerHeight = 70;
                playerWidth = 70;
                playerSpeed = 15;
            }
            // Handle key down events
            public void HandleKeyDown(Keys keyCode)
            {
                if (keyCode == Keys.Left)
                    left = true;
                if (keyCode == Keys.Right)
                    right = true;
                if (keyCode == Keys.Up)
                    up = true;
                if (keyCode == Keys.Down)
                    down = true;
            }
            // Handle key up events
            public void HandleKeyUp(Keys keyCode)
            {
                if (keyCode == Keys.Left)
                    left = false;
                if (keyCode == Keys.Right)
                    right = false;
                if (keyCode == Keys.Up)
                    up = false;
                if (keyCode == Keys.Down)
                    down = false;
            }
            // Draw the player on the canvas
            public void Draw(Graphics canvas)
            {
                canvas.DrawImage(playerImage, playerX, playerY, playerWidth, playerHeight);
            }
            // Update player position and animation
            public void Update()
            {
                if (left && playerX > 0)
                {
                    playerX -= playerSpeed;
                    AnimationPlayer(4, 7); // image range index from the file player character
                }
                else if (right && playerX + playerWidth < 1020)
                {
                    playerX += playerSpeed;
                    AnimationPlayer(8, 11); // image range index from the file player character
                }
                else if (up && playerY > 0)
                {
                    playerY -= playerSpeed;
                    AnimationPlayer(12, 15); // image range index from the file player character
                }
                else if (down && playerY + playerHeight < 670)
                {
                    playerY += playerSpeed;
                    AnimationPlayer(0, 3); // image range index from the file player character
                }
                else
                {
                    AnimationPlayer(0, 0);
                }
            }
            // Handle player animation
            private void AnimationPlayer(int start, int end)
            {
                slowDownFrameRate += 1;

                if (slowDownFrameRate == 4)
                {
                    steps++;
                    slowDownFrameRate = 0;
                }
                if (steps > end || steps < start)
                {
                    steps = start;
                }

                playerImage = Image.FromFile(playerMovements[steps]);
            }
            // Getters for player position and dimensions
            public int GetX()
            {
                return playerX;
            }

            public int GetY()
            {
                return playerY;
            }

            public int GetWidth()
            {
                return playerWidth;
            }

            public int GetHeight()
            {
                return playerHeight;
            }

            public void IncreasePower(int amount)
            {
                playerAttackPower += amount; // Implement power increase logic
            }

            public int GetPower()
            {
                return playerAttackPower; // Implement power retrieval logic
            }
        }
        // Item class
        class Item
        {
            // Item properties
            private int positionX;
            private int positionY;
            private Image itemImage;
            private int height;
            private int width;
            private string name;
            private int lifeTime;
            // Constructor
            public Item(Image image, string itemName)
            {
                itemImage = image;
                name = itemName;

                Random range = new Random();
                positionX = range.Next(20, 400);
                positionY = range.Next(20, 400);
                height = 50;
                width = 50;
                lifeTime = 200;
            }
            // Getter for item name
            public string Name
            {
                get { return name; }
            }
            // Check collision with the player
            public bool CheckCollision(MeleePlayer player)
            {
                int object1X = player.GetX();
                int object1Y = player.GetY();
                int object1Width = player.GetWidth();
                int object1Height = player.GetHeight();

                int object2X = positionX;
                int object2Y = positionY;
                int object2Width = width;
                int object2Height = height;

                return (object1X + object1Width >= object2X && object1X <= object2X + object2Width
                    && object1Y + object1Height >= object2Y && object1Y <= object2Y + object2Height);
            }
            // Draw the item on the canvas
            public void Draw(Graphics canvas)
            {
                canvas.DrawImage(itemImage, positionX, positionY, width, height);
            }
            // Update item's lifetime
            public void Update()
            {
                lifeTime--;
            }
            // Check if the item's lifetime is expired
            public bool IsExpired()
            {
                return lifeTime < 1;
            }
        }
    }
}
