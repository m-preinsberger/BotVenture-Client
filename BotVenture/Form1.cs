using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Forms;
namespace BotVenture
{
    public partial class BotVentureForm : Form
    {
        private Game game;

        public BotVentureForm()
        {
            InitializeComponent();
            game = new Game();
            LevelComboBox.DataSource = Enum.GetValues(typeof(Level));
            FilterComboBox.DataSource = Enum.GetValues(typeof(LobbyFilter));
            LobbiesListbox.DataSource = game.LobbyList;
            LobbiesListbox.DisplayMember = "Host";
        }

        private void SetGameID(string newID)
        {
            if (!string.IsNullOrEmpty(newID))
            {
                game.GameId = newID;
                GameIDLabel.Text = game.GameId;
            }
        }

        private async void commitButton_Click(object sender, EventArgs e)
        {
            if (game.GameIDSet && game.APISet)
            {
                var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
                await io.JoinGame(game.GameId);
                GameJoinedEvent();
            }
        }

        private void GameJoinedEvent()
        {
            game.GameJoined = true;
            JoinButton.Enabled = false;
            GameIdTextBox.Enabled = false;
            CreateButton.Enabled = false;
            DisplayGameStats.Enabled = true;
            TakeControlButton.Enabled = true;
        }

        private void GameIdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GameIdTextBox.Text) || string.IsNullOrEmpty(game.API_KEY))
            {
                game.GameIDSet = false;
                JoinButton.Enabled = false;
            }
            else
            {
                game.GameId = GameIdTextBox.Text;
                game.GameIDSet = true;
                JoinButton.Enabled = true;
            }
        }

        private void apikeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(apikeyTextBox.Text))
            {
                game.API_KEY = apikeyTextBox.Text;
                game.APISet = true;
            }
            else
            {
                game.APISet = false;
            }
        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {
            if (game.APISet)
            {
                var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'

                if (game.GameCreated)
                {
                    await io.CloseGame();
                    game.GameCreated = false;
                    CreateButton.Text = "Create Game";
                    StartButton.Enabled = false;
                    LevelComboBox.Enabled = true;
                }
                else
                {
                    game.GameId = await io.CreateGame(game.ChoosenLevel);
                    game.GameIDSet = true;
                    game.GameCreated = true;
                    if (game.GameCreated)
                    {
                        LevelComboBox.Enabled = false;
                        CreateButton.Text = "Close Game";
                        StartButton.Enabled = true;
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
            game.ChoosenLevel = (Level)LevelComboBox.SelectedItem;
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (game.GameCreated)
            {
                var io = new IO(game);
                var result = await io.StartGame();

                if (result.Now != null && result.StartAt != null)
                {
                    game.StartTime = DateTime.Parse(result.Now);
                    game.StartAt = DateTime.Parse(result.StartAt);

                    MessageBox.Show(
                        $"Game started successfully.\nNow: {game.StartTime}\nStart At: {game.StartAt}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await RefreshLobbyList();
                    game.GameStarted = true;

                    // Wait 5.5 seconds before starting the bot.
                    await Task.Delay(TimeSpan.FromSeconds(5.5));

                    // Start the bot asynchronously.
                    Brain bot = new Brain(new IO(game));
                    _ = Task.Run(async () => await bot.Run());
                }
                else
                {
                    MessageBox.Show("Failed to start the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadAPIKey();
            await RefreshLobbyList();
        }

        private async void TakerMoreLobbies_Click(object sender, EventArgs e)
        {
            game.TakeLobbiesNum += 20;
            await RefreshLobbyList();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            await io.CloseGame();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'

            switch (game.LobbyFilter)
            {
                case LobbyFilter.all:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.all, game.TakeLobbiesNum);
                    break;
                case LobbyFilter.open:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.open, game.TakeLobbiesNum);
                    break;
                case LobbyFilter.running:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.running, game.TakeLobbiesNum);
                    break;
            }

            LobbiesListbox.DataSource = null;
            LobbiesListbox.DataSource = game.LobbyList;
            LobbiesListbox.DisplayMember = "Host";

            bool hasOwnHostName = game.LobbyList?.Any(lobby => lobby.Host == game.OwnHostName) ?? false;
            game.GameCreated = hasOwnHostName;

            if (game.GameCreated)
            {
                LevelComboBox.Enabled = false;
                CreateButton.Text = "Close Game";
                StartButton.Enabled = false;
                game.GameId = null;
                CreateButton.Enabled = game.APISet;
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
            game.LobbyFilter = (LobbyFilter)FilterComboBox.SelectedItem;
            await RefreshLobbyList();
        }

        private async Task RefreshLobbyList()
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'

            switch (game.LobbyFilter)
            {
                case LobbyFilter.all:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.all, game.TakeLobbiesNum);
                    break;
                case LobbyFilter.open:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.open, game.TakeLobbiesNum);
                    break;
                case LobbyFilter.running:
                    game.LobbyList = await io.GetCurrentGames(LobbyFilter.running, game.TakeLobbiesNum);
                    break;
            }

            LobbiesListbox.DataSource = null;
            LobbiesListbox.DataSource = game.LobbyList;
            LobbiesListbox.DisplayMember = "Host";

            bool hasOwnHostName = game.LobbyList?.Any(lobby => lobby.Host == game.OwnHostName) ?? false;
            game.GameCreated = hasOwnHostName;

            bool hasOwnHostNameStarted = game.LobbyList?.Any(lobby => lobby.Host == game.OwnHostName && lobby.IsRunning) ?? false;
            game.GameStarted = hasOwnHostNameStarted;

            if (game.GameCreated)
            {
                DisplayGameStats.Enabled = true;
                LevelComboBox.Enabled = false;
                CreateButton.Text = "Close Game";
                StartButton.Enabled = false;
                GameIdTextBox.Enabled = false;
                game.GameId = null;
                TakeControlButton.Enabled = true;
                if (!game.APISet) CreateButton.Enabled = false;
                if (!game.GameStarted) StartButton.Enabled = true;
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
                if (game.APISet)
                {
                    var config = new Configuration
                    {
                        API_KEY = game.API_KEY
                    };

                    string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                    File.WriteAllText(filePath, json);

                    if (game.DEBUG) MessageBox.Show("API key saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                apikeyTextBox.Enabled = false;
            }
            else
            {
                apikeyTextBox.Enabled = true;
            }
        }

        private async Task LoadAPIKey()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    var config = JsonSerializer.Deserialize<Configuration>(json);
                    if (config != null)
                    {
                        game.API_KEY = config.API_KEY;
                        game.APISet = true;
                        apikeyTextBox.Text = config.API_KEY;
                        apikeyTextBox.Enabled = false;
                        SaveAPIKey.Checked = true;
                        if (game.DEBUG) MessageBox.Show("API key loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (LobbiesListbox.SelectedIndex != -1 && LobbiesListbox.Items.Count != 0)
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
            if (game.GameJoined || game.GameCreated)
            {
                var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
                game.GameState = await io.GetCurrentGameState();

                if (game.GameState != null)
                {
                    MessageBox.Show(
                        $"Is Running: {game.GameState.isRunning}\n" +
                        $"Start At: {game.GameState.startAt:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Width: {game.GameState.width}\n" +
                        $"Height: {game.GameState.height}\n" +
                        $"Player Y: {game.GameState.playerY}\n" +
                        $"Player X: {game.GameState.playerX}\n" +
                        $"View Radius: {game.GameState.viewRadius}\n" +
                        $"Goal Position Y: {game.GameState.goalPositionY}\n" +
                        $"Goal Position X: {game.GameState.goalPositionX}",
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

        private void TakeControlButton_Click(object sender, EventArgs e)
        {
            game.ManualControlMode = !game.ManualControlMode;
            if (game.ManualControlMode)
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
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            game.MoveResponse = await io.PlayerMove(Direction.Up);
            DisplayMoveResponse();
        }

        private async void ButtonMoveRight_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            game.MoveResponse = await io.PlayerMove(Direction.Right);
            DisplayMoveResponse();
        }

        private async void ButtonMoveDown_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            game.MoveResponse = await io.PlayerMove(Direction.Down);
            DisplayMoveResponse();
        }

        private async void ButtonMoveLeft_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            game.MoveResponse = await io.PlayerMove(Direction.Left);
            DisplayMoveResponse();
        }

        private void DisplayMoveResponse()
        {
            if (game.MoveResponse != null)
            {
                GameResponseDisplayLabel.Text = $"Game-over: {game.MoveResponse.GameOver}\nScore: {game.MoveResponse.Score}\nSuccess: {game.MoveResponse.Success}\nNew Position: {game.MoveResponse.NewPosition}";
            }
            else
            {
                GameResponseDisplayLabel.Text = "No move response available.";
            }
        }

        private void LobbiesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LobbiesListbox.SelectedItem is Lobby selectedLobby)
            {
                GameIdTextBox.Text = selectedLobby.GameID;
            }
        }

        private async void ButtonPlayerPickUp_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
            PicKUpResponse response = await io.PlayerPickUp();

            if (game.MoveResponse == null)
            {
                game.MoveResponse = new MoveResponse();
            }

            game.MoveResponse.GameOver = response.GameOver;
            game.MoveResponse.Score = response.Score;
            game.MoveResponse.Success = response.Success;
            DisplayMoveResponse();
        }

        private async void ButtonPlayerLook_Click(object sender, EventArgs e)
        {
            var io = new IO(game); // Updated from 'new IO(this)' to 'new IO(game)'
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

        private void PopulateLookGrid(List<TileType?>[,] grid)
        {
            LookGridTableLayoutPanel.Controls.Clear();
            LookGridTableLayoutPanel.RowCount = grid.GetLength(0);
            LookGridTableLayoutPanel.ColumnCount = grid.GetLength(1);

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    var tileTypes = grid[row, col];
                    Label cellLabel = new Label
                    {
                        Text = tileTypes != null ? string.Join(", ", tileTypes.Select(t => t.ToString())) : "Empty",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = GetCellColor(tileTypes)
                    };
                    LookGridTableLayoutPanel.Controls.Add(cellLabel, col, row);
                }
            }
        }

        private Color GetCellColor(List<TileType?> tileTypes)
        {
            if (tileTypes == null || tileTypes.Count == 0 || !tileTypes.Any(t => t.HasValue))
                return Color.LightGray;

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

            return Color.Gray;
        }
    }
}
