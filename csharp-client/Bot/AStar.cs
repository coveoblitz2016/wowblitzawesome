using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            Dictionary<string, InternalTile> Navigated = new Dictionary<string, InternalTile>();
            HashSet<InternalTile> Closed = new HashSet<InternalTile>();

            InternalTile beginning = new InternalTile() { TilePos = currentLocation, Weight = 0 };
            HashSet<InternalTile> Opened = new HashSet<InternalTile> {
                beginning
            };

            Dictionary<string, int> Scores = new Dictionary<string, int> {
                {GetKey(beginning.TilePos.x, beginning.TilePos.y), beginning.Weight}
            };


            Dictionary<string, float> FullScores = new Dictionary<string, float> {
                {GetKey(beginning.TilePos.x, beginning.TilePos.y), GetDistance(currentLocation, target)}
            };

            while (Opened.Any()) {
                InternalTile lowest = Opened.First(tile => GetKey(tile.TilePos.x, tile.TilePos.y) == GetLowestCostTile(FullScores, Opened));

                if (lowest.TilePos.x == target.x && lowest.TilePos.y == target.y) {
                    return ReconstructPath(Navigated, target);
                }

                Opened.Remove(lowest);
                Closed.Add(lowest);

                foreach (Pos neighbor in GetNeighbors(lowest.TilePos, board.Length, board[0].Length)) {
                    if (Closed.Any(tile => tile.TilePos.x == neighbor.x && tile.TilePos.y == neighbor.y)) {
                        continue;
                    }

                    string neighborKey = GetKey(neighbor.x, neighbor.y);
                    int curScore = Scores[GetKey(lowest.TilePos.x, lowest.TilePos.y)] + 1;
                    if (!Opened.Any(tile => tile.TilePos.x == neighbor.x && tile.TilePos.y == neighbor.y)) {
                        Opened.Add(new InternalTile { TilePos = new Pos { x = neighbor.x, y = neighbor.y } });
                    } else if (curScore >= (Scores.ContainsKey(neighborKey) ? Scores[neighborKey] : int.MaxValue)) {
                        continue;
                    }

                    Navigated.Add(neighborKey, lowest);
                    Scores.Add(neighborKey, curScore);
                    FullScores.Add(neighborKey, curScore + GetDistance(neighbor, target));
                }
            }

            return null;
        }

        public float GetDistance(Pos a,
            Pos b)
        {
            return (float) Math.Sqrt(Math.Pow(Math.Abs(b.x - a.x), 2) + Math.Pow(Math.Abs(b.y - a.y), 2));
        }

         private string GetKey(int x, int y)
            {
                return String.Format("{0}-{1}", x, y);
            }

        private string GetLowestCostTile(Dictionary<string, float> Tiles, HashSet<InternalTile> openedTiles)
        {
            string result = null;
            float lowest = float.MaxValue;

            foreach (KeyValuePair<string, float> internalTile in Tiles) {
                if (openedTiles.Any(tile => GetKey(tile.TilePos.x, tile.TilePos.y) == internalTile.Key) && internalTile.Value <= lowest) {
                    result = internalTile.Key;
                    lowest = internalTile.Value;
                }
            }

            return result;
        }

        private PathData ReconstructPath(Dictionary<string, InternalTile> navigated,
            Pos current)
        {
            Stack<Pos> totalPath = new Stack<Pos>();
            totalPath.Push(current);

            while (navigated.ContainsKey(GetKey(current.x, current.y))) {
                current = navigated[GetKey(current.x, current.y)].TilePos;
                totalPath.Push(current);
            }

            return new PathData { distance = navigated.Count, lostHealth = 0, nextDirection = GetDirectionOfPos(totalPath.Pop(), totalPath.Pop()) };
        }

        private string GetDirectionOfPos(Pos a,
            Pos b)
        {
            if (a.x > b.x) {
                return Direction.North;
            }

            if (a.x < b.x) {
                return Direction.South;
            }

            if (a.y > b.y) {
                return Direction.West;
            }

            if(a.y < b.y)
                return Direction.East;

            return Direction.Stay;
        }

        private List<Pos> GetNeighbors(Pos current, int maxX, int maxY)
        {
            List<Pos> lst = new List<Pos>();

            if (current.x + 1 < maxX)
                lst.Add(new Pos { x = current.x + 1, y = current.y });

            if (current.x - 1 >= 0) {
                lst.Add(new Pos { x = current.x - 1, y = current.y });
            }

            if (current.y + 1 < maxY) {
                lst.Add(new Pos { x = current.x, y = current.y + 1 });
            }

            if(current.y - 1 >= 0)
                lst.Add(new Pos { x = current.x, y = current.y - 1 });

            return lst;
        }
    }
}
