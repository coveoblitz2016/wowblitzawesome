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

        private int? lastLife;
        private Pos target;
        private Tile targetTile = Tile.FREE;

        public string BestMove(GameState gameState, IPathfinder pathfinder)
        {
            // If we suddenly lost a lot of life, maybe we should reconsider.
            if (lastLife.HasValue && lastLife.Value >= (gameState.myHero.life + 20)) {
                target = null;
                targetTile = Tile.FREE;
            }
            lastLife = gameState.myHero.life;

            if (target != null) {
                Console.WriteLine("EvenBestChoice: Current target: ({0},{1}) [tile {2}]", target.x, target.y, targetTile);
                PathData pathData = pathfinder.pathTo(target, gameState.myHero.pos, gameState.board, SPIKE_COST);
                // If this is the last move, release target unless it's a tavern and we're < 90 life
                if (pathData.distance <= 1 && (targetTile != Tile.TAVERN || gameState.myHero.life >= 90)) {
                    target = null;
                    targetTile = Tile.FREE;
                }
                return pathData.nextDirection;
            } else {
                if (gameState.myHero.life >= 50) {
                    return SeekMine(gameState, pathfinder);
                } else {
                    return SeekTavern(gameState, pathfinder);
                }
            }
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
            return SeekTiles(gameState, pathfinder, mineTiles.ToArray());
        }

        private string SeekTavern(GameState gameState, IPathfinder pathfinder)
        {
            return SeekTiles(gameState, pathfinder, Tile.TAVERN);
        }

        private string SeekTiles(GameState gameState, IPathfinder pathfinder, params Tile[] soughtTiles)
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
                        moves.Add(Tuple.Create(pos, tile, pathData));

                        // Fix health if we don't have one
                        if (pathData.lostHealth == 0) {
                            pathData.lostHealth = pathData.distance;
                        }
                    }
                }
            }

            // Seek to minimize lost health.
            moves.Sort((a, b) => a.Item3.lostHealth - b.Item3.lostHealth);

            string move = null;
            if (moves.Count != 0 && moves[0].Item3.distance < 1000) {
                Debug.Assert(target == null);
                Debug.Assert(targetTile == Tile.FREE);
                target = moves[0].Item1;
                targetTile = moves[0].Item2;
                move = moves[0].Item3.nextDirection;
            }
            return !String.IsNullOrEmpty(move) ? move : Direction.Stay;
        }
    }
}
