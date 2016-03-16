using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconService.GetTradeOffers
{
	public class TradeOfferFromIEconService
	{
		[JsonProperty("tradeofferid")]
		public string TradeOfferId { get; set; }

		[JsonProperty("accountid_other")]
		public string AccountIdOther { get; set; }

		[JsonProperty("message")]//empty?
		public string Message { get; set; }

		[JsonProperty("expiration_time")]
		public string ExpirationTime { get; set; }

		[JsonProperty("trade_offer_state")]
		public string TradeOfferState { get; set; }

		[JsonProperty("items_to_give")]
		public IEconService_Items[] ItemsToGive { get; set; }

		[JsonProperty("items_to_receive")]
		public IEconService_Items[] ItemsToReceive { get; set; }

		[JsonProperty("is_our_offer")]
		public string IsOurOffer { get; set; }

		[JsonProperty("time_created")]
		public string TimeCreated { get; set; }

		[JsonProperty("time_updated")]
		public string TimeUpdated { get; set; }

		[JsonProperty("from_real_time_trade")]
		public string FromRealTimeTrade { get; set; }

		[JsonProperty("escrow_end_date")]
		public string EscrowEndDate { get; set; }

		//received? -> Not needed at the moment
	}
}
