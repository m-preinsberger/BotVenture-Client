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
        private BotVentureForm _form;

        public IO(BotVentureForm form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            Communication = new Communication(_form);
        
        }
        private bool CheckAPIKey(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            return true;
        }

        private bool CheckGameID(string gameID)
        {
            if (gameID == null)
            {
                throw new ArgumentNullException(nameof(gameID));
            }
            return true;
        }

        public async Task JoinGame(string gameId)
        {
            CheckAPIKey(_form.API_KEY);
            CheckGameID(gameId);           // Call the join method from the Communication class
            await Communication.JoinGame(_form.API_KEY, gameId);
        }
        public async Task CreateGame(Level level)
        {
            CheckAPIKey(_form.API_KEY);
            string level_str = level.ToString();
            CheckGameID(level_str);
            // Call the join method from the Communication class
            await Communication.CreateGame(_form.API_KEY, level_str);
        }
        public async Task CloseGame()
        {
            CheckAPIKey(_form.API_KEY);
            await Communication.CloseHostedGame(_form.API_KEY);
        }
        public async Task<(string Now, string StartAt)> StartGame()
        {
            CheckAPIKey(_form.API_KEY);
            return await Communication.StartGame(_form.API_KEY);
        }
        public async Task<Lobby[]> GetCurrentGames(LobbyFilter lobbyFilter, int maxLobbies)
        {
            switch (lobbyFilter)
            {
                case LobbyFilter.open:
                    return await Communication.GetCurrentGames("false", maxLobbies.ToString());
                    break;
                case LobbyFilter.running:
                    return await Communication.GetCurrentGames("true", maxLobbies.ToString());
                case LobbyFilter.all:
                    return await Communication.GetCurrentGames("null", maxLobbies.ToString());
                default:
                    throw new NotImplementedException();
            }
        }
        public async Task<GameState> GetCurrentGameState()
        {
            CheckAPIKey (_form.API_KEY);
            return await Communication.GetCurrentGameState(_form.API_KEY);
        }
        public async Task<PicKUpResponse> PlayerPickUp()
        {
            CheckAPIKey (_form.API_KEY);
            return await Communication.PlayerPickUp(_form.API_KEY);
        }
        public async Task<MoveResponse> PlayerMove(Direction direction)
        {
            CheckAPIKey (_form.API_KEY);
            switch (direction)
            {
                case Direction.Up:
                    return await Communication.PlayerMoveDirection(_form.API_KEY, (int)direction);
                    break;
                case Direction.Right:
                    return await Communication.PlayerMoveDirection(_form.API_KEY, (int)direction);
                    break;
                case Direction.Down:
                    return await Communication.PlayerMoveDirection(_form.API_KEY, (int)direction);
                    break;
                case Direction.Left:
                    return await Communication.PlayerMoveDirection(_form.API_KEY, (int)direction);
                    break;
                default:
                    throw new ArgumentNullException("The Direction Enum has an invalid value.");
                    break;
            }
        }
        public async Task<List<TileType?>[,]> PlayerLook()
        {
            CheckAPIKey(_form.API_KEY);
            return await Communication.PlayerLookAsync(_form.API_KEY);
        }
    }
}