using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BotVenture;

namespace BotVenture
{
    internal class IO
    {
        private Communication Communication;
        private Game _game;

        public IO(Game game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            Communication = new Communication();
        }

        private bool CheckAPIKey()
        {
            if (string.IsNullOrEmpty(_game.API_KEY))
            {
                throw new ArgumentNullException("API_KEY cannot be null or empty.");
            }
            return true;
        }

        private bool CheckGameID(string gameID)
        {
            if (string.IsNullOrEmpty(gameID))
            {
                throw new ArgumentNullException(nameof(gameID), "GameID cannot be null or empty.");
            }
            return true;
        }

        public async Task JoinGame(string gameId)
        {
            CheckAPIKey();
            CheckGameID(gameId);
            await Communication.JoinGame(_game.API_KEY, gameId);
        }

        public async Task<string> CreateGame(Level level)
        {
            CheckAPIKey();
            string levelStr = level.ToString();
            // Assuming CreateGame requires a valid Level string, not a GameID
            return await Communication.CreateGame(_game.API_KEY, levelStr);
        }

        public async Task CloseGame()
        {
            CheckAPIKey();
            await Communication.CloseHostedGame(_game.API_KEY);
        }

        public async Task<(string Now, string StartAt)> StartGame()
        {
            CheckAPIKey();
            return await Communication.StartGame(_game.API_KEY);
        }

        public async Task<Lobby[]> GetCurrentGames(LobbyFilter lobbyFilter, int maxLobbies)
        {
            CheckAPIKey();

            switch (lobbyFilter)
            {
                case LobbyFilter.open:
                    return await Communication.GetCurrentGames("false", maxLobbies.ToString());
                case LobbyFilter.running:
                    return await Communication.GetCurrentGames("true", maxLobbies.ToString());
                case LobbyFilter.all:
                    return await Communication.GetCurrentGames("null", maxLobbies.ToString());
                default:
                    throw new NotImplementedException($"LobbyFilter '{lobbyFilter}' is not implemented.");
            }
        }

        public async Task<GameState> GetCurrentGameState()
        {
            CheckAPIKey();
            return await Communication.GetCurrentGameState(_game.API_KEY);
        }

        public async Task<PicKUpResponse> PlayerPickUp()
        {
            CheckAPIKey();
            return await Communication.PlayerPickUp(_game.API_KEY);
        }

        public async Task<MoveResponse> PlayerMove(Direction direction)
        {
            CheckAPIKey();
            return await Communication.PlayerMoveDirection(_game.API_KEY, (int)direction);
        }

        public async Task<List<TileType?>[,]> PlayerLook()
        {
            CheckAPIKey();
            return await Communication.PlayerLookAsync(_game.API_KEY);
        }
    }
}