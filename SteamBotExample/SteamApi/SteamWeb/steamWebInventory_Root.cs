using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb
{
	public class steamWebInventory_Root
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("rgInventory")]
		public Dictionary<string, steamWebInventory_InventoryItem> RgInventory { get; set; }//[]

		[JsonProperty("rgCurrency")]
		public steamWebInventory_RgCurrency[] RgCurrency { get; set; }

		[JsonProperty("rgDescriptions")]
		public Dictionary<string, steamWebInventory_RgDescription> RgDescriptions { get; set; }//[]

		[JsonProperty("more")]
		public string More { get; set; }

		[JsonProperty("more_start")]
		public string MoreStart { get; set; }
	}
}
