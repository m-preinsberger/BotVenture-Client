namespace BotVenture
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            apikeyTextBox = new TextBox();
            apikeyLabel = new Label();
            JoinButton = new Button();
            GameIdTextBox = new TextBox();
            LobbiesListbox = new ListBox();
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            GameIDLabel = new Label();
            CreateButton = new Button();
            LevelComboBox = new ComboBox();
            StartButton = new Button();
            RunningCheckBox = new CheckBox();
            TakerMoreLobbies = new Button();
            SuspendLayout();
            // 
            // apikeyTextBox
            // 
            apikeyTextBox.Location = new Point(12, 27);
            apikeyTextBox.Name = "apikeyTextBox";
            apikeyTextBox.Size = new Size(154, 23);
            apikeyTextBox.TabIndex = 0;
            apikeyTextBox.Text = "Enter your API-Key here";
            apikeyTextBox.TextChanged += apikeyTextBox_TextChanged;
            // 
            // apikeyLabel
            // 
            apikeyLabel.AutoSize = true;
            apikeyLabel.Location = new Point(12, 9);
            apikeyLabel.Name = "apikeyLabel";
            apikeyLabel.Size = new Size(47, 15);
            apikeyLabel.TabIndex = 1;
            apikeyLabel.Text = "API Key";
            // 
            // JoinButton
            // 
            JoinButton.Enabled = false;
            JoinButton.Location = new Point(310, 27);
            JoinButton.Name = "JoinButton";
            JoinButton.Size = new Size(75, 23);
            JoinButton.TabIndex = 2;
            JoinButton.Text = "Join";
            JoinButton.UseVisualStyleBackColor = true;
            JoinButton.TextChanged += commitButton_TextChanged;
            JoinButton.Click += commitButton_Click;
            // 
            // GameIdTextBox
            // 
            GameIdTextBox.Location = new Point(204, 27);
            GameIdTextBox.Name = "GameIdTextBox";
            GameIdTextBox.Size = new Size(100, 23);
            GameIdTextBox.TabIndex = 3;
            GameIdTextBox.Text = "Game-ID to join";
            GameIdTextBox.TextChanged += GameIdTextBox_TextChanged;
            // 
            // LobbiesListbox
            // 
            LobbiesListbox.FormattingEnabled = true;
            LobbiesListbox.ItemHeight = 15;
            LobbiesListbox.Location = new Point(566, 41);
            LobbiesListbox.Name = "LobbiesListbox";
            LobbiesListbox.Size = new Size(222, 319);
            LobbiesListbox.TabIndex = 4;
            // 
            // button1
            // 
            button1.Location = new Point(727, 12);
            button1.Name = "button1";
            button1.Size = new Size(61, 23);
            button1.TabIndex = 5;
            button1.Text = "Refresh";
            button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(566, 16);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 6;
            label1.Text = "Game list";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(204, 9);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 7;
            label2.Text = "Your Game-ID:";
            // 
            // GameIDLabel
            // 
            GameIDLabel.AutoSize = true;
            GameIDLabel.Location = new Point(310, 9);
            GameIDLabel.Name = "GameIDLabel";
            GameIDLabel.Size = new Size(0, 15);
            GameIDLabel.TabIndex = 8;
            // 
            // CreateButton
            // 
            CreateButton.Location = new Point(12, 85);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new Size(154, 23);
            CreateButton.TabIndex = 9;
            CreateButton.Text = "Create";
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += CreateButton_Click;
            // 
            // LevelComboBox
            // 
            LevelComboBox.FormattingEnabled = true;
            LevelComboBox.Location = new Point(12, 56);
            LevelComboBox.Name = "LevelComboBox";
            LevelComboBox.Size = new Size(154, 23);
            LevelComboBox.TabIndex = 10;
            LevelComboBox.SelectedIndexChanged += LevelComboBox_SelectedIndexChanged;
            // 
            // StartButton
            // 
            StartButton.Enabled = false;
            StartButton.Location = new Point(12, 114);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(154, 23);
            StartButton.TabIndex = 11;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // RunningCheckBox
            // 
            RunningCheckBox.AutoSize = true;
            RunningCheckBox.Location = new Point(628, 15);
            RunningCheckBox.Name = "RunningCheckBox";
            RunningCheckBox.Size = new Size(103, 19);
            RunningCheckBox.TabIndex = 12;
            RunningCheckBox.Text = "Show Running";
            RunningCheckBox.UseVisualStyleBackColor = true;
            RunningCheckBox.CheckedChanged += RunningCheckBox_CheckedChanged;
            // 
            // TakerMoreLobbies
            // 
            TakerMoreLobbies.Location = new Point(566, 366);
            TakerMoreLobbies.Name = "TakerMoreLobbies";
            TakerMoreLobbies.Size = new Size(222, 23);
            TakerMoreLobbies.TabIndex = 13;
            TakerMoreLobbies.Text = "Load more Lobies";
            TakerMoreLobbies.UseVisualStyleBackColor = true;
            TakerMoreLobbies.Click += TakerMoreLobbies_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(TakerMoreLobbies);
            Controls.Add(RunningCheckBox);
            Controls.Add(StartButton);
            Controls.Add(LevelComboBox);
            Controls.Add(CreateButton);
            Controls.Add(GameIDLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(LobbiesListbox);
            Controls.Add(GameIdTextBox);
            Controls.Add(JoinButton);
            Controls.Add(apikeyLabel);
            Controls.Add(apikeyTextBox);
            Name = "Form1";
            Text = "BotVenture";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox apikeyTextBox;
        private Label apikeyLabel;
        private Button JoinButton;
        private TextBox GameIdTextBox;
        private ListBox LobbiesListbox;
        private Button button1;
        private Label label1;
        private Label label2;
        private Label GameIDLabel;
        private Button CreateButton;
        private ComboBox LevelComboBox;
        private Button StartButton;
        private CheckBox RunningCheckBox;
        private Button TakerMoreLobbies;
    }
}
