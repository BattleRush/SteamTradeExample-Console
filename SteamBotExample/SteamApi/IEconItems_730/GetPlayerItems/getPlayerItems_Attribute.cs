using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconItems_730.GetPlayerItems
{
	public class getPlayerItems_Attribute
	{
		[JsonProperty("defindex")]
		public string DefIndex { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("Float_value")]
		public string Float_value { get; set; }
	}
}
