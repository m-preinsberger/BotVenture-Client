# BotVenture

# Software-Structure

- Class brain
- Class IO
- Class Communication
    - base
    - talks via json to the api
    - needs to implement all the

<aside>
💡

One class one 

</aside>

<aside>
💡

JSON: JavaScript-Object-Notation. Its always { “poperty” : “value”} 

</aside>

<aside>
💡

Functions in the RestAPI get called by HTTP … URL + Verb

</aside>

<aside>
💡

**`2iOTBDstEkqUygbh33w39w`**

</aside>

<aside>
💡

Only one game can exist at a time.

</aside>

Here are the most important **HTTP status codes** grouped by their categories. These are essential to understand for web development, API integration, and debugging.

---

### **1xx - Informational Responses**

- **100 - Continue**: The server has received the request headers and the client should proceed to send the request body.
- **101 - Switching Protocols**: The server is switching protocols as requested by the client (e.g., from HTTP to WebSocket).

---

### **2xx - Success**

- **200 - OK**: The request was successful, and the server returned the requested resource.
- **201 - Created**: The request succeeded, and a new resource was created (often seen after POST requests).
- **204 - No Content**: The request succeeded, but there is no content to send in the response (often used for DELETE requests).

---

### **3xx - Redirection**

- **301 - Moved Permanently**: The resource has been moved to a new URL permanently (used in SEO and URL changes).
- **302 - Found (Temporary Redirect)**: The resource has been temporarily moved to a different URL.
- **304 - Not Modified**: The resource has not changed since the last request (used in caching to avoid re-downloading resources).
- **307 - Temporary Redirect**: Similar to **302**, but more strict as it doesn't allow method changes (like POST to GET).
- **308 - Permanent Redirect**: Like **301**, but more strict and ensures the same request method is used.

---

### **4xx - Client Errors**

- **400 - Bad Request**: The server could not understand the request due to malformed syntax or invalid data.
- **401 - Unauthorized**: Authentication is required and has either not been provided or is incorrect.
- **403 - Forbidden**: The server understands the request but refuses to authorize it (often permission issues).
- **404 - Not Found**: The requested resource was not found on the server.
- **405 - Method Not Allowed**: The HTTP method used is not allowed for the requested resource (e.g., POST instead of GET).
- **408 - Request Timeout**: The server timed out waiting for the client to send a request.
- **429 - Too Many Requests**: The user has sent too many requests in a given amount of time (rate-limiting mechanism).

---

### **5xx - Server Errors**

- **500 - Internal Server Error**: A generic error when the server encounters an unexpected condition.
- **501 - Not Implemented**: The server does not recognize the request method or it lacks the ability to fulfil it.
- **502 - Bad Gateway**: The server received an invalid response from an upstream server (often when acting as a proxy).
- **503 - Service Unavailable**: The server is overloaded or down for maintenance.
- **504 - Gateway Timeout**: The server, while acting as a proxy, did not receive a timely response from the upstream server.

---

```csharp
        public async Task StartGame(string ApiKey)
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
                        MessageBox.Show($"Game started successfully.\nNow: {now}\nStart At: {startAt}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Failed to start the game. Status code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
```

## GET Requests and how you deserialize multiple objects

Lets take the GetCurrentGames() as an example. The result of this request is an array of objects. I created for this a extra class in an extra file called Level.cs. This class contains all the properties the JSON object has (this is a must, otherwise it wont work). 

```csharp
 var lobbies = JsonSerializer.Deserialize<Lobby[]>(responseContent, new JsonSerializerOptions
 {
     PropertyNameCaseInsensitive = true
 });
```

The `PropertyNameCaseInsensitive = true`  configures the desirializer so that ignores the case-sensitivity. Why do we need this, because JSON and C# have different naming conventions.

<aside>
💡

Lessons learned: Code looks more and more like hieroglyphs and coding feels more like alchemistry now.

</aside>

### GET Request of the StartedAt property

I had an error which I couldn't figure out for a long time. The JSOn-Deserialzer tried to convert from the string into a DateTime Format. Why did occcured the error here? After half an hour I found out that DateTime is a not nullable data type. So it throw an aggressive error at me every time I tried to read this value.

![This was the error.](image1.png)

This was the error.

<aside>
💡

Lessons learned: DateTime is a notNullable data type and notNullable data types don't like to be null.

</aside>

## TODO

1. When clicking on the lobbies a pop should be displayed showing the status of the lobby (all properties etc) ✅
2. Implement a way to save the API-Key ✅

## Save and Load the API-Key

My first intention was to save the key in a JSON file but I thought that would not be secure. And then I thought who cares? (and I don´t know how to make it secure. )

```csharp
string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
```

<aside>
💡

Today I learned that you should put try-catch blocks everywhere in your code with detailed output and error messages. This makes bug fixing so much easier. Especially in win forms with the handy message boxes.

</aside>

## Info Pop-up for the lobbies

![Started At is empty when its null. Maybe I could implement something that writes not yet started later on.](image%201.png)

Started At is empty when its null. Maybe I could implement something that writes not yet started later on.

I`ve implemented all the basic function for the game now except one function that gets the state of the game.

---

January 8, 2025 

## Show game stats

I’ve implemented a button that shows the game stats in a message-box when clicked. 

![image.png](image%202.png)

## Manual Moving

Now I can create, join, start, … games but I want to move. Implementing the whole bot algorithm was too time intensive so I thought about implementing a manual move system. (I even made an custom button).

![I made a round button. wow :O](image%203.png)

I made a round button. wow :O

I have to admit the AI helped me with the round button. The control itself is easy, the buttons are only shown when the round button is clicked. This button is only enabled when a game is started or a game has been joined. And when the game is closed it gets disabled and the other buttons invisible again. 

```csharp
    public class RoundButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Create a circular path
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(path);

            // Call base to handle other button properties
            base.OnPaint(pevent);
        }
    }
```

---

## TODO for January 15, 2025 :

- [x]  Implement a coordinate display
- [ ]  implement all the player functions
- [ ]  cut dependencies between communication und form, instead load it to IO class
- [ ]  Make a own setting class
- [x]  implement the pickup function
- [ ]  implement the look function
- [ ]  maybe even begin with the bot itself
- [ ]  Mention Github in the documentation
- [x]  Join System for the Lobby List
- [x]  Game Response Data Shown

---

January 15, 2025 

## Join System for the

![image.png](image%204.png)

When a lobby is selected the according GameID is set to the GameIDTextBox. When the join button is pressed, the normal join procedure is activated.

## Pickup function

I’ve added a button for Look and for PickUp right beneath the control buttons.

![image.png](image%205.png)

---

Now that the Pick-Up function is implemented. I’ve took on the look function, but here it gets interesting. I opened up the Swagger UI.

![Screenshot from the Swager UI](image%206.png)

Screenshot from the Swager UI

But I can’t quite make sense of it. Then I opened up Postmen and got this response.

```json
{
    "width": 5,
    "height": 5,
    "infos": [
        [{ "type": 0 }, { "type": 0 }, { "type": 0 }, { "type": 0 }, { "type": 0 }],
        [{ "type": 3 }, { "type": 3 }, { "type": 3 }, { "type": 3 }, null],
        [{ "type": 3 }, { "type": 3 }, { "type": 512 }, { "type": 2 }, null],
        [{ "type": 3 }, { "type": 3 }, { "type": 3 }, null, null],
        [{ "type": 3 }, { "type": 3 }, { "type": 3 }, { "type": 3 }, null]
    ],
    "gameOver": false,
    "score": 971
}
```

I wondered what the `null` value is. I knew the schemata of the enum bit table. I tried to analyse the 

---

January 29, 2025 

---

February 4, 2025 

I’ve found a bug in the API. Always the same 2 fields are null. Doesn't matter which level or where.

![image.png](image%207.png)

Postman confirms this.

```json
{
  "width": 5,
  "height": 5,
  "score": 773,
  "gameOver": false,
  "field": [
    [3, 3, 3, 3, 3],
    [3, 3, 3, 3, 3],
    [3, 3, 512, 3, 3],
    [3, 3, 3, null, 3],
    [3, 3, 3, 3, null]
  ]
}
```
