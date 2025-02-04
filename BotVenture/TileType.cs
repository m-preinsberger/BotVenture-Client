using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotVenture
{
    [Flags]
    public enum TileType : int
    {
        Block = 0,
        CanWalk = 1,
        CanFly = 2,
        CanSwim = 4,
        Hurt = 8,
        Enemy = 32,
        Mate = 64,
        Goal = 128,
        Inventory = 256,
        Player = 512
    }
}
