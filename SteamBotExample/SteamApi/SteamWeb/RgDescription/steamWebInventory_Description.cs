using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb.RgDescription
{
	public class steamWebInventory_Description
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }
	}
}
