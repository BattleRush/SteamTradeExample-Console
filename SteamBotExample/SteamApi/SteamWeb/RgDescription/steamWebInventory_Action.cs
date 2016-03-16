using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb.RgDescription
{
	public class steamWebInventory_Action
	{
		[JsonProperty("link")]
		public string link { get; set; }

		[JsonProperty("name")]
		public string name { get; set; }
	}
}
