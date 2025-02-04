using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVenture
{
    public class LookResponse
    {
        public bool GameOver { get; set; }
        public int Score { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<List<Tile?>> Infos { get; set; } // Use a list of lists to match the JSON structure.
    }
}
