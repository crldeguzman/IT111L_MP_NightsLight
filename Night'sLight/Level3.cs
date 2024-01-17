/*
    Programmed by: Adriel E. Groyon
    Date: 11/18/23
    LEVEL 3:
    Program Description: Level 3 of 'Night's Light' thrusts the hero deeper into the Abyssal Fortress, 
    where they encounter newfound powers bestowed by an enchanted amulet. Use spacebar to shoot and 
    face the relentless demon king's army, the hero must harness these abilities to combat and defaet all 
    the looming threat and press on towards victory.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Night_sLight
{
    // Abstract class representing game objects
    public abstract class GameObject : PictureBox
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Image Image { get; set; }

        public abstract void Move();
    }

    // Player class inheriting from GameObject
    public class ShooterPlayer : GameObject
    {
        private string[] playerImages = { "images/player/character_13.png", "images/player/character_14.png", "images/player/character_15.png", "images/player/character_16.png" };
        private int currentImageIndex = 0;
        private bool canShoot = true;
        private bool left, right;
        private int playerX;
        private int playerWidth;

        public ShooterPlayer()
        {
            Health = 3;
            Speed = 20; // default is 10

            playerX = 500;
            playerWidth = 64;

            // Initialize player properties
            BackColor = Color.Transparent;
            Image = new Bitmap(playerImages[currentImageIndex]);
            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(playerWidth, 64);
            // Set initial player location (adjust as needed)
            Location = new Point(playerX, 500);
        }

        public override void Move()
        {
            int formWidth = Parent.ClientSize.Width; // Get the width of the form

            if (left && playerX > 0)
            {
                playerX -= Speed;
                UpdatePlayerImage();
            }
            else if (right && playerX + playerWidth < formWidth)
            {
                playerX += Speed;
                UpdatePlayerImage();
            }

            // Update the actual location of the player
            Location = new Point(playerX, Location.Y);
        }

        public void HandleKeyDown(Keys keyCode)
        {
            if (keyCode == Keys.Left)
                left = true;
            if (keyCode == Keys.Right)
                right = true;
        }
        public void HandleKeyUp(Keys keyCode)
        {
            if (keyCode == Keys.Left)
                left = false;
            if (keyCode == Keys.Right)
                right = false;
        }

        public void Shoot(Keys keyCode, List<Projectiles> projectiles, DateTime lastShootTime, TimeSpan shootInterval)
        {
            // Check if the specified key is the spacebar and the player can shoot
            if (keyCode == Keys.Space && canShoot && DateTime.Now - lastShootTime >= shootInterval)
            {
                canShoot = false; // Disable shooting temporarily

                // Create and add a new projectile
                Projectiles projectile = new Projectiles();
                projectile.Location = new Point(Left + Width / 2 - projectile.Width / 2, Top);
                projectiles.Add(projectile);
                Parent.Controls.Add(projectile);

                // Update the last shoot time
                lastShootTime = DateTime.Now;

                // Use a Timer to re-enable shooting after the specified interval
                Timer shootCooldownTimer = new Timer();
                shootCooldownTimer.Interval = (int)shootInterval.TotalMilliseconds;
                shootCooldownTimer.Tick += (sender, args) =>
                {
                    canShoot = true; // Enable shooting again
                    shootCooldownTimer.Stop();
                    shootCooldownTimer.Dispose();
                };
                shootCooldownTimer.Start();
            }
        }

        public void DrawHearts(Graphics graphics)
        {
            Image heartImage = new Bitmap("images/heart.png");
            int heartSpacing = 35; // Adjust spacing between hearts as needed
            int initialX = 15; // Initial X position of hearts
            int heartY = 40; // Y position of hearts

            // Draw hearts based on player's health
            for (int i = 0; i < Health; i++)
            {
                int heartX = initialX + i * heartSpacing;

                // Draw heart image at the calculated position
                graphics.DrawImage(heartImage, new Rectangle(heartX, heartY, 30, 30)); // Adjust size as needed
            }
        }

        public void UpdatePlayerImage()
        {
            // Update player image based on the current index in the array
            Image = new Bitmap(playerImages[currentImageIndex]);

            // Move to the next image in the array
            currentImageIndex = (currentImageIndex + 1) % playerImages.Length;
        }
    }

    // Enemy class inheriting from GameObject
    public class Enemy : GameObject
    {
        private string[] enemyImages = { "images/enemies/enemy_1.gif", "images/enemies/enemy_2.gif", "images/enemies/enemy_3.gif", "images/enemies/enemy_4.gif", "images/enemies/enemy_5.gif", "images/enemies/enemy_6.gif" };
        private Random rand = new Random();
        private Label healthLabel = new Label();

        internal Image[] enemyAnimationFrames; // Array to hold the frames of the enemy's animated GIF // Make enemyAnimationFrames accessible by other classes in the same assembly
        private int currentFrameIndex = 0;
        private Size imageSize = new Size(64, 64); // Desired size for the enemy frames

        private int hp;
        public int Health
        {
            get { return hp; }
            set
            {
                hp = value;
                UpdateHealthLabel(); // Update the health label when health changes
            }
        }

        public Enemy()
        {
            // Default Health and Speed
            Health = 1;
            Speed = 5;
            // Initialize enemy properties
            BackColor = Color.Transparent;
            // Random Image Path
            string stringImage = enemyImages[rand.Next(enemyImages.Length)];

            // Load animated GIF frames into an array
            enemyAnimationFrames = LoadAndResizeGifFrames(stringImage, imageSize);
            Image = enemyAnimationFrames[0]; // Set the initial frame as the Image property
            Size = imageSize;

            // Initialize health label
            healthLabel.ForeColor = Color.DarkRed;
            healthLabel.Width = 70;
            healthLabel.Font = new Font("Century Gothic", 13, FontStyle.Bold);
            healthLabel.Text = $"HP: {Health}"; // Display initial health
            healthLabel.Location = new Point(10, 0);
            this.Controls.Add(healthLabel); // don't implement yet because it causes frame rate issues
        }

        public override void Move()
        {
            Top += Speed; // Move the enemy downward
            AnimateEnemy(); // Call the method to animate the enemy
        }

        private void AnimateEnemy()
        {
            // Update the current frame index to display the next frame
            currentFrameIndex = (currentFrameIndex + 1) % enemyAnimationFrames.Length;
            Image = enemyAnimationFrames[currentFrameIndex]; // Set the current frame as the Image property
        }

        // Helper method to load GIF frames into an array and resize them
        private Image[] LoadAndResizeGifFrames(string path, Size newSize)
        {
            Image gifImage = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);

            Image[] frames = new Image[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);
                Image currentFrame = (Image)gifImage.Clone();

                // Resize the frame to fit the specified size while maintaining the aspect ratio
                frames[i] = ResizeImage(currentFrame, newSize);
                currentFrame.Dispose(); // Dispose the temporary frame
            }

            gifImage.Dispose(); // Dispose the original GIF image
            return frames;
        }

        // Helper method to resize an image while maintaining aspect ratio
        private Image ResizeImage(Image image, Size newSize)
        {
            float ratio = Math.Min((float)newSize.Width / image.Width, (float)newSize.Height / image.Height);
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        // Method to set animation frames
        public void SetAnimationFrames(string path, Size newSize)
        {
            enemyAnimationFrames = LoadAndResizeGifFrames(path, newSize);
            Image = enemyAnimationFrames[0];
            Size = newSize;
        }

        //  Display updated health
        private void UpdateHealthLabel()
        {
            healthLabel.Text = $"HP: {Health}";
        }

        // Method to position the health label above the Enemy
        public void SetHealthLabelPosition(Point enemyLocation)
        {
            healthLabel.Location = new Point(enemyLocation.X, enemyLocation.Y - 30);
        }
    }

    // Heart class inheriting from PictureBox; For the Player
    public class Heart : PictureBox
    {
        public Heart()
        {
            Image = new Bitmap("images/heart.png");
            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(30, 30); // Adjust size as needed
            BackColor = Color.Transparent;
        }
    }

    // Projectile class inheriting from PictureBox
    public class Projectiles : PictureBox
    {
        public int Speed { get; set; }

        public Projectiles()
        {
            Speed = 30; // Adjust the speed as needed
            // Initialize projectile properties
            BackColor = Color.Transparent;
            Image = new Bitmap("images/projectile.png");
            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(20, 20);
        }

        public void Move()
        {
            Top -= Speed;
        }
    }
    internal class Level3 : Form
    {
        private Image bgImage = new Bitmap("images/Game Project Lvl2 BG (4).png");
        private ShooterPlayer player = new ShooterPlayer();
        private List<Enemy> enemies = new List<Enemy>();
        private List<Projectiles> projectiles = new List<Projectiles>();
        private Timer gameTimer = new Timer();
        private Label heartLabel = new Label();
        private bool gameRunning = true;

        // For projectile
        private DateTime lastShootTime = DateTime.MinValue;
        private TimeSpan shootInterval = TimeSpan.FromSeconds(0.2); // Set the interval between shots

        private int currentWave = 1; // Initialize the wave counter

        // For Dialogues
        private Panel dialoguePanel = new Panel();
        private Label dialogueLabel = new Label();
        private Label levelLabel = new Label();
        private Button continueButton = new Button();

        public Level3()
        {
            Console.WriteLine(Program.GetCurrentTime() + " - Level 3 is loaded.");

            // Form initialization 
            this.Text = "Night's Light (Level 3)";
            this.Width = 1000;
            this.Height = 600;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = bgImage;
            this.MaximizeBox = false;
            this.FormClosing += Level3_FormClosing;
            // For smoother movements
            this.DoubleBuffered = true;

            SetupDialoguePanel();

            // Add player to the form
            this.Controls.Add(player);
            this.KeyDown += new KeyEventHandler(KeyDownMovement);
            this.KeyUp += new KeyEventHandler(KeyUpMovement);

            // Initialize the heart label
            heartLabel.Text = "Player Hearts: ";
            heartLabel.Width = 160;
            heartLabel.Font = new Font("Arial", 15, FontStyle.Bold);
            heartLabel.BackColor = Color.Transparent;
            heartLabel.ForeColor = Color.Red;
            heartLabel.Location = new Point(10, 10);
            this.Controls.Add(heartLabel);

            // Set up timer and other game logic
            gameTimer.Interval = 20;
            gameTimer.Tick += new EventHandler(UpdateGame);
        }

        // Rest of Level3 class implementation (event handlers, methods, etc.)
        private void Level3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the game
            gameRunning = false;

            Console.WriteLine(Program.GetCurrentTime() + " - Level 3 Closed.");

            // Hide the form
            this.Hide();
        }

        private void SetupDialoguePanel()
        {
            string dialogue1 = "Narrator: After the hero went through the first 2 floors of the Abyssal Fortress, " +
                "\nhe encountered the amulet that the mysterious voice was describing. It was a bright and " +
                "\nbeautiful amulet that shined brightly, and once the hero held the amulet, he learned a holy " +
                "\nspell that could shoot out holy bullets of light." +
                "\n\n\nDemon King Malgrim: Who dares to trespass my fortress?! My army, go and bring the " +
                "\ntresspasser's head to me!" +
                "\n\n\nNarrator: The hero finds himself in another crisis, with the demon king army closing " +
                "\nin on him. To defeat the demon king, he decides to fight the demon king's army head on, " +
                "\nwith the help of the amulet, and his newfound powers." +
                "\n\n\nPlayer Movements: Left-Right Arrow Keys, Spacebar to shoot.\r\n";

            dialoguePanel.BackgroundImage = Image.FromFile("images/Dialogues BG (4).png");
            dialoguePanel.BackgroundImageLayout = ImageLayout.Stretch; // Adjust to fit the panel
            dialoguePanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            dialoguePanel.Location = new Point(0, 0);
            dialoguePanel.Visible = true;

            // To determine what level is the current panel
            levelLabel.Text = "Level 3: Defeat all enemies while steering clear of any contact.";
            levelLabel.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            levelLabel.ForeColor = Color.White;
            levelLabel.BackColor = Color.Transparent;
            levelLabel.Location = new Point(110, 30);
            levelLabel.AutoSize = true;

            // Create dialogue labels
            dialogueLabel.Text = dialogue1;
            dialogueLabel.Font = new Font("Century Gothic", 11, FontStyle.Regular);
            dialogueLabel.ForeColor = Color.White;
            dialogueLabel.BackColor = Color.Transparent;
            dialogueLabel.Location = new Point(110, 90);
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
            continueButton.Location = new Point(110, (dialogueLabel.Location.Y + dialogueLabel.Height) + 35);
            continueButton.Click += ContinueButton_Click;

            // Add controls to the form
            this.Controls.Add(dialoguePanel);
            // Add continueButton to the dialoguePanel
            dialoguePanel.Controls.Add(continueButton);
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            // Show the random power-up dialogue
            ShowPowerUpDialogue();

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

            // Start the game by initializing enemies and starting the timer
            InitializeEnemies(2, 1, 3, 5); // numberOfEnemies, minHealth, maxHealth, speed
            gameTimer.Start();

            // Set focus back to the game form
            this.Focus();
        }
        private void ShowPowerUpDialogue()
        {
            // Array of random dialogues for power-ups with choices
            string[] powerUpDialogues = {
                "You received the amulet. Do you wish to take care of it?",
                "A sudden gust of wind surrounds you. Do you embrace it?",
                "You feel a surge of energy within. Do you accept its power?",
                "A mysterious light envelops you. Do you step into it?",
                "You hear an ancient chant echoing. Do you listen attentively?",
                "The air crackles with magic. Do you reach out to it?",
                "A radiant warmth of the moonlit fills the forest. Do you bask in its energy?",
                "A mysterious voice speaks to you. Do you heed its call?",
                "Your inner self calls out. Do you heed its guidance?",
                "A moment of reflection arrives. Do you embrace it?"
            };

            // Display a random power-up dialogue from the array
            Random random = new Random();
            string selectedDialogue = powerUpDialogues[random.Next(powerUpDialogues.Length)];

            // Display the random power-up dialogue in a MessageBox
            DialogResult result = MessageBox.Show(selectedDialogue, "Power-up Decision", MessageBoxButtons.YesNoCancel);

            // Handle the player's choice based on the result of the MessageBox
            HandlePlayerChoice(result);
        }

        private void HandlePlayerChoice(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.Yes:
                    // Speed Boost
                    player.Speed = 30;
                    MessageBox.Show("You've gained a Speed Enhancement! (+10 Player Speed)", "Outcome of the decision", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case DialogResult.No:
                    // Lower Speed
                    player.Speed = 10;
                    MessageBox.Show("You've incurred a Speed Reduction... (Player Speed -10)", "Outcome of the decision", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case DialogResult.Cancel:
                    // Default Speed
                    player.Speed = 20;
                    MessageBox.Show("You choose to ignore it and nothing happens...", "Outcome of the decision", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void KeyDownMovement(object sender, KeyEventArgs e)
        {
            player.HandleKeyDown(e.KeyCode);
            // Handle player shooting
            player.Shoot(e.KeyCode, projectiles, lastShootTime, shootInterval);
        }

        private void KeyUpMovement(object sender, KeyEventArgs e)
        {
            player.HandleKeyUp(e.KeyCode);
        }

        private void InitializeLastWave()
        {
            Size imageSize = new Size(250, 250); // Desired size for the enemy frames

            Enemy eliteEnemy = new Enemy();
            eliteEnemy.Health = 30;
            eliteEnemy.Speed = 3;

            eliteEnemy.SetAnimationFrames("images/enemies/elite_enemy.gif", imageSize);
            eliteEnemy.Location = new Point(this.Width / 2 - eliteEnemy.Width / 2, -150);
            enemies.Add(eliteEnemy);
            this.Controls.Add(eliteEnemy);

            // Timer setup for challenging wave
            Timer challengingWaveTimer = new Timer();
            challengingWaveTimer.Interval = 5000; // 3000 milliseconds = 3 seconds
            int challengingWaveCounter = 0;
            challengingWaveTimer.Tick += (sender, e) =>
            {
                if (challengingWaveCounter < 5) // Call InitializeEnemies twice
                {
                    InitializeEnemies(1, 1, 1, 5);
                    challengingWaveCounter++;
                }
                else
                {
                    challengingWaveTimer.Stop(); // Stop the timer after two calls
                }
            };
            challengingWaveTimer.Start();
        }

        private void InitializeEnemies(int numberOfEnemies, int minHealth, int maxHealth, int speed)
        {
            Random rand = new Random();
            List<Point> usedPositions = new List<Point>();

            for (int i = 0; i < numberOfEnemies; i++)
            {
                Enemy enemy = new Enemy();
                enemy.Health = rand.Next(minHealth, maxHealth + 1);
                enemy.Speed = speed;

                Point enemyPosition;
                do
                {
                    enemyPosition = new Point(rand.Next(Width - enemy.Width), 0);
                } while (IsPositionUsed(enemyPosition, usedPositions, enemy.Size));

                usedPositions.Add(enemyPosition);

                enemy.Location = enemyPosition;

                enemies.Add(enemy); // Add the enemy to the list
                Controls.Add(enemy); // Add the enemy to the form's controls
            }
        }

        // Method to check if a position has already been used or overlaps with other positions
        private bool IsPositionUsed(Point newPosition, List<Point> usedPositions, Size sizeToCheck)
        {
            foreach (Point usedPosition in usedPositions)
            {
                // Check if the new position overlaps with any used position based on the size of the object
                if (newPosition.X < usedPosition.X + sizeToCheck.Width &&
                    newPosition.X + sizeToCheck.Width > usedPosition.X &&
                    newPosition.Y < usedPosition.Y + sizeToCheck.Height &&
                    newPosition.Y + sizeToCheck.Height > usedPosition.Y)
                {
                    return true; // Position overlaps; it's already used
                }
            }
            return false; // Position is not used or doesn't overlap
        }

        // Method to handle progression to the next wave
        private void ProceedToNextWave()
        {
            // Increase the wave counter
            currentWave++;

            // Check if there are more waves or if the game should end after a certain number of waves
            if (currentWave <= 5) // Change this number to adjust the total number of waves
            {
                // Logic for different waves
                switch (currentWave)
                {
                    case 2:
                        InitializeEnemies(2, 2, 5, 8); // Second wave with different parameters
                        break;
                    case 3:
                        InitializeEnemies(3, 2, 6, 8); // Third wave with different parameters
                        break;
                    case 4:
                        InitializeEnemies(3, 5, 10, 10); // Fourth wave with different parameters
                        break;
                    case 5:
                        InitializeLastWave(); // Final wave
                        break;
                }
            }
            else
            {
                // Stop the game
                gameRunning = false;

                Console.WriteLine(Program.GetCurrentTime() + " - Level 3 completed.");

                // Hide the form
                this.Hide();
                // Proceed to Game Over
                Level4 level4 = new Level4();
                level4.Show();
            }
        }

        private void MoveEnemies()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Move(); // Move each enemy
                if (enemy.Bottom >= this.Height)
                {
                    //player.Health--; // If enemies reach the bottom, decrease player health
                    enemy.Top = -enemy.Height; // Reset the enemy position
                }
            }
        }

        private void HandleCollisions()
        {
            // Projectile-Enemy Collision Detection
            foreach (Projectiles projectile in projectiles.ToList()) // Use ToList to avoid enumeration issues when removing items
            {
                foreach (Enemy enemy in enemies.ToList()) // Use ToList to avoid enumeration issues when removing items
                {
                    if (projectile.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        // Handle collision actions
                        enemy.Health--; // Decrease enemy's health
                        if (enemy.Health <= 0)
                        {
                            enemies.Remove(enemy); // Remove enemy from the list
                            Controls.Remove(enemy); // Remove enemy from the form's controls
                        }
                        projectiles.Remove(projectile); // Remove the projectile
                        Controls.Remove(projectile); // Remove the projectile from the form
                        break; // Break out of the inner loop since the projectile can only hit one enemy
                    }
                }
            }

            // Enemy-Player Collision Detection
            foreach (Enemy enemy in enemies.ToList()) // Use ToList to avoid enumeration issues when removing items
            {
                if (enemy.Bounds.IntersectsWith(player.Bounds))
                {
                    // Handle collision actions
                    player.Health--; // Decrease player's health
                    enemy.Health--; // Decrease enemy's health
                    if (enemy.Health <= 0)
                    {
                        enemies.Remove(enemy); // Remove enemy from the list
                        Controls.Remove(enemy); // Remove enemy from the form's controls
                    }
                    enemy.Top = -enemy.Height; // Reset the enemy position
                }
            }

            // Check if all enemies are defeated to proceed to the next wave
            if (enemies.Count == 0)
            {
                ProceedToNextWave(); // Proceed to the next wave
            }
        }

        // Used DrawImage instead of PictureBox to avoid frame rate issues
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw player
            e.Graphics.DrawImage(player.Image, player.Location);

            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                e.Graphics.DrawImage(enemy.Image, enemy.Location);
            }

            // Draw hearts for player health using Player's DrawHearts method
            player.DrawHearts(e.Graphics);
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (!gameRunning)
            {
                // Game is no longer running, stop updating
                gameTimer.Stop();
                return;
            }

            // Game update logic (movement, collisions, etc.)
            player.Move();
            MoveEnemies();
            UpdateProjectiles();
            HandleCollisions();

            // Refresh the form to invoke OnPaint and redraw the game
            this.Invalidate();

            // Check if player's health is zero
            if (player.Health <= 0)
            {
                // Stop the game
                gameRunning = false;

                Console.WriteLine(Program.GetCurrentTime() + " - Level 3 Game Over.");

                // Hide the form
                this.Hide();
                // Proceed to Game Over
                GameOver gameOverForm = new GameOver();
                gameOverForm.Show();
            }
        }

        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Move();

                // Remove projectiles that go out of bounds
                if (projectiles[i].Top < 0)
                {
                    this.Controls.Remove(projectiles[i]);
                    projectiles.RemoveAt(i);
                }
            }
        }
    }
}
