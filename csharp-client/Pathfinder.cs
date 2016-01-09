using System;
using Coveo;
using CoveoBlitz;

namespace Coveo
{
    public interface IPathfinder
    {
        PathData pathTo(Pos target, Pos currentLocation, GameState gameState, int spikeCost = 5);
    }
}
