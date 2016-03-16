using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconItems_730.GetPlayerItems
{
	public class getPlayerItems_Items
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("original_id")]
		public string OriginalId { get; set; }

		[JsonProperty("defindex")]
		public string DefIndex { get; set; }

		[JsonProperty("level")]
		public string Level { get; set; }

		[JsonProperty("quality")]
		public string Quality { get; set; }

		[JsonProperty("inventory")]
		public string Inventory { get; set; }

		[JsonProperty("quantity")]
		public string Quantity { get; set; }
					
		[JsonProperty("rarity")]
		public string Rarity { get; set; }

		[JsonProperty("flag_cannot_trade")]
		public string FlagCannotTrade { get; set; }

		[JsonProperty("flag_cannot_craft")]
		public string FlagCannotCraft { get; set; }

		[JsonProperty("attributes")]
		public getPlayerItems_Attribute[] Attributes { get; set; }
	}
}
