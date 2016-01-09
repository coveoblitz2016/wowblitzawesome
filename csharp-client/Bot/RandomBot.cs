// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;

namespace CoveoBlitz.RandomBot
{
	/// <summary>
	/// RandomBot
	///
	/// This bot will randomly chose a direction each turns.
	/// </summary>
	public class RandomBot : ISimpleBot
	{
		private readonly Random random = new Random ();

		/// <summary>
		/// This will be run before the game starts
		/// </summary>
		public void Setup ()
		{
			Console.WriteLine ("Coveo's C# Wow Blitz AWSOME bot!");
		}

		/// <summary>
		/// This will be run on each turns. It must return a direction fot the bot to follow
		/// </summary>
		/// <param name="state">The game state</param>
		/// <returns></returns>
		public string Move (GameState state)
		{
			string direction;

			var canMove = state.myHero.life > 10;

			if (canMove) {
				switch (random.Next (0, 5)) {
				case 0:
					direction = Direction.East;
					break;

				case 1:
					direction = Direction.West;
					break;

				case 2:
					direction = Direction.North;
					break;

				default:
					direction = Direction.South;
					break;
				}
			} else {
				direction = Direction.Stay;
			}

			Console.WriteLine ("Completed turn {0}, going {1}", state.currentTurn, direction);
			return direction;
		}

		/// <summary>
		/// This is run after the game.
		/// </summary>
		public void Shutdown ()
		{
			Console.WriteLine ("Done");
		}
	}
}