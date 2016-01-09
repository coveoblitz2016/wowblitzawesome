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
		/// <returns>The Tavern tile or -1 if it should find a gold mine</returns>
		/// <param name="state">State.</param>
		public int getPriority (GameState state)
		{
			int life = state.myHero.life;
			int tileToGo;

			// TODO find the distance between the bot and the tavern, eval life required to go to tavern

			if (life > 40) {
				tileToGo = -1;
			} else {
				tileToGo = Tile.TAVERN;
			}

			return tileToGo;
		}
	}
}

