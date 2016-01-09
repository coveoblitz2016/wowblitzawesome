using System;
using Coveo;

namespace CoveoBlitz.WowBlitzAwsome.Bot
{
    /// <summary>
    /// Robot using <see cref="IBestChoice"/> to pick moves.
    /// </summary>
    public class BestBot : ISimpleBot
    {
        private IBestChoice bestChoice;
        private IPathfinder pathfinder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bestChoice">Object to pick best choice for next move.</param>
        /// <param name="pathfinder">Object to compute paths.</param>
        public BestBot(IBestChoice bestChoice, IPathfinder pathfinder)
        {
            this.bestChoice = bestChoice;
            this.pathfinder = pathfinder;
        }

        public string Move(GameState state)
        {
            if (bestChoice != null && pathfinder != null) {
                return bestChoice.BestMove(state, pathfinder);
            } else {
                return CoveoBlitz.Direction.Stay;
            }
        }

        public void Setup()
        {
            Console.WriteLine("Wow Blitz Awsome BestBot: activate");
            if (bestChoice == null) {
                Console.WriteLine("Careful: no best choice provided, won't work");
            }
            if (pathfinder == null) {
                Console.WriteLine("Careful: no pathfinder provided, won't work");
            }
        }

        public void Shutdown()
        {
            Console.WriteLine("Wow Blitz Awsome BestBot: shutting down");
        }
    }
}
