using System;
using System.Collections.Generic;
using System.Linq;
using CoveoBlitz;

namespace Coveo
{
	public class JackBot : ISimpleBot
	{

		private IPathfinder pathfinder;

		public JackBot(IPathfinder pathfinder) {
			this.pathfinder = pathfinder;
		}

		public void Setup ()
		{
			Console.WriteLine ("Starting up Jack! He's AWSOME!");
		}

		public string Move (GameState state)
		{
			return this.EvalBestMove(state);
		}

		public void Shutdown ()
		{
			Console.WriteLine ("Done");
		}

		private string EvalBestMove (GameState state)
		{
			Pos tavernPos = state.GetTilePos (Tile.TAVERN);

			List<Pos> availableMinesPos = state.GetAvailableMinesPos ();

			List<PathData> availableMinesPath = availableMinesPos.Select ((Pos minePos) => 
				this.pathfinder.pathTo(minePos, state.myHero.pos, state.board)).ToList();

			availableMinesPath.ForEach ((PathData minePath) => {
				PathData pathBetweenMineAndTavern = this.pathfinder.pathTo(tavernPos, minePath.destination, state.board);
				minePath.lostHealth += pathBetweenMineAndTavern.lostHealth;
			});

			availableMinesPath.Sort ((PathData pathA, PathData pathB) => {
				return pathA.distance < pathB.distance ? 0 : 1;
			});

			// Make sure not to suicide
			if (availableMinesPath.First ().lostHealth < state.myHero.life) {
				return availableMinesPath.First ().nextDirection;
			} else {
				// We should go to tavern
				PathData pathToTavern = this.pathfinder.pathTo(tavernPos, state.myHero.pos, state.board);
				return pathToTavern.nextDirection;
			}
		}
	}
}

