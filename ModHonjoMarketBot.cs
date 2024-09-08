// <copyright file="ModHonjoMarketBot.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace HonjoMarketBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Backend;
    using BotLib.Generated;
    using HonjoMarketBot.Classes;
    using Newtonsoft.Json;
    using NQ;

    /// <summary>
    /// The main body of the bot.
    /// </summary>
    public class ModHonjoMarketBot : Mod
    {
        private readonly HonjoConfig _config;
        private Dictionary<ulong, double> _buyPrices = new Dictionary<ulong, double>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHonjoMarketBot"/> class.
        /// </summary>
        /// <param name="confPath">path to the json file.</param>
        public ModHonjoMarketBot(string confPath)
        {
            var confText = System.IO.File.ReadAllText(confPath);
            this._config = JsonConvert.DeserializeObject<HonjoConfig>(confText);
        }

        /// <summary>
        /// Setups the bot and gets ready to start buying.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Loop()
        {
            Bot = await CreateUser("honjosminion", true, false).ConfigureAwait(false);
            var bank = Bot.GameplayBank;

            foreach (var (key, value) in this._config.BuyRecursivePrices)
            {
                var baseEntry = bank.GetDefinition(key);
                var childrenIds = baseEntry.GetChildrenIdsRecursive();
                foreach (var childId in childrenIds)
                {
                    var entry = bank.GetDefinition(childId);
                    if (entry.GetChildren().Count() != 0)
                    {
                        continue;
                    }

                    this._buyPrices[childId] = value * 100;
                }
            }

            Console.WriteLine($"Trader will buy {this._buyPrices.Count} items");

            await this.SafeLoop(this.Action, 5000, async () =>
            {
                Bot = await CreateUser("honjosminion", true, false).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Buys items for us.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task BuyStuff(ulong parent)
        {
            // get all markets on alioth (ALioths ID (Construct) is 2)
            var ml = await Bot.Req.MarketGetList(parent).ConfigureAwait(false);

            // get all the market ids from the list
            var mids = ml.markets.Select(x => x.marketId).ToList();

            // loop over each market
            foreach (var mkt in ml.markets)
            {
                var doneIds = new List<ulong>();

                // get my orders for this market
                var orders = await Bot.Req.MarketSelectItem(
                    new MarketSelectRequest
                    {
                        marketIds = new List<ulong> { mkt.marketId },
                        itemTypes = this._buyPrices.Keys.ToList(),
                    }).ConfigureAwait(false);

                // loop over all my orders in this market
                foreach (var order in orders.orders)
                {
                    if (order.ownerName == "marketbot" || order.ownerName == "honjosminion")
                    {
                        // most likely a seeded order. skip
                        continue;
                    }

                    Console.WriteLine($@"Expiration Time: {order.expirationDate.ToDateTime()}");
                    var diff = order.expirationDate.ToDateTime() - DateTime.UtcNow;

                    if (diff.TotalDays > 1)
                    {
                        // over 24hrs remaining, let others try to buy first.
                        continue;
                    }

                    if (this._buyPrices[order.itemType] < order.unitPrice)
                    {
                        // over priced, don't touch;
                        continue;
                    }

                    var boughtItems = await Bot.Req.MarketInstantOrder(
                        new MarketRequest
                        {
                            marketId = order.marketId,
                            itemType = order.itemType,
                            buyQuantity = Math.Abs(order.buyQuantity),
                            unitPrice = order.unitPrice,
                        }).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///     What the bot will do.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Action()
        {
            foreach (ulong parent in this._config.Planets)
            {
                await this.BuyStuff(parent).ConfigureAwait(false);
            }

            await Task.Delay(10000).ConfigureAwait(false);
        }
    }
}