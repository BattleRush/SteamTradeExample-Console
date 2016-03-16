using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.IEconService.GetTradeOffers
{
	public class IEconService_Items
	{
		[JsonProperty("appid")]
		public string AppId { get; set; }

		[JsonProperty("contextid")]
		public string ContextId { get; set; }

		[JsonProperty("amount")]
		public string Amount { get; set; }

		[JsonProperty("assetid")]
		public string Assetid { get; set; }

		[JsonProperty("classid")]
		public string ClassId { get; set; }

		[JsonProperty("instanceid")]
		public string InstanceId { get; set; }

		[JsonProperty("missing")]
		public string Missing { get; set; }
	}
}
