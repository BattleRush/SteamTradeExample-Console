using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb.RgDescription
{
	public class steamWebInventory_Tag
	{
		[JsonProperty("internal_name")]
		public string InternalName { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("category")]
		public string Category { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }

		[JsonProperty("category_name")]
		public string CategoryName { get; set; }
	}
}
