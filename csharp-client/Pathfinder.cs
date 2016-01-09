using System;
using Coveo;
using CoveoBlitz;

namespace Coveo
{
    public interface IPathfinder
    {
        public PathData pathTo(Pos target, Pos currentLocation, Tile[][] board, int spikeCost = 5);
    }
}
