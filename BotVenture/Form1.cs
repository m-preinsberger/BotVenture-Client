namespace BotVenture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LevelComboBox.DataSource = Enum.GetValues(typeof(Level));
            FilterComboBox.DataSource = Enum.GetValues(typeof(LobbyFilter));
            LobbiesListbox.DataSource = LobbyList;
            LobbiesListbox.DisplayMember = "Host";
        }

        public string API_KEY { get; internal set; } = string.Empty;
        public string GameId { get; private set; } = string.Empty;
        internal bool APISet { set; get; }
        private string OwnHostName { get; set; } = "Prometheus";
        public DateTime StartTime { get; set; }
        public DateTime StartAt { get; set; }
        public int TakeLobbiesNum { get; set; } = 20;
        public LobbyFilter LobbyFilter { get; set; } = LobbyFilter.open;
        public Lobby[] LobbyList { get; set; } = new Lobby[0];

        public bool DEBUG { get; private set; } = true;
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
            else
            {
                MessageBox.Show("API-Key not set", "API-Key missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void Form1_Load(object sender, EventArgs e)
        {
            // TODO load the lobies available
            await RefreshLobbyList();
        }

        private void RunningCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // assigning the value to the property
            // This Checkbox doesnt exist anymore but im afraid to delete it, because winforms doesnt like it when you delete functions
        }

        private async void TakerMoreLobbies_Click(object sender, EventArgs e)
        {
            // increase the number of lobbies the function should return
            TakeLobbiesNum += 20;
            // Call the refresh function with the new parameters
            button1_Click(sender, e);
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameCreated)
            {
                var io = new IO(this);
                await io.CloseGame();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var io = new IO(this);

            // Fetch the lobbies based on the selected LobbyFilter
            switch (LobbyFilter)
            {
                case LobbyFilter.all:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.all, TakeLobbiesNum);
                    break;
                case LobbyFilter.open:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.open, TakeLobbiesNum);
                    break;
                case LobbyFilter.running:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.running, TakeLobbiesNum);
                    break;
            }

            // Refresh the lobby list in the listbox
            LobbiesListbox.DataSource = null; // Reset the data source
            LobbiesListbox.DataSource = LobbyList; // Reassign the updated list
            LobbiesListbox.DisplayMember = "Host"; // Ensure it displays the 'Host' property

            // Check if there is a lobby with the host name matching OwnHostName
            bool hasOwnHostName = LobbyList?.Any(lobby => lobby.Host == OwnHostName) ?? false;
            GameCreated = hasOwnHostName;

            // Update UI elements based on whether the user's game is found
            if (GameCreated)
            {
                LevelComboBox.Enabled = false;
                CreateButton.Text = "Close Game";
                StartButton.Enabled = false;
                GameId = null;
                CreateButton.Enabled = APISet; // Enable CreateButton only if API is set
            }
            else
            {
                LevelComboBox.Enabled = true;
                CreateButton.Text = "Create Game";
                StartButton.Enabled = true;
            }
        }


        private async void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the LobbyFilter property based on the selected item in FilterComboBox
            LobbyFilter = (LobbyFilter)FilterComboBox.SelectedItem;

            // Refresh the lobby list
            await RefreshLobbyList();
        }
        private async Task RefreshLobbyList()
        {
            var io = new IO(this);

            switch (LobbyFilter)
            {
                case LobbyFilter.all:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.all, TakeLobbiesNum);
                    break;
                case LobbyFilter.open:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.open, TakeLobbiesNum);
                    break;
                case LobbyFilter.running:
                    LobbyList = await io.GetCurrentGames(LobbyFilter.running, TakeLobbiesNum);
                    break;
            }

            // Update the list box to show the filtered lobbies
            LobbiesListbox.DataSource = null; // Reset the data source
            LobbiesListbox.DataSource = LobbyList; // Reassign the updated list
            LobbiesListbox.DisplayMember = "Host"; // Ensure it displays the 'Host' property

            // Check if there is a lobby with the host name matching OwnHostName
            bool hasOwnHostName = LobbyList?.Any(lobby => lobby.Host == OwnHostName) ?? false;
            GameCreated = hasOwnHostName;

            // Update UI elements based on the presence of the user's lobby
            if (GameCreated)
            {
                LevelComboBox.Enabled = false;
                CreateButton.Text = "Close Game";
                StartButton.Enabled = false;
                GameId = null;
                if (!APISet) CreateButton.Enabled = false;
                else CreateButton.Enabled = true;
            }
            else
            {
                LevelComboBox.Enabled = true;
                CreateButton.Text = "Create Game";
                StartButton.Enabled = true;
            }
        }
        private void SaveAPIKey_CheckedChanged(object sender, EventArgs e)
        {
            if (APISet)
            {
                // lets save the key in a json file in the project 
            }
        }
    }
}
