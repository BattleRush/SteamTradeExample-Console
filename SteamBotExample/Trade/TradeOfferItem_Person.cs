using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.Trade
{
	public class TradeOfferItem_Person
	{
		[JsonProperty("assets")]
		public TradeOfferItemAsset[] Assets { get; set; }

		[JsonProperty("currency")]
		public TradeOfferItemAsset[] Currency { get; set; }

		[JsonProperty("ready")]
		public string Ready { get; set; }
	}
	
}
