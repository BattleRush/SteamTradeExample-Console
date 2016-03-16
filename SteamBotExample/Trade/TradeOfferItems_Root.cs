using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.Trade
{
	public class TradeOfferItems_Root
	{
		[JsonProperty("newversion")]
		public string NewVersion { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("me")]
		public TradeOfferItem_Person Me { get; set; }

		[JsonProperty("them")]
		public TradeOfferItem_Person Them { get; set; }
	}
}
