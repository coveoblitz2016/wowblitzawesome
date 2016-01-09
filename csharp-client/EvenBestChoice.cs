using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBlitz;

namespace Coveo
{
    /// <summary>
    /// Best choice provider that moves to mines if energy is above 50%, otherwise tavern.
    /// </summary>
    public class EvenBestChoice : IBestChoice
    {
        public const int SPIKE_COST = 10;
        public const int MINE_COST = 25;
        public const int TAVERN_COST = 0;

        private sealed class TargetInfo
        {
            public Pos pos;
            public int distance;
            public Tile tile;

            public TargetInfo(Pos pos, int distance, Tile tile)
            {
                Debug.Assert(pos != null);

                this.pos = pos;
                this.distance = distance;
                this.tile = tile;
            }
        }

        private int? heroLife;
        private Pos heroPos;
        private TargetInfo target;
        private PathData lastPathData;

        public string BestMove(GameState gameState, IPathfinder pathfinder)
        {
            String murderDirection = murderAlgo(gameState);
            if (!string.IsNullOrEmpty(murderDirection)) {
                Console.Out.WriteLine("WARWARWAR");
                return murderDirection;
            }
            // If we suddenly lost a lot of life, maybe we should reconsider.
            if (heroLife.HasValue && heroLife.Value >= (gameState.myHero.life + 20)) {
                Console.WriteLine("EvenBestChoice: LOW ON LIFE! Maybe we were attacked?");
                target = null;
            }
            heroLife = gameState.myHero.life;

            // If we suddenly moved a lot, maybe we should reconsider.
            if (heroPos != null && Pos.DistanceBetween(heroPos, gameState.myHero.pos) >= 2) {
                Console.WriteLine("EvenBestChoice: TELEPORTED! Maybe we were killed?");
                target = null;
            }
            heroPos = gameState.myHero.pos;

            PathData pathData = null;
            if (target != null) {
                Console.WriteLine("EvenBestChoice: Current target: ({0},{1}) [tile {2}]", target.pos.x, target.pos.y, target.tile);
                pathData = pathfinder.pathTo(target.pos, gameState.myHero.pos, gameState, SPIKE_COST);
            } else {
                // Seek mine if possible, otherwise seek a tavern
                if (gameState.myHero.life >= 35) {
                    pathData = SeekMine(gameState, pathfinder);
                }
                if (pathData == null || pathData.lostHealth >= gameState.myHero.life) {
                    pathData = SeekTavern(gameState, pathfinder);
                }
            }

            // If this is the last move, release target unless it's a tavern and we're < 90 life
            if (target != null && pathData != null && pathData.nextDirection != Direction.Stay &&
                Pos.DistanceBetween(target.pos, gameState.myHero.pos) <= 1 && (target.tile != Tile.TAVERN || gameState.myHero.life >= 90)) {

                Console.WriteLine("EvenBestChoice: Reached destination ({0},{1}) [{2}], releasing target",
                    target.pos.x, target.pos.y, target.tile);
                target = null;
            }
            lastPathData = pathData;
            string nextDirection = pathData != null ? pathData.nextDirection : null;
            return !String.IsNullOrEmpty(nextDirection) ? nextDirection : Direction.Stay;
        }

        private String murderAlgo(GameState gameState)
        {
            int size = gameState.board.Length -1 ;
            Pos myPos = gameState.myHero.pos;
            foreach(Hero hero in gameState.heroes) {

                if (hero.pos.x < size && gameState.board[hero.pos.x + 1][hero.pos.y] == Tile.TAVERN) {
                    continue;
                }
                if (hero.pos.y < size && gameState.board[hero.pos.x][hero.pos.y + 1] == Tile.TAVERN) {
                    continue;
                }
                if (hero.pos.x > 0 && gameState.board[hero.pos.x - 1][hero.pos.y] == Tile.TAVERN) {
                    continue;
                }
                if (hero.pos.y > 0 && gameState.board[hero.pos.x][hero.pos.y - 1] == Tile.TAVERN) {
                    continue;
                }
                if (Pos.DistanceBetween(hero.pos, hero.spawnPos) == 0) {
                    continue;
                }
                if (hero.life < gameState.myHero.life) {
                    if (Math.Abs(myPos.x - hero.pos.x) + Math.Abs(myPos.y - hero.pos.y) == 2) {
                        if (myPos.x > hero.pos.x) {
                            return Direction.North;
                        } else if (myPos.x < hero.pos.x) {
                            return Direction.South;
                        } else if (myPos.y > hero.pos.y) {
                            return Direction.West;
                        } else {
                            return Direction.East;
                        }
                    } else if (Math.Abs(myPos.x - hero.pos.x) + Math.Abs(myPos.y - hero.pos.y) == 1) {
                        //Maybe there's a tavern
                        if (myPos.x < size && gameState.board[myPos.x + 1][myPos.y] == Tile.TAVERN) {
                            return Direction.South;
                        }
                        if (myPos.y < size && gameState.board[myPos.x][myPos.y + 1] == Tile.TAVERN) {
                            return Direction.East;
                        }
                        if (myPos.x > 0 && gameState.board[myPos.x - 1][myPos.y] == Tile.TAVERN) {
                            return Direction.North;
                        }
                        if (myPos.y > 0 && gameState.board[myPos.x][myPos.y - 1] == Tile.TAVERN) {
                            return Direction.West;
                        }
                        return Direction.Stay;
                    }
                }
            }
            return "";
        }

        private PathData SeekMine(GameState gameState, IPathfinder pathfinder)
        {
            List<Tile> mineTiles = new List<Tile>(new[] {
                Tile.GOLD_MINE_NEUTRAL,
                Tile.GOLD_MINE_1,
                Tile.GOLD_MINE_2,
                Tile.GOLD_MINE_3,
                Tile.GOLD_MINE_4,
            });
            if (gameState.myHero.id == 1) {
                mineTiles.Remove(Tile.GOLD_MINE_1);
            } else if (gameState.myHero.id == 2) {
                mineTiles.Remove(Tile.GOLD_MINE_2);
            } else if (gameState.myHero.id == 3) {
                mineTiles.Remove(Tile.GOLD_MINE_3);
            } else if (gameState.myHero.id == 4) {
                mineTiles.Remove(Tile.GOLD_MINE_4);
            }
            return SeekTiles(gameState, pathfinder, MINE_COST, mineTiles.ToArray());
        }

        private PathData SeekTavern(GameState gameState, IPathfinder pathfinder)
        {
            return SeekTiles(gameState, pathfinder, TAVERN_COST, Tile.TAVERN);
        }

        private PathData SeekTiles(GameState gameState, IPathfinder pathfinder, int tileCost, params Tile[] soughtTiles)
        {
            // Scan game board and find path data to all matching tiles
            List<Tuple<Pos, Tile, PathData>> moves = new List<Tuple<Pos, Tile, PathData>>();
            SortedSet<Tile> tiles = new SortedSet<Tile>(soughtTiles);
            for (int x = 0; x < gameState.board.Length; ++x) {
                for (int y = 0; y < gameState.board[x].Length; ++y) {
                    Tile tile = gameState.board[x][y];
                    if (tiles.Contains(tile)) {
                        Pos pos = new Pos(x, y);
                        PathData curPathData = pathfinder.pathTo(pos, gameState.myHero.pos, gameState, SPIKE_COST);
                        
                        // Fix health if we don't have one
                        if (curPathData.lostHealth == 0) {
                            curPathData.lostHealth = curPathData.distance;
                        }

                        // Add tile cost to health cost
                        curPathData.lostHealth += tileCost;
                        
                        // Add potential target.
                        moves.Add(Tuple.Create(pos, tile, curPathData));
                    }
                }
            }

            // Seek to minimize lost health.
            moves.Sort((a, b) => a.Item3.lostHealth - b.Item3.lostHealth);

            // Find a move that will take us to the target.
            PathData pathData = null;
            if (moves.Count != 0 && moves[0].Item3.distance < 1000) {
                Debug.Assert(target == null);
                pathData = moves[0].Item3;
                target = new TargetInfo(moves[0].Item1, pathData.distance, moves[0].Item2);
                if (pathData.lostHealth >= gameState.myHero.life) {
                    Console.WriteLine("EvenBestChoice: WARNING: current choice will kill us: costs {0}, remaining life {1}",
                        pathData.lostHealth, gameState.myHero.life);
                }
            }
            return pathData;
        }
    }
}
