using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVenture
{
    public class Lobby
    {
        public string Level { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }
        public int AmountOfBots { get; set; }
        public bool IsRunning { get; set; }
        public DateTime? StartedAt { get; set; }
        public string GameID { get; set; }
    }
}
