namespace BotVenture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LevelComboBox.DataSource = Enum.GetValues(typeof(Level));
        }

        public string API_KEY { get; internal set; } = string.Empty;
        public string GameId { get; private set; } = string.Empty;
        internal bool APISet { set; get; }
        public DateTime StartTime { get; set; }
        public DateTime StartAt { get; set; }
        public int TakeLobbiesNum { get; set; } = 20;
        public bool ShowRunningLobies { get; set; } = false;

        public bool DEBUG { get; private set; } = false;
        internal bool GameIDSet { set; get; } = false;
        internal bool GameStarted { get; set; } = false;
        internal bool GameCreated { get; set; } = false;
        public void SetGameID(string newID)
        {
            if (!string.IsNullOrEmpty(newID))
            {
                GameId = newID;
                GameIDLabel.Text = GameId;
            }
        }
        private Level ChoosenLevel { get; set; } = Level.level0;
        internal enum Level
        {
            level0
        }

        private async void commitButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(API_KEY) && !string.IsNullOrEmpty(GameId))
            {
                // Create an instance of IO and call JoinGame
                var io = new IO(this);
                await io.JoinGame(GameId);
            }
        }

        private void GameIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GameIdTextBox.Text) || string.IsNullOrEmpty(API_KEY))
            {
                GameIDSet = false;
                JoinButton.Enabled = false;
            }
            else
            {
                GameIDSet = true;
                JoinButton.Enabled = true;
            }
        }

        private void commitButton_TextChanged(object sender, EventArgs e)
        {

        }

        private void apikeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(apikeyTextBox.Text))
            {
                API_KEY = apikeyTextBox.Text;
                APISet = true;
            }
            else
            {
                APISet = false;
            }
        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {
            if (APISet)
            {
                var io = new IO(this);

                if (GameCreated)
                {
                    // If the game is already created, close it
                    await io.CloseGame();
                    GameCreated = false;
                    CreateButton.Text = "Create Game";
                    StartButton.Enabled = false;
                    LevelComboBox.Enabled = true;
                }
                else
                {
                    // If the game is not created, create it
                    await io.CreateGame(ChoosenLevel);
                    if (GameCreated)
                    {
                        LevelComboBox.Enabled = false;
                        CreateButton.Text = "Close Game";
                        StartButton.Enabled = true;
                        GameId = null;
                    }
                }
            }
        }

        private void LevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChoosenLevel = (Level)LevelComboBox.SelectedItem;
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (GameCreated)
            {
                var io = new IO(this);
                var result = await io.StartGame(); // Await the result

                if (result.Now != null && result.StartAt != null) // Check if result is valid
                {
                    StartTime = DateTime.Parse(result.Now);
                    StartAt = DateTime.Parse(result.StartAt);

                    MessageBox.Show($"Game started successfully.\nNow: {StartTime}\nStart At: {StartAt}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to start the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO load the lobies available
        }

        private void RunningCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // asigning the the value to the propertie
            ShowRunningLobies = RunningCheckBox.Checked;
        }

        private void TakerMoreLobbies_Click(object sender, EventArgs e)
        {
            // increase the number of lobbies the function should return
            TakeLobbiesNum += 20;
            // Call the refresh function with the new parameters

        }
    }
}
