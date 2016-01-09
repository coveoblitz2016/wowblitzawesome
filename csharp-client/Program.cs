// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using CoveoBlitz;
using CoveoBlitz.RandomBot;
using CoveoBlitz.WowBlitzAwsome.Bot;

namespace Coveo
{
    internal class MyBot
    {
        public static int spikeDamage = 10;
        /**
         * @param args args[0] Private key
         * @param args args[1] [training|arena]
         * @param args args[2] Game Id
         */

        private static void Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine("Usage: myBot.exe key training|arena|HAX gameId");
                Console.WriteLine("gameId is optionnal when in training mode");
                Console.ReadKey();
                return;
            }

            string serverURL = "http://blitz2016.xyz:8080";
            bool trainingMode = args[1] == "training" || args[1] == "HAX";
            string gameId = args.Length == 3 ? args[2] : null;

            IBestChoice bestChoice = new EvenBestChoice();
            IPathfinder pathfinder = new Disjkstra();

            ISimpleBot bot;
            if (args[1] == "HAX") {
                bot = new ManualBot();
            } else if (bestChoice != null && pathfinder != null) {
                bot = new BestBot(bestChoice, pathfinder);
            } else {
                bot = new RandomBot();
            }
            
			// add a random param to start browser in training mode
            SimpleBotRunner runner = new SimpleBotRunner(
                new ApiToolkit(serverURL, args[0], trainingMode, gameId),
				bot, args[1] == "training" && args.Length == 3);

            runner.Run();

            Console.Read();
        }
    }
}