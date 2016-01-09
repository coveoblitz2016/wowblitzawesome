using System;
using CoveoBlitz;

namespace Coveo
{
	public class PriorityFinder
	{
		public PriorityFinder ()
		{
		}

		/// <summary>
		/// Find where thr bot should go
		/// </summary>
		/// <returns>The Tavern tile or Free if it should find a gold mine</returns>
		/// <param name="state">State.</param>
		public Tile getPriority (GameState state)
		{
			int life = state.myHero.life;
			Tile tileToGo;

			// TODO find the distance between the bot and the tavern, eval life required to go to tavern

			if (life > 40) {
				tileToGo = Tile.FREE;
			} else {
				tileToGo = Tile.TAVERN;
			}

			return tileToGo;
		}
	}
}

