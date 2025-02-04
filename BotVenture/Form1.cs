using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
namespace BotVenture
{
    public partial class BotVentureForm : Form
    {
        public BotVentureForm()
        {
            InitializeComponent();
            LevelComboBox.DataSource = Enum.GetValues(typeof(Level));
            FilterComboBox.DataSource = Enum.GetValues(typeof(LobbyFilter));
            LobbiesListbox.DataSource = LobbyList;
            LobbiesListbox.DisplayMember = "Host";
        }

        public string API_KEY { get; internal set; } = string.Empty;
        public bool API_ENABLED { get; internal set; }
        public string GameId { get; private set; } = string.Empty;
        internal bool APISet { set; get; }
        private MoveResponse? moveResponse { get; set; }
        internal bool ManualControlMode { get; set; } = false;
        private string OwnHostName { get; set; } = "Prometheus";
        public DateTime StartTime { get; set; }
        public DateTime StartAt { get; set; }
        public int TakeLobbiesNum { get; set; } = 20;
        public LobbyFilter LobbyFilter { get; set; } = LobbyFilter.all;
        public Lobby[] LobbyList { get; set; } = new Lobby[0];
        public GameState GameState { get; set; }

        public bool DEBUG { get; private set; } = false;
        internal bool GameIDSet { set; get; } = false;
        internal bool GameJoined { set; get; } = false;
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
            if (GameIDSet && APISet)
            {
                // Create an instance of IO and call JoinGame
                var io = new IO(this);
                await io.JoinGame(GameId);
                GameJoinedEvent();
            }
        }
        private void GameJoinedEvent()
        {
            GameJoined = true;
            JoinButton.Enabled = false;
            GameIdTextBox.Enabled = false;
            CreateButton.Enabled = false;
            DisplayGameStats.Enabled = true;
            TakeControlButton.Enabled = true;
            // There is no funcrtion to enable it agian because the server hasnt the function to leave so the user should jsut restart the game after the host closed the lobby
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
                GameId = GameIdTextBox.Text;
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
                        //GameId = null;
                    }
                }
            }
            else
            {
                MessageBox.Show("API-Key not set", "API-Key missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await RefreshLobbyList();
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
                    await RefreshLobbyList();
                    GameStarted = true;
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
            await LoadAPIKey();
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
            RefreshLobbyList();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var io = new IO(this);
            await io.CloseGame();
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

            // Check if the game with your own hostname has already started
            bool hasOwnHostNameStarted = LobbyList?.Any(lobby => lobby.Host == OwnHostName && lobby.IsRunning) ?? false;
            GameStarted = hasOwnHostNameStarted;

            // Update UI elements based on the presence of the user's lobby
            if (GameCreated)
            {
                DisplayGameStats.Enabled = true;
                LevelComboBox.Enabled = false;
                CreateButton.Text = "Close Game";
                StartButton.Enabled = false;
                GameIdTextBox.Enabled = false;
                GameId = null;
                TakeControlButton.Enabled = true;
                if (!APISet) CreateButton.Enabled = false;
                if (!GameStarted) StartButton.Enabled = true;
                else CreateButton.Enabled = true;
            }
            else
            {
                LevelComboBox.Enabled = true;
                CreateButton.Text = "Create Game";
                StartButton.Enabled = false;
                GameIdTextBox.Enabled = true;
                TakeControlButton.Enabled = false;
                DisplayGameStats.Enabled = false;
                ButtonMoveUp.Visible = false;
                ButtonMoveUp.Enabled = false;
                ButtonMoveRight.Visible = false;
                ButtonMoveRight.Enabled = false;
                ButtonMoveDown.Visible = false;
                ButtonMoveDown.Enabled = false;
                ButtonPlayerPickUp.Visible = false;
                ButtonPlayerLook.Visible = false;
                ButtonMoveLeft.Visible = false;
                ButtonMoveLeft.Enabled = false;
                GameResponseDisplayLabel.Visible = false;
            }
        }
        private void SaveAPIKey_CheckedChanged(object sender, EventArgs e)
        {
            if (SaveAPIKey.Checked)
            {
                if (APISet)
                {
                    // Create an instance of the configuration class
                    var config = new Configuration
                    {
                        API_KEY = API_KEY
                    };

                    // Serialize the configuration to JSON
                    string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

                    // Write the JSON to a file
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                    File.WriteAllText(filePath, json);

                    if (DEBUG) MessageBox.Show("API key saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Disable the textbox
                apikeyTextBox.Enabled = false;
            }
            else
            {
                // Enable the textbox
                apikeyTextBox.Enabled = true;
            }
        }

        private async Task LoadAPIKey()
        {
            // Check if theres a theres a json file existing and when it is deserialze it and save it in the APIKey variable
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(filePath))
            {
                try
                {
                    // Read the JSON file
                    string json = File.ReadAllText(filePath);
                    // Now lets deserialize the data and save it in a new instance of config
                    var config = JsonSerializer.Deserialize<Configuration>(json);
                    if (config != null)
                    {
                        API_KEY = config.API_KEY;
                        APISet = true;
                        apikeyTextBox.Text = config.API_KEY;
                        apikeyTextBox.Enabled = false;
                        SaveAPIKey.Checked = true;
                        if (DEBUG) MessageBox.Show("API key loaded successfully.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load API key: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No saved API-key found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LobbiesListbox_DoubleClick(object sender, EventArgs e)
        {
            // when a item in the listbox is double clicked, a messagebox with all the according informations should pop up
            if (LobbiesListbox.SelectedIndex != null && LobbiesListbox.Items.Count != 0)
            {
                try
                {
                    Lobby item = (Lobby)LobbiesListbox.Items[LobbiesListbox.SelectedIndex];
                    MessageBox.Show(
                        $"Host:       {item.Host}\n" +
                        $"Level:      {item.Level}\n" +
                        $"Description:\n{item.Description}\n\n" +
                        $"GameID:     {item.GameID}\n" +
                        $"Started At: {item.StartedAt:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Is Running: {item.IsRunning}",
                        "Game Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while casting the LobbiesListBox Items to the Lobby class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void DisplayGameStats_Click(object sender, EventArgs e)
        {
            if (GameJoined || GameCreated)
            {
                var io = new IO(this);

                GameState = await io.GetCurrentGameState();

                if (GameState != null)
                {
                    MessageBox.Show(
                        $"Is Running: {GameState.isRunning}\n" +
                        $"Start At: {GameState.startAt:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Width: {GameState.width}\n" +
                        $"Height: {GameState.height}\n" +
                        $"Player Y: {GameState.playerY}\n" +
                        $"Player X: {GameState.playerX}\n" +
                        $"View Radius: {GameState.viewRadius}\n" +
                        $"Goal Position Y: {GameState.goalPositionY}\n" +
                        $"Goal Position X: {GameState.goalPositionX}",
                        "Game State Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show("Failed to retrieve game state.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }


        private void TakeControlButton_Click(object sender, EventArgs e)
        {
            ManualControlMode = !ManualControlMode;
            if (ManualControlMode)
            {
                ButtonMoveUp.Visible = true;
                ButtonMoveUp.Enabled = true;
                ButtonMoveRight.Visible = true;
                ButtonMoveRight.Enabled = true;
                ButtonMoveDown.Visible = true;
                ButtonMoveDown.Enabled = true;
                ButtonPlayerPickUp.Visible = true;
                ButtonPlayerLook.Visible = true;
                ButtonMoveLeft.Visible = true;
                ButtonMoveLeft.Enabled = true;
                LookGridTableLayoutPanel.Visible = true;
                GameResponseDisplayLabel.Visible = true;
            }
            else
            {
                ButtonMoveUp.Visible = false;
                ButtonMoveUp.Enabled = false;
                ButtonMoveRight.Visible = false;
                ButtonMoveRight.Enabled = false;
                ButtonMoveDown.Visible = false;
                LookGridTableLayoutPanel.Visible = false;
                ButtonPlayerLook.Visible = false;
                ButtonPlayerPickUp.Visible = false;
                ButtonMoveDown.Enabled = false;
                ButtonMoveLeft.Visible = false;
                ButtonMoveLeft.Enabled = false;
                GameResponseDisplayLabel.Visible = false;
            }
        }

        private async void ButtonMoveUp_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            moveResponse = await io.PlayerMove(Direction.Up);
            DisplayMoveResponse();
        }
        private async void ButtonMoveRight_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            moveResponse = await io.PlayerMove(Direction.Right);
            DisplayMoveResponse();
        }

        private async void ButtonMoveDown_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            moveResponse = await io.PlayerMove(Direction.Down);
            DisplayMoveResponse();
        }

        private async void ButtonMoveLeft_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            moveResponse = await io.PlayerMove(Direction.Left);
            DisplayMoveResponse();
        }

        private void DisplayMoveResponse()
        {
            if (moveResponse != null)
            {
                GameResponseDisplayLabel.Text = $"Game-over: {moveResponse.GameOver}\nScore: {moveResponse.Score}\nSuccess: {moveResponse.Success}\nNew Position: {moveResponse.NewPosition}";
            }
            else
            {
                GameResponseDisplayLabel.Text = "No move response available.";
            }
        }

        private void LobbiesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // the text should be the gameID of the selected lobby
            if (LobbiesListbox.SelectedItem is Lobby selectedLobby)
            {
                GameIdTextBox.Text = selectedLobby.GameID;
            }
        }

        private async void ButtonPlayerPickUp_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            PicKUpResponse response = await io.PlayerPickUp();

            if (moveResponse == null)
            {
                moveResponse = new MoveResponse(); // Ensure moveResponse is initialized
            }

            moveResponse.GameOver = response.GameOver;
            moveResponse.Score = response.Score;
            moveResponse.Success = response.Success;
            DisplayMoveResponse();
        }

        private async void ButtonPlayerLook_Click(object sender, EventArgs e)
        {
            var io = new IO(this);
            var grid = await io.PlayerLook();

            if (grid != null)
            {
                PopulateLookGrid(grid);
            }
            else
            {
                MessageBox.Show("Failed to load the grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void PopulateLookGrid(List<TileType?>[,] grid)
        {
            // Clear any existing controls in the TableLayoutPanel
            LookGridTableLayoutPanel.Controls.Clear();

            // Set the row and column counts
            LookGridTableLayoutPanel.RowCount = grid.GetLength(0);
            LookGridTableLayoutPanel.ColumnCount = grid.GetLength(1);

            // Loop through the grid to populate the TableLayoutPanel
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    // Get the list of TileTypes for this cell
                    var tileTypes = grid[row, col];

                    // Create a new Label for each cell
                    Label cellLabel = new Label
                    {
                        Text = tileTypes != null ? string.Join(", ", tileTypes.Select(t => t.ToString())) : "Empty",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = GetCellColor(tileTypes) // Set a background color based on TileTypes
                    };

                    // Add the Label to the TableLayoutPanel
                    LookGridTableLayoutPanel.Controls.Add(cellLabel, col, row);
                }
            }
        }

        private Color GetCellColor(List<TileType?> tileTypes)
        {
            if (tileTypes == null || tileTypes.Count == 0 || !tileTypes.Any(t => t.HasValue))
                return Color.LightGray;

            // Prioritize colors based on TileTypes present
            if (tileTypes.Contains(TileType.Player))
                return Color.Red;

            if (tileTypes.Contains(TileType.Enemy))
                return Color.Violet;

            if (tileTypes.Contains(TileType.Goal))
                return Color.Gold;

            if (tileTypes.Contains(TileType.Mate))
                return Color.Green;

            if (tileTypes.Contains(TileType.CanWalk))
                return Color.White;

            if (tileTypes.Contains(TileType.Block))
                return Color.Black;

            // Default color
            return Color.Gray;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
