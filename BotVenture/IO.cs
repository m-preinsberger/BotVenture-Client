using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotVenture;

namespace BotVenture
{
    internal class IO
    {
        private Communication Communication;
        private Form1 _form;

        public IO(Form1 form)
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
                    return await Communication.GetCurrentGames("Null", maxLobbies.ToString());
                default:
                    throw new NotImplementedException();
            }
        }
    }
}