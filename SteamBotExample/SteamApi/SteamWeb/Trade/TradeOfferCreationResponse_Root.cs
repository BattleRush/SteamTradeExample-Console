using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb.Trade
{
	public class TradeOfferCreationResponse_Root
	{
		[JsonProperty("tradeofferid")]
		public string TradeOfferId { get; set; }
	}
}
