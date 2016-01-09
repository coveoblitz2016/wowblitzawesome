using System.Collections.Generic;
using System.Linq;

namespace CoveoBlitz
{
    public class GameState
    {
        public Hero myHero { get; set; }
        public List<Hero> heroes { get; set; }

        public int currentTurn { get; set; }
        public int maxTurns { get; set; }
        public bool finished { get; set; }
        public bool errored { get; set; }

        public Tile[][] board { get; set; }

		public Pos GetTilePos(Tile tile) 
		{
			for (int x = 0; x < this.board.Length; ++x) {
				for (int y = 0; y < this.board [x].Length; ++y) {
					if (this.board [x] [y] == tile) {
						return new Pos (x, y);
					}
				}
			}

			// Should not happen...
			return null;
		}

		public List<Pos> GetTilesPos(List<Tile> tiles) {
			List<Pos> minesPos = new List<Pos>();

			tiles.ForEach ((Tile tile) => {
				for (int x = 0; x < this.board.Length; ++x) {
					for (int y = 0; y < this.board [x].Length; ++y) {
						if (this.board [x] [y] == tile) {
							minesPos.Add(new Pos(x, y));
						}
					}
				}
			});

			return minesPos;
		}

		public List<Tile> GetAvailableMinesTiles ()
		{
			List<Tile> mineTiles = new List<Tile> (new[] {
				Tile.GOLD_MINE_NEUTRAL,
				Tile.GOLD_MINE_1,
				Tile.GOLD_MINE_2,
				Tile.GOLD_MINE_3,
				Tile.GOLD_MINE_4,
			});

			if (this.myHero.id == 1) {
				mineTiles.Remove (Tile.GOLD_MINE_1);
			} else if (this.myHero.id == 2) {
				mineTiles.Remove (Tile.GOLD_MINE_2);
			} else if (this.myHero.id == 3) {
				mineTiles.Remove (Tile.GOLD_MINE_3);
			} else if (this.myHero.id == 4) {
				mineTiles.Remove (Tile.GOLD_MINE_4);
			}

			return mineTiles;
		}

		public List<Pos> GetAvailableMinesPos ()
		{
			return this.GetTilesPos (this.GetAvailableMinesTiles ());
		}
    }
}