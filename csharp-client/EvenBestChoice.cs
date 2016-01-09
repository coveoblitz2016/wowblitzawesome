using System;
using System.Collections.Generic;
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
            List<PathData> moves = new List<PathData>();
            SortedSet<Tile> tiles = new SortedSet<Tile>(soughtTiles);
            for (int i = 0; i < gameState.board.Length; ++i) {
                for (int j = 0; j < gameState.board[i].Length; ++j) {
                    if (tiles.Contains(gameState.board[i][j])) {
                        moves.Add(pathfinder.pathTo(new Pos { x = i, y = j }, gameState.myHero.pos, gameState.board));
                    }
                }
            }

            // Seek to minimize lost health
            moves.Sort((a, b) => a.lostHealth - b.lostHealth);

            if (moves.Count != 0) {
                // #clp TODO fixme
                return moves[0].nextDirection.ToString();
            } else {
                return CoveoBlitz.Direction.Stay;
            }
        }
    }
}
