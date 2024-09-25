// <copyright file="HonjoConfig.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace HonjoMarketBot.Classes
{
    using System.Collections.Generic;

    /// <summary>
    /// A class containing the config settings for this mod.
    /// </summary>
    public class HonjoConfig
    {
        /// <summary>
        /// Gets or sets list of items to buy and the top price to pay.
        /// </summary>
        public Dictionary<string, double> BuyPrices { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Gets or sets list of items to recursively buy and the top price to pay.
        /// </summary>
        public Dictionary<string, double> BuyRecursivePrices { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Gets or sets list of market ids and there multiplier.
        /// </summary>
        public Dictionary<string, double> MarketBudgetMultiplier { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Gets or sets planets / parents to iterate over for markets.
        /// </summary>
        public ulong[] Planets { get; set; } = { 0 };

        /// <summary>
        /// Gets or sets the numbers days to buy before expiration.
        /// </summary>
        /// <example> 1 means once its goes under 24hrs remaining itll buy if within the buy range.</example>
        public int DaysToWaitBeforeExpiration { get; set; } = 1;

        /// <summary>
        /// Gets or sets the bot's player name.
        /// </summary>
        public string BotName { get; set; } = "honjosminion";

        /// <summary>
        /// Gets or sets the bot's markup.
        /// </summary>
        public double BotMarkup { get; set; } = 1.1; // 10%
    }
}
