using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb
{
	public class steamWebInventory_InventoryItem
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("classid")]
		public string ClassId { get; set; }

		[JsonProperty("instanceid")]
		public string InstanceId { get; set; }

		[JsonProperty("amount")]
		public string Amount { get; set; }

		[JsonProperty("pos")]
		public string Pos { get; set; }
	}
}
