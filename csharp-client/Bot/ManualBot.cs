using System;
using System.Collections.Generic;
using System.Threading;

namespace CoveoBlitz.WowBlitzAwsome.Bot
{
    /// <summary>
    /// Bot that accepts user input. (Might be against the rules, shush)
    /// </summary>
    public sealed class ManualBot : ISimpleBot
    {
        private Thread inputThread;
        private bool quitFlag = false;
        private Queue<string> nextMoves = new Queue<string>();

        public string Move(GameState state)
        {
            string move = null;
            lock (nextMoves) {
                if (nextMoves.Count != 0) {
                    move = nextMoves.Dequeue();
                }
            }
            return move ?? CoveoBlitz.Direction.Stay;
        }

        public void Setup()
        {
            Console.WriteLine("Wow Blitz AWSOME Manual \"Bot\" (or something)");
            inputThread = new Thread(delegate() {
                while (!quitFlag) {
                    while (!quitFlag && !Console.KeyAvailable) {
                        Thread.Sleep(1);
                    }
                    if (!quitFlag) {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        string move = null;
                        if (keyInfo.Key == ConsoleKey.UpArrow) {
                            move = CoveoBlitz.Direction.North;
                        } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                            move = CoveoBlitz.Direction.South;
                        } else if (keyInfo.Key == ConsoleKey.LeftArrow) {
                            move = CoveoBlitz.Direction.West;
                        } else if (keyInfo.Key == ConsoleKey.RightArrow) {
                            move = CoveoBlitz.Direction.East;
                        }
                        if (move != null) {
                            lock (nextMoves) {
                                nextMoves.Enqueue(move);
                            }
                        }
                    }
                };
            });
            inputThread.Start();
        }

        public void Shutdown()
        {
            quitFlag = true;
            inputThread.Join();

            Console.WriteLine("ManualBot: done");
        }
    }
}
