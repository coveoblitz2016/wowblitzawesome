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
        public string BestMove(GameState gameState, IPathfinder pathfinder)
        {
            if (gameState.myHero.life >= 50) {
                return SeekMine(gameState, pathfinder);
            } else {
                return SeekTavern(gameState, pathfinder);
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
                        //Console.WriteLine("EvenBestChoice: seeking path to ({0},{1}) [tile {2}]", x, y, tile);
                        moves.Add(Tuple.Create(new Pos(x, y), tile, (PathData) null));
                    }
                }
            }
            //Console.WriteLine("EvenBestChoice: seeking paths to {0} tiles", moves.Count);
            for (int i = 0; i < moves.Count; ++i) {
                Stopwatch watch = Stopwatch.StartNew();
                PathData pathData = pathfinder.pathTo(moves[i].Item1, gameState.myHero.pos, gameState.board);
                //Console.WriteLine("EvenBestChoice: sought path to ({0},{1}) [tile {2}] in {3}ms",
                //    moves[i].Item1.x, moves[i].Item1.y, moves[i].Item2, watch.ElapsedMilliseconds);
                moves[i] = Tuple.Create(moves[i].Item1, moves[i].Item2, pathData);
            }

            // Seek to minimize lost health
            moves.Sort((a, b) => a.Item3.lostHealth - b.Item3.lostHealth);

            if (moves.Count != 0) {
                Direction move = moves[0].Item3.nextDirection;
                string moveStr = null;
                if (move == Direction.North) {
                    moveStr = CoveoBlitz.Direction.North;
                } else if (move == Direction.South) {
                    moveStr = CoveoBlitz.Direction.South;
                } else if (move == Direction.East) {
                    moveStr = CoveoBlitz.Direction.East;
                } else if (move == Direction.West) {
                    moveStr = CoveoBlitz.Direction.West;
                }
                if (moveStr == null) {
                    Console.WriteLine("EvenBestChoice: unknown direction: {0}", move);
                }
                return moveStr ?? CoveoBlitz.Direction.Stay;
            } else {
                return CoveoBlitz.Direction.Stay;
            }
        }
    }
}
