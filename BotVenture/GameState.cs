using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVenture
{
    public class GameState
    {
        public bool isRunning {  get; set; }
        public DateTime? startAt { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public int? playerY { get; set; }
        public int? playerX { get; set; }
        public int? viewRadius { get; set; }
        public int? goalPositionY { get; set; }
        public int? goalPositionX { get; set; }
    }
}
