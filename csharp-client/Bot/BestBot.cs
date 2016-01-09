using System;
using System.Diagnostics;
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
        private bool printedBoard = false;

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
            if (!printedBoard) {
                PrintBoard(state);
                printedBoard = true;
            }

            if (bestChoice != null && pathfinder != null) {
                Stopwatch watch = Stopwatch.StartNew();
                string move = bestChoice.BestMove(state, pathfinder);
                long elapsed_ms = watch.ElapsedMilliseconds;
                Console.WriteLine("BestBot: next move: {0} (computed in {1} ms)", move, elapsed_ms);
                return move;
            } else {
                Console.WriteLine("BestBot: next move: NONE");
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

        private void PrintBoard(GameState state)
        {
            for (int x = 0; x < state.board.Length; ++x) {
                for (int y = 0; y < state.board[x].Length; ++y) {
                    switch (state.board[x][y])
                    {
                    case Tile.IMPASSABLE_WOOD:
                        Console.Write("##");
                        break;
                    case Tile.FREE:
                        Console.Write("  ");
                        break;
                    case Tile.SPIKES:
                        Console.Write("^^");
                        break;
                    case Tile.HERO_1:
                        Console.Write("@1");
                        break;
                    case Tile.HERO_2:
                        Console.Write("@2");
                        break;
                    case Tile.HERO_3:
                        Console.Write("@3");
                        break;
                    case Tile.HERO_4:
                        Console.Write("@4");
                        break;
                    case Tile.TAVERN:
                        Console.Write("[]");
                        break;
                    case Tile.GOLD_MINE_NEUTRAL:
                        Console.Write("$-");
                        break;
                    case Tile.GOLD_MINE_1:
                        Console.Write("$1");
                        break;
                    case Tile.GOLD_MINE_2:
                        Console.Write("$2");
                        break;
                    case Tile.GOLD_MINE_3:
                        Console.Write("$3");
                        break;
                    case Tile.GOLD_MINE_4:
                        Console.Write("$4");
                        break;
                    default:
                        Console.Write("??");
                        break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
