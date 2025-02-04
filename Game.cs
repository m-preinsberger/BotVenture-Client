namespace BotVenture
{
    public class Game
    {
        public string API_KEY { get; set; } = string.Empty;
        public bool API_ENABLED { get; set; }
        public string GameId { get; set; } = string.Empty;
        public bool APISet { get; set; }
        public MoveResponse? MoveResponse { get; set; }
        public bool ManualControlMode { get; set; } = false;
        public string OwnHostName { get; set; } = "Prometheus";
        public DateTime StartTime { get; set; }
        public DateTime StartAt { get; set; }
        public int TakeLobbiesNum { get; set; } = 20;
        public LobbyFilter LobbyFilter { get; set; } = LobbyFilter.all;
        public Lobby[] LobbyList { get; set; } = new Lobby[0];
        public GameState GameState { get; set; }
        public bool DEBUG { get; set; } = false;
        public bool GameIDSet { get; set; } = false;
        public bool GameJoined { get; set; } = false;
        public bool GameStarted { get; set; } = false;
        public bool GameCreated { get; set; } = false;
        public Level ChoosenLevel { get; set; } = Level.level0;
    }
}
