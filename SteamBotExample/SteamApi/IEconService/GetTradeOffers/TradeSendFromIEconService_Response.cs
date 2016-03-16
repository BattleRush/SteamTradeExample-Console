using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconService.GetTradeOffers
{
	public class TradeSendFromIEconService_Response
	{
		[JsonProperty("trade_offers_sent")]
		public TradeOfferFromIEconService[] TradeOffersSent { get; set; }

		//received? -> Not needed at the moment
	}
}
