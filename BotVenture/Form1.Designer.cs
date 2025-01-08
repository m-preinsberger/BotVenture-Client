namespace BotVenture
{
    partial class BotVentureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BotVentureForm));
            apikeyTextBox = new TextBox();
            apikeyLabel = new Label();
            JoinButton = new Button();
            GameIdTextBox = new TextBox();
            LobbiesListbox = new ListBox();
            RefreshLobbies = new Button();
            label1 = new Label();
            label2 = new Label();
            GameIDLabel = new Label();
            CreateButton = new Button();
            LevelComboBox = new ComboBox();
            StartButton = new Button();
            TakerMoreLobbies = new Button();
            FilterComboBox = new ComboBox();
            SaveAPIKey = new CheckBox();
            DisplayGameStats = new Button();
            TakeControlButton = new RoundButton();
            ButtonMoveUp = new Button();
            ButtonMoveDown = new Button();
            ButtonMoveRight = new Button();
            ButtonMoveLeft = new Button();
            SuspendLayout();
            // 
            // apikeyTextBox
            // 
            apikeyTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            apikeyTextBox.Location = new Point(12, 27);
            apikeyTextBox.Name = "apikeyTextBox";
            apikeyTextBox.Size = new Size(154, 23);
            apikeyTextBox.TabIndex = 0;
            apikeyTextBox.Text = "Enter your API-Key here";
            apikeyTextBox.TextChanged += apikeyTextBox_TextChanged;
            // 
            // apikeyLabel
            // 
            apikeyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            apikeyLabel.AutoSize = true;
            apikeyLabel.Location = new Point(12, 9);
            apikeyLabel.Name = "apikeyLabel";
            apikeyLabel.Size = new Size(47, 15);
            apikeyLabel.TabIndex = 1;
            apikeyLabel.Text = "API Key";
            // 
            // JoinButton
            // 
            JoinButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            JoinButton.Enabled = false;
            JoinButton.Location = new Point(398, 25);
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
            GameIdTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GameIdTextBox.Location = new Point(278, 27);
            GameIdTextBox.Name = "GameIdTextBox";
            GameIdTextBox.Size = new Size(114, 23);
            GameIdTextBox.TabIndex = 3;
            GameIdTextBox.Text = "Game-ID to join";
            GameIdTextBox.TextChanged += GameIdTextBox_TextChanged;
            // 
            // LobbiesListbox
            // 
            LobbiesListbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LobbiesListbox.FormattingEnabled = true;
            LobbiesListbox.ItemHeight = 15;
            LobbiesListbox.Location = new Point(566, 41);
            LobbiesListbox.Name = "LobbiesListbox";
            LobbiesListbox.Size = new Size(222, 319);
            LobbiesListbox.TabIndex = 4;
            LobbiesListbox.DoubleClick += LobbiesListbox_DoubleClick;
            // 
            // RefreshLobbies
            // 
            RefreshLobbies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RefreshLobbies.Location = new Point(727, 12);
            RefreshLobbies.Name = "RefreshLobbies";
            RefreshLobbies.Size = new Size(61, 23);
            RefreshLobbies.TabIndex = 5;
            RefreshLobbies.Text = "Refresh";
            RefreshLobbies.UseVisualStyleBackColor = true;
            RefreshLobbies.Click += button1_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(566, 16);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 6;
            label1.Text = "Game list";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(278, 9);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 7;
            label2.Text = "Your Game-ID:";
            // 
            // GameIDLabel
            // 
            GameIDLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GameIDLabel.AutoSize = true;
            GameIDLabel.Location = new Point(368, 9);
            GameIDLabel.Name = "GameIDLabel";
            GameIDLabel.Size = new Size(0, 15);
            GameIDLabel.TabIndex = 8;
            // 
            // CreateButton
            // 
            CreateButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            LevelComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LevelComboBox.FormattingEnabled = true;
            LevelComboBox.Location = new Point(12, 56);
            LevelComboBox.Name = "LevelComboBox";
            LevelComboBox.Size = new Size(154, 23);
            LevelComboBox.TabIndex = 10;
            LevelComboBox.SelectedIndexChanged += LevelComboBox_SelectedIndexChanged;
            // 
            // StartButton
            // 
            StartButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            StartButton.Enabled = false;
            StartButton.Location = new Point(12, 114);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(154, 23);
            StartButton.TabIndex = 11;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // TakerMoreLobbies
            // 
            TakerMoreLobbies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TakerMoreLobbies.Location = new Point(566, 366);
            TakerMoreLobbies.Name = "TakerMoreLobbies";
            TakerMoreLobbies.Size = new Size(222, 23);
            TakerMoreLobbies.TabIndex = 13;
            TakerMoreLobbies.Text = "Load more Lobies";
            TakerMoreLobbies.UseVisualStyleBackColor = true;
            TakerMoreLobbies.Click += TakerMoreLobbies_Click;
            // 
            // FilterComboBox
            // 
            FilterComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FilterComboBox.FormattingEnabled = true;
            FilterComboBox.Location = new Point(636, 13);
            FilterComboBox.Name = "FilterComboBox";
            FilterComboBox.Size = new Size(85, 23);
            FilterComboBox.TabIndex = 14;
            FilterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            // 
            // SaveAPIKey
            // 
            SaveAPIKey.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SaveAPIKey.AutoSize = true;
            SaveAPIKey.Location = new Point(172, 29);
            SaveAPIKey.Name = "SaveAPIKey";
            SaveAPIKey.Size = new Size(95, 19);
            SaveAPIKey.TabIndex = 15;
            SaveAPIKey.Text = "Save API-Key";
            SaveAPIKey.UseVisualStyleBackColor = true;
            SaveAPIKey.CheckedChanged += SaveAPIKey_CheckedChanged;
            // 
            // DisplayGameStats
            // 
            DisplayGameStats.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DisplayGameStats.Enabled = false;
            DisplayGameStats.Location = new Point(566, 395);
            DisplayGameStats.Name = "DisplayGameStats";
            DisplayGameStats.Size = new Size(222, 23);
            DisplayGameStats.TabIndex = 16;
            DisplayGameStats.Text = "Show game stats";
            DisplayGameStats.UseVisualStyleBackColor = true;
            DisplayGameStats.Click += DisplayGameStats_Click;
            // 
            // TakeControlButton
            // 
            TakeControlButton.Enabled = false;
            TakeControlButton.Location = new Point(307, 190);
            TakeControlButton.Name = "TakeControlButton";
            TakeControlButton.Size = new Size(75, 75);
            TakeControlButton.TabIndex = 17;
            TakeControlButton.Text = "Take Control";
            TakeControlButton.UseVisualStyleBackColor = true;
            TakeControlButton.Click += TakeControlButton_Click;
            // 
            // ButtonMoveUp
            // 
            ButtonMoveUp.Enabled = false;
            ButtonMoveUp.Location = new Point(307, 161);
            ButtonMoveUp.Name = "ButtonMoveUp";
            ButtonMoveUp.Size = new Size(75, 23);
            ButtonMoveUp.TabIndex = 18;
            ButtonMoveUp.Text = "Up";
            ButtonMoveUp.UseVisualStyleBackColor = true;
            ButtonMoveUp.Visible = false;
            ButtonMoveUp.Click += ButtonMoveUp_Click;
            // 
            // ButtonMoveDown
            // 
            ButtonMoveDown.Enabled = false;
            ButtonMoveDown.Location = new Point(307, 271);
            ButtonMoveDown.Name = "ButtonMoveDown";
            ButtonMoveDown.Size = new Size(75, 23);
            ButtonMoveDown.TabIndex = 19;
            ButtonMoveDown.Text = "Down";
            ButtonMoveDown.UseVisualStyleBackColor = true;
            ButtonMoveDown.Visible = false;
            ButtonMoveDown.Click += ButtonMoveDown_Click;
            // 
            // ButtonMoveRight
            // 
            ButtonMoveRight.Enabled = false;
            ButtonMoveRight.Location = new Point(388, 216);
            ButtonMoveRight.Name = "ButtonMoveRight";
            ButtonMoveRight.Size = new Size(75, 23);
            ButtonMoveRight.TabIndex = 20;
            ButtonMoveRight.Text = "Right";
            ButtonMoveRight.UseVisualStyleBackColor = true;
            ButtonMoveRight.Visible = false;
            ButtonMoveRight.Click += ButtonMoveRight_Click;
            // 
            // ButtonMoveLeft
            // 
            ButtonMoveLeft.Enabled = false;
            ButtonMoveLeft.Location = new Point(226, 216);
            ButtonMoveLeft.Name = "ButtonMoveLeft";
            ButtonMoveLeft.Size = new Size(75, 23);
            ButtonMoveLeft.TabIndex = 21;
            ButtonMoveLeft.Text = "Left";
            ButtonMoveLeft.UseVisualStyleBackColor = true;
            ButtonMoveLeft.Visible = false;
            ButtonMoveLeft.Click += ButtonMoveLeft_Click;
            // 
            // BotVentureForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ButtonMoveLeft);
            Controls.Add(ButtonMoveRight);
            Controls.Add(ButtonMoveDown);
            Controls.Add(ButtonMoveUp);
            Controls.Add(TakeControlButton);
            Controls.Add(DisplayGameStats);
            Controls.Add(SaveAPIKey);
            Controls.Add(FilterComboBox);
            Controls.Add(TakerMoreLobbies);
            Controls.Add(StartButton);
            Controls.Add(LevelComboBox);
            Controls.Add(CreateButton);
            Controls.Add(GameIDLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(RefreshLobbies);
            Controls.Add(LobbiesListbox);
            Controls.Add(GameIdTextBox);
            Controls.Add(JoinButton);
            Controls.Add(apikeyLabel);
            Controls.Add(apikeyTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "BotVentureForm";
            Text = "BotVenture";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox apikeyTextBox;
        private Label apikeyLabel;
        private Button JoinButton;
        private TextBox GameIdTextBox;
        private ListBox LobbiesListbox;
        private Button RefreshLobbies;
        private Label label1;
        private Label label2;
        private Label GameIDLabel;
        private Button CreateButton;
        private ComboBox LevelComboBox;
        private Button StartButton;
        private Button TakerMoreLobbies;
        private ComboBox FilterComboBox;
        private CheckBox SaveAPIKey;
        private Button DisplayGameStats;
        private RoundButton TakeControlButton;
        private Button ButtonMoveUp;
        private Button ButtonMoveDown;
        private Button ButtonMoveRight;
        private Button ButtonMoveLeft;
    }
}
