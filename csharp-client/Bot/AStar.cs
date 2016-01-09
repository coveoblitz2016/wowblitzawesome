using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBlitz;

namespace Coveo.Bot
{
    public class AStar: IPathfinder
    {
        public class InternalTile
        {
            public Pos TilePos;
            public int Weight;
        }

        public PathData pathTo(Pos target, Pos currentLocation, Tile[][] board, int spikeCost = 5)
        {
            HashSet<Pos>
        }
    }
}
