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

        public string BestMove(GameState gameState, IPathfinder pathfinder)
        {
            // If we suddenly lost a lot of life, maybe we should reconsider.
            if (heroLife.HasValue && heroLife.Value >= (gameState.myHero.life + 20)) {
                Console.WriteLine("EvenBestChoice: LOW ON LIFE! Maybe we were attacked?");
                target = null;
            }
            heroLife = gameState.myHero.life;

            // If we suddenly moved a lot, maybe we should reconsider.
            if (heroPos != null && MovedALot(heroPos, gameState.myHero.pos)) {
                Console.WriteLine("EvenBestChoice: TELEPORTED! Maybe we were killed?");
                target = null;
            }
            heroPos = gameState.myHero.pos;

            string move = null;
            if (target != null) {
                Console.WriteLine("EvenBestChoice: Current target: ({0},{1}) [tile {2}]", target.pos.x, target.pos.y, target.tile);
                PathData pathData = pathfinder.pathTo(target.pos, gameState.myHero.pos, gameState.board, SPIKE_COST);
                move = pathData.nextDirection;
            } else {
                // Seek mine if possible, otherwise seek a tavern
                if (gameState.myHero.life >= 50) {
                    move = SeekMine(gameState, pathfinder);
                }
                if (String.IsNullOrEmpty(move) && gameState.myHero.life < 50) {
                    move = SeekTavern(gameState, pathfinder);
                }
            }
            if (String.IsNullOrEmpty(move)) {
                move = Direction.Stay;
            }

            // If this is the last move, release target unless it's a tavern and we're < 90 life
            if (target != null && move != Direction.Stay && target.distance <= 1 && (target.tile != Tile.TAVERN || gameState.myHero.life >= 90)) {
                target = null;
            }
            return move;
        }

        private string SeekMine(GameState gameState, IPathfinder pathfinder)
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

        private string SeekTavern(GameState gameState, IPathfinder pathfinder)
        {
            return SeekTiles(gameState, pathfinder, TAVERN_COST, Tile.TAVERN);
        }

        private string SeekTiles(GameState gameState, IPathfinder pathfinder, int tileCost, params Tile[] soughtTiles)
        {
            // Scan game board and find path data to all matching tiles
            List<Tuple<Pos, Tile, PathData>> moves = new List<Tuple<Pos, Tile, PathData>>();
            SortedSet<Tile> tiles = new SortedSet<Tile>(soughtTiles);
            for (int x = 0; x < gameState.board.Length; ++x) {
                for (int y = 0; y < gameState.board[x].Length; ++y) {
                    Tile tile = gameState.board[x][y];
                    if (tiles.Contains(tile)) {
                        Pos pos = new Pos(x, y);
                        PathData pathData = pathfinder.pathTo(pos, gameState.myHero.pos, gameState.board, SPIKE_COST);
                        
                        // Fix health if we don't have one
                        if (pathData.lostHealth == 0) {
                            pathData.lostHealth = pathData.distance;
                        }

                        // Add tile cost to health cost
                        pathData.lostHealth += tileCost;
                        
                        // Add potential target.
                        moves.Add(Tuple.Create(pos, tile, pathData));
                    }
                }
            }

            // Seek to minimize lost health.
            moves.Sort((a, b) => a.Item3.lostHealth - b.Item3.lostHealth);

            string move = null;
            if (moves.Count != 0 && moves[0].Item3.distance < 1000) {
                Debug.Assert(target == null);
                target = new TargetInfo(moves[0].Item1, moves[0].Item3.distance, moves[0].Item2);
                move = moves[0].Item3.nextDirection;
                if (moves[0].Item3.lostHealth >= gameState.myHero.life) {
                    Console.WriteLine("EvenBestChoice: WARNING: current choice will kill us: costs {0}, remaining life {1}",
                        moves[0].Item3.lostHealth, gameState.myHero.life);
                }
            }
            return !String.IsNullOrEmpty(move) ? move : null;
        }

        private static bool MovedALot(Pos lastPos, Pos curPos)
        {
            int xDist, yDist;
            if (lastPos.x < curPos.x) {
                xDist = curPos.x - lastPos.x;
            } else {
                xDist = lastPos.x - curPos.x;
            }
            if (lastPos.y < curPos.y) {
                yDist = curPos.y - lastPos.y;
            } else {
                yDist = lastPos.y - curPos.y;
            }
            return (xDist + yDist) >= 2;
        }
    }
}
