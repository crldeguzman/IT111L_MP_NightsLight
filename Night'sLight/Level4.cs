/*
    Programmed by: Jasper John M. Raguin
    Date: 11/18/23
    LEVEL 4:
    Program Description: This program will be the level 4 of our game and we will encounter and fight
    the demon king using letters J and K for attacks.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Night_sLight
{
    public class Projectile
    {
        private Image projectileImage;
        private int projectileX;
        private int projectileY;
        private int projectileSpeed;
        private float projectileScale; // Added variable for scaling
        private int damage = 1; // Added variable for damage
        private float velocityX; // Added variable for scaling
        private float velocityY; // Added variable for scaling
        private int scaledWidth;
        private int scaledHeight;
        public Projectile(int x, int y, int speed, float scale, int damage, double angle)
        {
            projectileImage = Image.FromFile("images/1_0.png");
            projectileX = x;
            projectileY = y;
            projectileSpeed = speed;
            projectileScale = scale;
            this.damage = damage;
            // Calculate the initial velocity components based on the angle
            velocityX = (float)(projectileSpeed * Math.Cos(angle));
            velocityY = (float)(projectileSpeed * Math.Sin(angle));
            // Initialize scaledWidth and scaledHeight
            scaledWidth = (int)(projectileImage.Width * projectileScale);
            scaledHeight = (int)(projectileImage.Height * projectileScale);
        }
        public void Draw(Graphics canvas)
        {
            // Draw the scaled projectile maintaining aspect ratio
            canvas.DrawImage(projectileImage, projectileX, (projectileY + 20), scaledWidth, scaledHeight);
        }
        public void Update()
        {
            // Update the projectile's position based on the calculated velocities
            projectileX += (int)velocityX;
            projectileY += (int)velocityY;
        }
        public int GetX()
        {
            return projectileX;
        }
        public int GetY()
        {
            return projectileY;
        }
        public int GetWidth()
        {
            return scaledWidth;
        }
        public int GetHeight()
        {
            return scaledHeight;
        }
        public int GetDamage()
        {
            // Return the damage value of the projectile
            return damage;
        }
    }
    // Boss class
    public class Boss
    {
        // Boss properties
        private Player player;
        private List<Image> bossImages;
        private int currentFrame;
        private int bossX;
        private int bossY;
        private int bossHeight;
        private int bossWidth;
        private int bossSpeed;
        public int bossHealth;
        private int attackCooldown;
        private int attackCooldownMax = 50; // Adjust as needed
        private int followDistance = 200; // Adjust as needed
        private Random random; // Used for randomizing boss movements
        private List<Projectile> projectiles = new List<Projectile>();

        public PictureBox bossPictureBox { get; private set; }

        public List<Projectile> GetProjectiles()
        {
            return projectiles;
        }
        // Constructor
        public Boss(int health, Player player)
        {
            currentFrame = 0;

            bossHeight = 406;
            bossWidth = 328;

            bossX = 650; // Set the initial X position
            bossY = 180; // Set the initial Y position

            bossHealth = health;
            this.player = player; // Initialize the player field

            InitializeBossPictureBox();
        }
        private void InitializeBossPictureBox()
        {
            // Initialize boss PictureBox
            bossPictureBox = new PictureBox();
            bossPictureBox.BackColor = Color.Transparent;
            bossPictureBox.Size = new Size(bossWidth, bossHeight);
            bossPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            bossPictureBox.Location = new Point(bossX, bossY);
            bossPictureBox.Image = Image.FromFile("images/boss_4(updated).gif");
        }
        public void DrawProjectiles(Graphics canvas)
        {
            // Draw all boss projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(canvas);
            }
        }
        public void UpdateProjectiles()
        {
            // Update all boss projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update();
            }

            // Remove boss projectiles that are off-screen (optional)
            projectiles.RemoveAll(projectile => projectile.GetX() > 1020);
        }

        public void DecreaseHealth(int damage)
        {
            bossHealth -= damage;

        }

        public void HandleProjectileCollision(Projectile projectile)
        {

            DecreaseHealth(projectile.GetDamage());
        }

        //private void DecreasePlayerHealth(int damage)
        //{
        //    player.DecreaseHealth(damage);
        //}

        public bool CanShoot()
        {
            return attackCooldown <= 0;
        }
        public void ShootProjectiles(int playerX, int playerY)
        {
            // Shoot a single projectile
            ShootProjectile(playerX, playerY);
        }
        private void ShootProjectile(int playerX, int playerY)
        {
            double angle = Math.PI; // Use PI angle for shooting to the left
            // Set the scale factor for the projectile
            float projectileScale = 2.25f; // Adjust the scale factor as needed
            int projectileDamage = 1; // Change this to the desired damage value

            // Create a new projectile and add it to the list
            projectiles.Add(new Projectile((bossX - 200) + bossWidth / 2, bossY + bossHeight / 2, 10, projectileScale, projectileDamage, angle));
        }
        public void ResetShootCooldown()
        {
            attackCooldown = attackCooldownMax;
        }
        public void UpdateShootCooldown()
        {
            if (attackCooldown > 0)
            {
                attackCooldown--;
            }
        }
        private bool IsProjectileColliding(Projectile projectile, float projectileScale)
        {
            Rectangle bossVisibleRect = new Rectangle(bossX + 140, bossY, bossWidth - 120, bossHeight);

            // Adjust the projectile rectangle based on the scaling factor
            Rectangle projectileRect = new Rectangle(projectile.GetX(), projectile.GetY(), (int)(projectile.GetWidth() * projectileScale), (int)(projectile.GetHeight() * projectileScale));

            if (projectileRect.IntersectsWith(bossVisibleRect))
            {
                DecreaseHealth(projectile.GetDamage());
            }
            return bossVisibleRect.IntersectsWith(projectileRect);
        }
        public void AttackPlayer(Player player)
        {
            // Check for projectile collisions
            List<Projectile> projectiles = player.GetProjectiles();

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = projectiles[i];

                if (IsProjectileColliding(projectile, 1f))
                {
                    player.RemoveProjectile(projectile); // Remove the projectile after hitting the boss
                }
            }
        }
        public bool IsDefeated()
        {
            return bossHealth <= 0;
        }
        public int GetX()
        {
            return bossX;
        }
        public int GetY()
        {
            return bossY;
        }
        public int GetWidth()
        {
            return bossWidth;
        }
        public int GetHeight()
        {
            return bossHeight;
        }
    }
    // Player class
    public class Player
    {
        // Player properties
        private Image playerImage;
        private List<string> playerMovements;
        private int steps;
        private float slowDownFrameRate;
        private int playerX;
        private int playerY;
        private int playerHeight;
        private int playerWidth;
        private int playerSpeed;
        private int playerAttackPower;
        private bool isJumping;
        private int jumpSpeed = 10;
        private int jumpHeight = 100;
        private int jumpCount;
        private int playerHealth;
        private int playerDirection = 1;
        private bool isMeleeAttacking = false;
        private int meleeAttackCount = 0;
        private bool isPerformingMeleeAttack = false;
        private bool isShooting = false;
        private int shootAnimationCount = 0;
        private Timer shootCooldownTimer;
        private bool canJump = true;
        private bool canShoot = true;
        private DateTime lastShootTime = DateTime.MinValue;
        private TimeSpan shootInterval = TimeSpan.FromSeconds(1.25);
        private List<Projectile> projectiles = new List<Projectile>();
        private Rectangle playerHitbox;
        // Directional flags
        private bool left, right, up, spaceBar;
        // Constants
        private const int GravityFallSpeed = 5;
        private const int MaxXPosition = 410;
        private const int MinXPosition = -110;
        private const int FallingGroundLevel = 585;
        private const int ProjectileRemoveThreshold = 1020;
        private const int MeleeAttackFrameThreshold = 52;
        private const int ShootingAnimationStart = 49;
        private const int ShootingAnimationEnd = 54;
        private const double AnimationSlowDownRate = 4.0;
        public List<Projectile> GetProjectiles()
        {
            return projectiles;
        }
        public void RemoveProjectile(Projectile projectile)
        {
            projectiles.Remove(projectile);
        }
        // Constructor
        public Player(int health)
        {
            playerMovements = Directory.GetFiles("images/Lvl4(player)", "*.png").ToList();
            playerImage = Image.FromFile(playerMovements[0]);
            steps = 0;
            slowDownFrameRate = 0;
            playerX = 100;
            playerY = 160;
            playerHeight = 380;
            playerWidth = 380;
            playerSpeed = 8;
            isJumping = false;
            jumpSpeed = 10;
            jumpHeight = 150;
            jumpCount = 0;
            projectiles = new List<Projectile>();
            playerHealth = health;
            playerHitbox = new Rectangle(playerX, playerY, (playerWidth - 320), (playerHeight - 220));
        }
        public bool IsMeleeAttacking()
        {
            return isMeleeAttacking;
        }
        // Update player health
        public void DecreaseHealth(int damage)
        {
            playerHealth -= damage;

            // Check if the player is defeated
            if (playerHealth <= 0)
            {
                // Handle player defeat logic (customize based on your game requirements)
                Console.WriteLine("Player defeated!");
                // You may want to implement a game over state or transition to another level/menu here
            }
        }
        public bool IsShooting()
        {
            return isShooting;
        }
        // Reset shooting state
        public void ResetShootingState()
        {
            isShooting = false;
            shootAnimationCount = 0;
        }
        public void Shoot()
        {
            if (!canShoot)
            {
                return; // If canShoot is false, do not proceed with shooting
            }
            double angle = 0;
            // Set the scale factor for the projectile
            float projectileScale = 2.25f; // Adjust the scale factor as needed
                                           // Create a new projectile and add it to the list
            projectiles.Add(new Projectile(playerX + playerWidth / 2, playerY + playerHeight / 2, 10, projectileScale, 2, angle));
            canShoot = false;
            // Use a Timer to re-enable shooting after the specified interval
            shootCooldownTimer = new Timer();
            shootCooldownTimer.Interval = (int)shootInterval.TotalMilliseconds;
            shootCooldownTimer.Tick += (sender, args) =>
            {
                canShoot = true; // Enable shooting again
                shootCooldownTimer.Stop();
                shootCooldownTimer.Dispose();
            };
            shootCooldownTimer.Start();
        }
        public void DrawProjectiles(Graphics canvas)
        {
            // Draw all projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(canvas);
            }
        }
        public void UpdateProjectiles()
        {
            // Update all boss projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update();
            }

            // Remove boss projectiles that are off-screen (optional)
            projectiles.RemoveAll(projectile => projectile.GetX() > 1020);
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

            // Check for melee attack key
            if (keyCode == Keys.J)
            {
                isPerformingMeleeAttack = true;
                isMeleeAttacking = true;
                meleeAttackCount = 0;
            }
            // Check for projectile attack key
            if (keyCode == Keys.K)
            {
                // Set the shooting animation state
                isShooting = true;

                // Shoot regardless of cooldown
                Shoot();
            }
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

            // Reset melee attack state on key up
            if (keyCode == Keys.J)
            {
                isPerformingMeleeAttack = false;
                isMeleeAttacking = false;
                meleeAttackCount = 0;
            }

            // Reset projectile attack state on key up
            if (keyCode == Keys.K)
            {
                isShooting = false;
                shootAnimationCount = 0;
            }
        }
        public void Draw(Graphics canvas)
        {
            // Check the direction and flip the image accordingly
            if (playerDirection == 1)
            {
                canvas.DrawImage(playerImage, playerX, playerY, playerWidth, playerHeight);
            }
            else
            {
                // Flip the image horizontally
                canvas.DrawImage(playerImage, playerX + playerWidth, playerY, -playerWidth, playerHeight);
            }

            DrawProjectiles(canvas);
        }

        public void CheckProjectileCollisions(List<Projectile> bossProjectiles)
        {
            List<Projectile> copyOfBossProjectiles = new List<Projectile>(bossProjectiles);
            foreach (Projectile projectile in copyOfBossProjectiles)
            {
                if (IsCollidingWithProjectile(projectile))
                {
                    DecreaseHealth(projectile.GetDamage());
                    Console.WriteLine("Player Health after hit: " + playerHealth);

                    // Handle player defeat logic
                    if (playerHealth <= 0)
                    {
                        Console.WriteLine("Player defeated!");
                        // Implement game over logic or transition to another state/level
                    }

                    // Remove the projectile after hitting the player from the original list
                    bossProjectiles.Remove(projectile);
                    break; // Break out of the loop after handling the collision with one projectile
                }
            }
        }

        private bool IsCollidingWithProjectile(Projectile projectile)
        {
            Rectangle projectileRectangle = new Rectangle(projectile.GetX(), projectile.GetY(), projectile.GetWidth(), projectile.GetHeight());

            // Check if the player's hitbox intersects with the projectile's hitbox
            return playerHitbox.IntersectsWith(projectileRectangle);
        }

        public bool IsJumping()
        {
            return isJumping;
        }
        // Start the jump
        public void StartJump()
        {
            if (canJump)
            {
                isJumping = true;
                jumpCount = 0;
                canJump = false; // Set to false when the jump starts
            }
        }
        // Update player position and animation

        public void Update()
        {
            if (isJumping)
            {
                Jump();
                AnimationPlayer(18, 38);
            }
            else
            {
                ApplyGravity();
                UpdateHitboxPosition(); // Update hitbox position when not jumping

                // Update player hitbox position
                playerHitbox.X = playerX + 135;
                playerHitbox.Y = playerY + 210;

                // Check for left and right movement
                if (left && playerX >= -110)
                {
                    playerX -= playerSpeed;
                    AnimationPlayer(40, 46);
                    ResetAnimationIfNotMoving(); // Reset animation if the player is not moving
                }
                else if (right && playerX <= 410)
                {
                    playerX += playerSpeed;
                    AnimationPlayer(40, 46);
                    ResetAnimationIfNotMoving(); // Reset animation if the player is not moving
                }
                // Check for jumping
                else if (up && !isJumping)
                {

                    StartJump();
                }
                else
                {
                    // Rest of the code remains the same
                    if (isMeleeAttacking)
                    {
                        AnimationPlayer(3, 10);
                        meleeAttackCount++;

                        if (meleeAttackCount > 52)
                        {
                            isMeleeAttacking = false;
                            meleeAttackCount = 0;

                            // Optionally reset the melee attack state when the animation completes
                            // You may adjust this based on your animation frame count
                        }
                    }
                    else if (isShooting)
                    {
                        // Shooting animation frames
                        AnimationPlayer(49, 54);
                        if (canShoot && DateTime.Now - lastShootTime >= shootInterval)
                        {
                            Shoot();
                            lastShootTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        AnimationPlayer(11, 18);
                    }
                    UpdateProjectiles();

                }
            }
        }
        public void Update(Boss boss)
        {
            Update();
        }
        private void ResetAnimationIfNotMoving()
        {
            if (!left && !right)
            {
                steps = 0;
            }
        }
        private void ApplyGravity()
        {
            // Check if the player is above the ground level and not jumping
            if (!isJumping && playerY < 585 - playerHeight)
            {
                // Increment the player's Y position to simulate falling
                playerY += 5; // Adjust the value to control the falling speed
            }
            else
            {
                // Snap the player to the ground level
                playerY = 585 - playerHeight;
                canJump = true; // Allow jumping when the player is on the ground
            }
        }
        // Handle player animation
        private void AnimationPlayer(int start, int end)
        {
            slowDownFrameRate += 1;

            if (slowDownFrameRate == 4.0)
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
        // Handle the jumping motion
        private void Jump()
        {
            if (jumpCount < jumpHeight)
            {
                playerY -= jumpSpeed;
                jumpCount += jumpSpeed;
                UpdateHitboxPosition(); // Update hitbox position during jump
            }
            else
            {
                if (playerY + playerHeight < FallingGroundLevel)
                {
                    playerY += GravityFallSpeed;
                    UpdateHitboxPosition(); // Update hitbox position during falling
                }
                else
                {
                    isJumping = false;
                    jumpCount = 0;
                    canJump = true;
                }
            }
        }

        private void UpdateHitboxPosition()
        {
            // Update player hitbox position during movement
            playerHitbox.X = playerX + 135; // Adjust as needed
            playerHitbox.Y = playerY + 210; // Adjust as needed
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
        // Increase player's attack power
        public void IncreasePower(int amount)
        {
            playerAttackPower += amount;
        }
        // Get player's attack power
        public int GetPower()
        {
            return playerAttackPower;
        }
    }
    internal class Level4 : Form
    {
        // Player-related variables
        private Player player;
        private int playerHealth = 5; // Set player health to 5
        // Boss-related variables
        private Boss boss;
        private int bossHealth = 15; // Set boss health to 15
        private bool bossDefeated = false;
        // UI Labels
        private Label lblCollected;
        private Label lblPower;
        private Label lblHearts;
        private PictureBox[] heartPictureBoxes;
        // Level transition flag
        private bool gameOver = false;
        // Constructor
        public Level4()
        {
            Console.WriteLine(Program.GetCurrentTime() + " - Level 4 loaded.");
            MessageBox.Show("PLAYER MOVEMENT: Up-Down-Left-Right Arrow Keys, letter J for melee attack and letter K to shoot.");
            InitializeForm();
            InitializeEvents();
            InitializeTimer();
            this.DoubleBuffered = true;
            gameOver = false;
            // Set initial health values
            playerHealth = 5;
            bossHealth = 30;
            // Player and boss initialization
            player = new Player(playerHealth); // Pass player health to the Player constructor
            boss = new Boss(bossHealth, player); // Pass boss health to the Boss constructor

            // Add the boss PictureBox to the form's Controls collection
            this.Controls.Add(boss.bossPictureBox);
        }
        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            // Draw player, boss, and projectiles
            player.Draw(canvas);
            player.DrawProjectiles(canvas); // Add this line to draw player projectiles
            boss.DrawProjectiles(canvas);
            // Update health labels
            lblHearts.Text = "Player Health: " + playerHealth;
            lblPower.Text = "Boss Health: " + boss.bossHealth;
        }
        // Initialize the main form
        private void InitializeForm()
        {
            // Form properties
            this.Text = "Night's Light (Level 4)";
            this.Width = 1020;
            this.Height = 670;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = Image.FromFile("images/Lvl4(background).jpg");
            // Initialize health labels
            lblHearts = InitializeLabel("Player Health: " + playerHealth, new Point(10, 10));
            this.Controls.Add(lblHearts);
            lblPower = InitializeLabel("Boss Health: " + bossHealth, new Point(this.Width - 200, 10));
            this.Controls.Add(lblPower);
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
            label.ForeColor = Color.Beige;
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
            Timer timer = new Timer();
            timer.Tick += new EventHandler(TimerEvent);
            timer.Interval = 1000 / 60;
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

            if (e.KeyCode == Keys.NumPad2)
            {
                player.ResetShootingState();
            }
        }
        // Timer event handler for game updates
        private void TimerEvent(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                player.Update(boss);
                player.UpdateProjectiles();
                if (!bossDefeated)
                {
                    boss.AttackPlayer(player);
                    // Boss shooting logic
                    if (boss.CanShoot())
                    {
                        boss.ShootProjectiles(player.GetX(), player.GetY());
                        boss.ResetShootCooldown();
                    }
                    boss.UpdateShootCooldown();
                    boss.UpdateProjectiles();
                    if (boss.IsDefeated())
                    {
                        bossDefeated = true;
                        // Display a MessageBox when the boss is defeated
                        Console.WriteLine(Program.GetCurrentTime() + " - Level 4 completed.");
                        // Add any additional logic or transition to the next level/menu as needed
                        // For now, close the current form
                        this.Close();
                        Conclusion conclusion = new Conclusion();
                        conclusion.Show();
                    }
                }
                if (player.IsShooting())
                {
                    player.Shoot();
                }
                player.CheckProjectileCollisions(boss.GetProjectiles()); // Check collisions with boss projectiles
                boss.UpdateProjectiles();
                this.Invalidate();
            }
            else
            {
                ((Timer)sender).Stop();
            }
        }
    }
}
