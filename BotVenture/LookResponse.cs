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
        public List<List<Info>> Infos { get; set; }
        public class Info
        {
            public int Type { get; set; }
        }
    }
}
