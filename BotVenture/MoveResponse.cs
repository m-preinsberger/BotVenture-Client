﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVenture
{
    public class MoveResponse
    {
        public bool GameOver { get; set; }
        public int Score { get; set; }
        public bool Success { get; set; }
        public Position NewPosition { get; set; }
    }
}
