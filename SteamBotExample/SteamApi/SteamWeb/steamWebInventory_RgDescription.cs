using Newtonsoft.Json;
using SteamBotExample.SteamApi.SteamWeb.RgDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.SteamApi.SteamWeb
{
	public class steamWebInventory_RgDescription
	{
		[JsonProperty("appid")]
		public string AppId { get; set; }

		[JsonProperty("classid")]
		public string ClassId { get; set; }

		[JsonProperty("instanceid")]
		public string InstanceId { get; set; }

		[JsonProperty("icon_url")]
		public string IconUrl { get; set; }

		[JsonProperty("icon_drag_url")]
		public string IconDragUrl { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("market_hash_name")]
		public string MarketHashName { get; set; }

		[JsonProperty("market_name")]
		public string MarketName { get; set; }

		[JsonProperty("name_color")]
		public string NameColor { get; set; }

		[JsonProperty("background_color")]
		public string BackgroundColor { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("tradable")]
		public string Tradable { get; set; }

		[JsonProperty("marketable")]
		public string Marketable { get; set; }

		[JsonProperty("commodity")]
		public string Commodity { get; set; }

		[JsonProperty("market_tradable_restriction")]
		public string MarketTradableRestriction { get; set; }

		[JsonProperty("cache_expiration")]
		public string CacheExpiration { get; set; }

		[JsonProperty("descriptions")]
		public steamWebInventory_Description[] Descriptions { get; set; }//[]

		[JsonProperty("actions")]
		public steamWebInventory_Action[] Action { get; set; }//[]

		[JsonProperty("market_actions")]
		public steamWebInventory_MarketAction[] MarketActions { get; set; }//[]

		[JsonProperty("tags")]
		public steamWebInventory_Tag[] Tags { get; set; }//[]
	}

}
