using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotVenture;
using static BotVenture.Form1;

namespace BotVenture
{
    internal class Communication
    {
        private readonly Form1 _form;

        public Communication(Form1 form)
        {
            _form = form;
        }

        private HttpClient Client { get; } = new HttpClient { BaseAddress = new Uri("https://botventure.htl-neufelden.at") };
        public async Task CloseHostedGame(string ApiKey)
        {
            // Create the URL by appending the API key to the base URL
            string requestUrl = $"/api/game/{ApiKey}/close";

            try
            {
                HttpResponseMessage response = await Client.PostAsync(requestUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully closed the hosted game.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to close the hosted game. Status code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task JoinGame(string ApiKey, string gameId)
        {
            // Create the URL by appending the API key and game ID to the base URL
            string requestUrl = $"/api/game/{ApiKey}/join/{gameId}";

            // Create the content to send in the POST request (even if empty, it should be initialized properly)
            var content = new StringContent(JsonSerializer.Serialize(new { gameId = gameId }), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully joined the game.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to join the game. Status code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public async Task CreateGame(string ApiKey, string level)
        {
            // Create the URL by appending the API key and game ID to the base URL
            string requestUrl = $"/api/game/{ApiKey}/create/{level}";
            var content = new StringContent(JsonSerializer.Serialize(new { level = level }), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.PostAsync(requestUrl, content);
                if (_form.DEBUG)
                {
                    string tempResponseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Raw Response: {tempResponseBody}");
                    var contentType = response.Content.Headers.ContentType?.MediaType;
                    MessageBox.Show($"Content-Type: {contentType}");
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        string id = root.GetProperty("id").GetString();
                        MessageBox.Show($"Game created successfully. Game ID: {id}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _form.SetGameID(id);
                        _form.GameIDSet = true;
                        _form.GameCreated = true;
                        // i should remove all of this, communication class should not have to know the form                                                    
                    }
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        string error = root.GetProperty("error").GetString();
                        string message = root.GetProperty("message").GetString();
                        MessageBox.Show($"Failed to create the game. Error: {error}, Message: {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task<(string Now, string StartAt)> StartGame(string ApiKey)
        {
            // Create the URL by appending the API key to the base URL
            string requestUrl = $"/api/game/{ApiKey}/start";

            try
            {
                HttpResponseMessage response = await Client.PostAsync(requestUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        string now = root.GetProperty("now").GetString();
                        string startAt = root.GetProperty("startAt").GetString();

                        return (now, startAt); // Return the parsed values
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to start the game. Status code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            }
        }
        public async Task<GameState> GetCurrentGameState(string ApiKey)
        {
            if (string.IsNullOrEmpty(ApiKey)) throw new ArgumentNullException("API-Key cannot be null.");
            string requestUrl = $"/api/game/{ApiKey}/state";
            try
            {
                HttpResponseMessage response = await Client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var GameState = JsonSerializer.Deserialize<GameState>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return GameState;
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"Error deserializing response: {jsonEx.Message}", "Deserialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                else
                {
                    string errorMessage = _form.DEBUG ? $"Failed to get current game state. \nRequest URL: {requestUrl}\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}"
                        : $"Failed to get current game state.\n{response.StatusCode}";
                    MessageBox.Show(errorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = _form.DEBUG
                ? $"An error occurred:\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}\nRequest URL: {requestUrl}"
                : $"An error occurred: {ex.Message}";

                MessageBox.Show(exceptionMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public async Task<Lobby[]> GetCurrentGames(string running, string take)
        {
            string requestUrl = $"/api/game/list/{running}/{take}";
            if (running == "null") requestUrl = $"/api/game/list/";

            try
            {
                HttpResponseMessage response = await Client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var lobbies = JsonSerializer.Deserialize<Lobby[]>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return lobbies ?? Array.Empty<Lobby>();
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"Error deserializing response: {jsonEx.Message}", "Deserialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return Array.Empty<Lobby>();
                    }
                }
                else
                {
                    string errorMessage = _form.DEBUG
                        ? $"Failed to get current games.\nRequest URL: {requestUrl}\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}"
                        : $"Failed to get current games. Status code: {response.StatusCode}";

                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Array.Empty<Lobby>();
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = _form.DEBUG
                    ? $"An error occurred:\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}\nRequest URL: {requestUrl}"
                    : $"An error occurred: {ex.Message}";

                MessageBox.Show(exceptionMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Array.Empty<Lobby>();
            }
        }


    }
}
