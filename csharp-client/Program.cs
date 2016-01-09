﻿// Copyright (c) 2005-2016, Coveo Solutions Inc.

using CoveoBlitz;
using CoveoBlitz.RandomBot;
using System;
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
                Console.WriteLine("Usage: myBot.exe key training|arena gameId");
                Console.WriteLine("gameId is optionnal when in training mode");
                Console.ReadKey();
                return;
            }

            string serverURL = "http://blitz2016.xyz:8080";
            string gameId = args.Length == 3 ? args[2] : null;

            IBestChoice bestChoice = new EvenBestChoice();
            IPathfinder pathfinder = null;

            ISimpleBot bot;
            if (bestChoice != null && pathfinder != null) {
                bot = new BestBot(bestChoice, pathfinder);
            } else {
                bot = new RandomBot();
            }

            SimpleBotRunner runner = new SimpleBotRunner(
                new ApiToolkit(serverURL, args[0], args[1] == "training", gameId),
                bot);

            runner.Run();

            Console.Read();
        }
    }
}