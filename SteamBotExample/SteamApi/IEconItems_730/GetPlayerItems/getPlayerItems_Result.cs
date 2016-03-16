using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconItems_730.GetPlayerItems
{
	public class getPlayerItems_Result
	{
		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("items")]
		public getPlayerItems_Items[] Items { get; set; }
	}
}
