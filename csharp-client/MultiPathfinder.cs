using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBlitz;

namespace Coveo
{
    /// <summary>
    /// Pathfinder that asks multiple pathfinders for the best path and takes the one
    /// that minimizes lost health.
    /// </summary>
    public class MultiPathfinder : IPathfinder
    {
        private IEnumerable<IPathfinder> pathfinders;

        public MultiPathfinder(IEnumerable<IPathfinder> pathfinders)
        {
            this.pathfinders = pathfinders ?? new List<IPathfinder>();
        }

        public PathData pathTo(Pos target, Pos currentLocation, Tile[][] board, int spikeCost = 5)
        {
            PathData pathData = null;
            foreach (IPathfinder pathfinder in pathfinders) {
                PathData newPathData = pathfinder.pathTo(target, currentLocation, board, spikeCost);
                if (pathData == null || pathData.lostHealth > newPathData.lostHealth) {
                    pathData = newPathData;
                }
            }
            return pathData;
        }
    }
}
