using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconItems_730.GetPlayerItems
{
	public class getPlayerItems_Root
	{
		[JsonProperty("result")]
		public getPlayerItems_Result Result { get; set; }
	}
}
