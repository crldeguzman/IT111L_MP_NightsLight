using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Night_sLight
{
    internal class Conclusion : Form
    {
        private Image bgImage = new Bitmap("images/Game Project BG (3).png");
        // For Dialogues
        private Panel dialoguePanel = new Panel();
        private Label dialogueLabel = new Label();
        private Label levelLabel = new Label();
        private Button continueButton = new Button();

        // For Input Username
        private TextBox textbox1;

        public Conclusion()
        {
            // Form initialization 
            this.Text = "Night's Light (Conclusion)";
            this.Width = 1000;
            this.Height = 600;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = bgImage;
            this.MaximizeBox = false;
            Console.WriteLine(Program.GetCurrentTime() + " - Conclusion (Epilogue) is loaded.");

            SetupDialoguePanel();
        }
        private void SetupDialoguePanel()
        {
            string dialogue1 = "Yusha: I did it! I defeated the demon king!" +
                "\n\n???: I knew that you had it in you, hero Yusha. I congratulate you for being the one to end the reign of the demon king, " +
                "\nand bringing light into this kingdom." +
                "\n\nYusha: Thank you, but I want to know, who are you? and why did you help me defeat the demon king? and what was that " +
                "\namulet's powers?" +
                "\n\nElysia: I am but a divine sentinel, named Elysia, who guides heroes to defeat the demon king, so that he can't bring " +
                "\nforth the eternal darkness. He has ended too many undeserving souls, and it had to be stopped. That amulet of yours is" +
                "\nan ancient relic that is imbued with the power of the Goddess, and it can bring light to any darkness. The reason that " +
                "\nthe amulet is inside the Abyssal Fortress is because the previous hero who came close to defeating the demon king was " +
                "\ndefeated by Malgrim himself, and it was left inside the fortress. The reason that the demon king Malgrim couldn't get " +
                "\nrid of it was because demons can not touch that amulet, as it's an amulet that responds only to bravery, willpower, " +
                "\nand righteousness, which the demons do not have." +
                "\n\nElysia: But enough about that, I would like to thank you once again for making the souls of the ones who died by the " +
                "\nhands of the demon king be able to rest freely now." +
                "\n\nYusha: I would also like to thank you for guiding me, and helping me avenge my family and hometown. I can now be at peace, " +
                "\nknowing that the demon king is gone, and soon all of the demon king army will be wiped out and no one will have to experience" +
                "\na loss of life by their hands ever again.";

            dialoguePanel.BackgroundImage = Image.FromFile("images/Dialogues BG (2).png");
            dialoguePanel.BackgroundImageLayout = ImageLayout.Stretch; // Adjust to fit the panel
            dialoguePanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            dialoguePanel.Location = new Point(0, 0);
            dialoguePanel.Visible = true;

            // To determine what level is the current panel
            levelLabel.Text = "Epilogue: THE HERO DEFEATS THE DEMON KING";
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
            continueButton.Text = "END";
            continueButton.Font = new Font("Century Gothic", 13, FontStyle.Bold);
            continueButton.FlatStyle = FlatStyle.Flat;
            continueButton.FlatAppearance.BorderSize = 0;
            continueButton.BackColor = Color.FromArgb(50, 71, 73, 115);
            continueButton.ForeColor = Color.White;
            continueButton.AutoSize = true;
            continueButton.Location = new Point(850, (dialogueLabel.Location.Y + dialogueLabel.Height) + 0);
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

            // Start the game by initializing enemies and starting the timer
            InitializeNameSaving();

            // Set focus back to the game form
            this.Focus();
        }
        private void InitializeNameSaving()
        {
            // Enter Name
            Label label2 = new Label();
            label2.Text = "Insert Username:";
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent; 
            label2.ForeColor = Color.White;
            //label2.ForeColor = Color.FromArgb(7, 22, 48);
            label2.Font = new Font("Century Gothic", 15, FontStyle.Regular);
            label2.Location = new Point(400,200);
            this.Controls.Add(label2);

            textbox1 = new TextBox();
            textbox1.Location = new Point(400, 230);
            textbox1.Size = new Size(250, 20);
            textbox1.Multiline = true;
            textbox1.Font = new Font("Century Gothic", 11, FontStyle.Regular);
            textbox1.ForeColor = Color.FromArgb(124, 133, 148);
            textbox1.BorderStyle = BorderStyle.None;
            textbox1.BackColor = Color.White;
            textbox1.Text = "Enter your username"; // Placeholder text
            this.Controls.Add(textbox1);

            Panel linePanel1 = new Panel();
            linePanel1.BackColor = Color.FromArgb(7, 22, 48); 
            linePanel1.Size = new Size(250, 1);
            linePanel1.Location = new Point(400, 231);
            this.Controls.Add(linePanel1);
        }
    }
}
