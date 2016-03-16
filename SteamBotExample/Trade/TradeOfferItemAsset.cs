using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.Trade
{
	public class TradeOfferItemAsset
	{
		[JsonProperty("appid")]
		public string AppId { get; set; }

		[JsonProperty("contextid")]
		public string ContextId { get; set; }

		[JsonProperty("amount")]
		public string Amount { get; set; }

		[JsonProperty("assetid")]
		public string Assetid { get; set; }
	}
}
