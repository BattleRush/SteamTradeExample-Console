using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconService.GetTradeOffers
{
	public class TradeSendFromIEconService_Root
	{
		[JsonProperty("response")]
		public TradeSendFromIEconService_Response Response { get; set; }
	}
}
