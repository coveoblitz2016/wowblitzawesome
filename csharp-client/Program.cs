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
                Console.WriteLine("Usage: myBot.exe key training|arena gameId|showbrowser");
                Console.WriteLine("gameId is used to show browser in training mode");
                Console.ReadKey();
                return;
            }

            string serverURL = "http://blitz2016.xyz:8080";
            string apiKey = args[0];
            bool trainingMode = args[1] == "training";
            string gameId = args.Length == 3 && !trainingMode ? args[2] : null;
            bool showBrowser = trainingMode && args.Length == 3;

            IBestChoice bestChoice = new EvenBestChoice();
            IPathfinder pathfinder = new Disjkstra();

            ISimpleBot bot;
            if (bestChoice != null && pathfinder != null) {
                bot = new BestBot(bestChoice, pathfinder);
            } else {
                bot = new RandomBot();
            }
            
			// add a random param to start browser in training mode
            SimpleBotRunner runner = new SimpleBotRunner(
                new ApiToolkit(serverURL, apiKey, trainingMode, gameId, 300),
				bot,
                showBrowser);

            runner.Run();

            Console.Read();
        }
    }
}