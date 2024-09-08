// <copyright file="Program.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace HonjoMarketBot
{
    using System;
    using System.Threading.Tasks;
    using HonjoMarketBot.Classes;
    using NQutils.Config;

    /// <summary>
    /// Main Entry point to app.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main Entry point to app.
        /// </summary>
        /// <param name="args">Paths to the config files.</param>
        public static void Main(string[] args)
        {
            var cmdline = string.Join(",", args);

            Console.WriteLine($"{cmdline}");

            try
            {
                Config.ReadYamlFileFromArgs("mod", args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}\n{e.StackTrace}");
                return;
            }

            Mod.Setup().Wait();
            var trader = new ModHonjoMarketBot(args[1]);
            trader.Start().Wait();
        }
    }
}