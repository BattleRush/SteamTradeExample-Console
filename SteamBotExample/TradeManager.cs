using Newtonsoft.Json;
using SteamBotExample.Steam2FA;
using SteamBotExample.SteamApi.IEconService.GetTradeOffers;
using SteamBotExample.SteamApi.SteamWeb.Trade;
using SteamBotExample.Trade;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SteamBotExample
{
	public static class TradeManager
	{
		/// <summary>
		/// Returns NameValue Collection for the TradeData
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="partnerSteam64Id"></param>
		/// <param name="message"></param>
		/// <param name="itemsJson"></param>
		/// <param name="tokenJson"></param>
		/// <returns></returns>
		public static NameValueCollection GetTradeData(string sessionId, string partnerSteam64Id, string message, string itemsJson, string tokenJson)
		{
			var data = new NameValueCollection();
			data.Add("sessionid", sessionId);
			data.Add("serverid", "1");
			data.Add("partner", partnerSteam64Id);
			data.Add("tradeoffermessage", message);
			data.Add("json_tradeoffer", itemsJson);//JSON HERE
			data.Add("captcha", "");
			data.Add("trade_offer_create_params", tokenJson);

			return data;
		}

		/// <summary>
		/// Gets a JSON of the Token from a UserTradeUrl
		/// </summary>
		/// <param name="tradeUrl"></param>
		/// <returns></returns>
		private static string GetTradeOfferTokenJson(string tradeUrl)
		{
			Uri tradeLink = new Uri(tradeUrl);

			string token = HttpUtility.ParseQueryString(tradeLink.Query).Get("token");
			var offerToken = new OfferAccessToken() { TradeOfferAccessToken = token };

			return JsonConvert.SerializeObject(offerToken, new JsonSerializerSettings());
		}
		/// <summary>
		/// Returns JSON for the Items in the Tradey
		/// </summary>
		/// <param name="assetsToReceive"></param>
		/// <param name="assetsToGive"></param>
		/// <returns></returns>
		private static string GetTradeItemsJson(List<TradeOfferItemAsset> assetsToReceive, List<TradeOfferItemAsset> assetsToGive)
		{
			//Hardcoded
			string emptyJsonString = @"{""newversion"":true,""version"":1,""me"":{""assets"":[],""currency"":[],""ready"":false},""them"":{""assets"":[],""currency"":[],""ready"":false}}";

			TradeOfferItems_Root tradeOfferData = JsonConvert.DeserializeObject<TradeOfferItems_Root>(emptyJsonString);

			tradeOfferData.Them.Assets = assetsToReceive.ToArray();
			tradeOfferData.Me.Assets = assetsToGive.ToArray();

			//Version ammount of items on to gove and receive + 1
			tradeOfferData.Version = (tradeOfferData.Them.Assets.Count() + tradeOfferData.Me.Assets.Count()).ToString();

			return JsonConvert.SerializeObject(tradeOfferData);
		}

		/// <summary>
		/// Returns list of the Assets to trade
		/// </summary>
		/// <param name="list"></param>
		/// <param name="appId"></param>
		/// <param name="contextId"></param>
		/// <returns></returns>
		private static List<TradeOfferItemAsset> GetAssetTradeOfferItemsList(List<long> list, int appId, int contextId)
		{
			List<TradeOfferItemAsset> itemsList = new List<TradeOfferItemAsset>();
			foreach (var assetId in list)
			{
				itemsList.Add(new TradeOfferItemAsset() { Amount = "1", AppId = appId.ToString(), Assetid = assetId.ToString(), ContextId = contextId.ToString() });
			}
			return itemsList;
		}


		public static void CancelTrade(long tradeId, TradeBot bot)
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;//get all trades from last day

			try
			{
				//CANCEL TRADE
				string urlFormatCancel = "key={0}&format=json&tradeofferid={1}";

				string urlCancelData = string.Format(urlFormatCancel, bot.ApiKey, tradeId);
				string urlCancelMain = "https://api.steampowered.com/IEconService/CancelTradeOffer/v1/";

				using (WebClient cancelCient = new WebClient())
				{
					cancelCient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
					string HtmlResult = cancelCient.UploadString(urlCancelMain, urlCancelData);
				}
			}
			catch (Exception ex)
			{

			}
		}
		public static ETradeOfferState GetTradeStatus(long? tradeId, TradeBot bot, Int32 unixTime)
		{
			if (!tradeId.HasValue)
				return ETradeOfferState.ETradeOfferError;

			string urlFormat = "https://api.steampowered.com/IEconService/GetTradeOffers/v1/?key={0}&format=json&get_sent_offers=1&get_received_offers=0&get_descriptions=0&active_only=0&historical_only=0&time_historical_cutoff={1}";

			string url = string.Format(urlFormat, bot.ApiKey, unixTime);

			ETradeOfferState currentTradeState = ETradeOfferState.ETradeOfferNotFound;

			using (WebClient client = new WebClient())
			{
				try
				{
					string htmlCode = client.DownloadString(url);
					TradeSendFromIEconService_Root root = JsonConvert.DeserializeObject<TradeSendFromIEconService_Root>(htmlCode);
					var currentTrade = root.Response.TradeOffersSent.SingleOrDefault(i => i.TradeOfferId == tradeId.ToString());


					/*
					 * k_ETradeOfferStateInvalid		1	Invalid
					 * k_ETradeOfferStateActive			2	This trade offer has been sent, neither party has acted on it yet.
					 * k_ETradeOfferStateAccepted		3	The trade offer was accepted by the recipient and items were exchanged.
					 * k_ETradeOfferStateCountered		4	The recipient made a counter offer
					 * k_ETradeOfferStateExpired		5	The trade offer was not accepted before the expiration date
					 * k_ETradeOfferStateCanceled		6	The sender cancelled the offer
					 * k_ETradeOfferStateDeclined		7	The recipient declined the offer
					 * k_ETradeOfferStateInvalidItems	8	Some of the items in the offer are no longer available (indicated by the missing flag in the output)
					 * k_ETradeOfferStateEmailPending	9	The offer hasn't been sent yet and is awaiting email confirmation
					 * k_ETradeOfferStateEmailCanceled	10	The receiver cancelled the offer via email
					 * k_ETradeOfferStateInEscrow		11	The trade has been placed on hold. The items involved in the trade have all been removed from both parties' inventories and will be automatically delivered in the future.
					 */


					if (currentTrade != null)
					{
						currentTradeState = (ETradeOfferState)Enum.Parse(typeof(ETradeOfferState), currentTrade.TradeOfferState);

						switch (currentTradeState)
						{

							//Do Nothing
							case ETradeOfferState.ETradeOfferStateInvalid:
							case ETradeOfferState.ETradeOfferStateActive:
							case ETradeOfferState.ETradeOfferStateAccepted:
							case ETradeOfferState.ETradeOfferStateCountered:
							case ETradeOfferState.ETradeOfferStateExpired:
							case ETradeOfferState.ETradeOfferStateCanceled:
							case ETradeOfferState.ETradeOfferStateDeclined:
							case ETradeOfferState.ETradeOfferStateInvalidItems:
							case ETradeOfferState.ETradeOfferStateCreatedNeedsConfirmation:
							case ETradeOfferState.ETradeOfferStateCanceledBySecondFactor:
								return currentTradeState;
							case ETradeOfferState.ETradeOfferStateInEscrow:
								//Update escrow end date and set status
	
								break;
							default:
								return ETradeOfferState.ETradeOfferNotFound;
						}

					}

					return currentTradeState;
				}
				catch (Exception ex)
				{
					return ETradeOfferState.ETradeOfferError;
				}
			}
		}


		public static long SendTrade(TradeBot fromBot, long steam64IdToSend, string tradeUrlToSend, List<long> assetIdsToGive, List<long> assetIdsToReceive, int appId, int contextId)
		{
			long tradeIdLong = 0;

			try
			{
				List<TradeOfferItemAsset> assetsToReceive = new List<TradeOfferItemAsset>();
				List<TradeOfferItemAsset> assetsToGive = new List<TradeOfferItemAsset>();

				assetsToReceive = GetAssetTradeOfferItemsList(assetIdsToReceive, appId, contextId);
				assetsToGive = GetAssetTradeOfferItemsList(assetIdsToGive, appId, contextId);		

				string itemsJson = GetTradeItemsJson(assetsToReceive, assetsToGive);

				if (string.IsNullOrWhiteSpace(tradeUrlToSend))
				{
					//throw new Exception("Please provide a valid TradeUrl.");
					Common.Log("Please provide a valid TradeUrl.", ConsoleColor.Red);
					return 0;
				}

				string tradeTokenJson = GetTradeOfferTokenJson(tradeUrlToSend);

				//TODO GENERATE SOMETHING ACTUAL
				string message = "SEND_TRADE_MESSAGE_INCLUDED";

				string partnerSteam64Id = steam64IdToSend.ToString();

				NameValueCollection data = TradeManager.GetTradeData(fromBot.SteamWeb.SessionId, partnerSteam64Id, message, itemsJson, tradeTokenJson);
				string url = "https://steamcommunity.com/tradeoffer/new/send";

				Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

				string response = "";

				var tradeUrlResponse = fromBot.SteamWeb.Fetch(tradeUrlToSend, "GET", null, true, "");

				//Error while opening the trade
				if (tradeUrlResponse.Contains("error_page_bg"))
				{

					string errorMessage = Common.GetBetween(tradeUrlResponse, "<div id=\"error_msg\">", "</div>").Trim();


					return 0;

					//throw new Exception(errorMessage);
				}

				//Get Login Status
				string loginStatus = Common.GetBetween(tradeUrlResponse, "Logged In',", ",").Replace("'", "").Trim();

				if (loginStatus.ToLower() == "false")
				{
					return 0;

					//Account lost session
					//throw new Exception("Unauthorized. The Bot has lost connection to Steam.");
				}


				// The number of days the trade will be placed on hold if the corresponding party is sending items in the trade.
				// We round up, thus even a single second of escrow will be shown to the user.
				//var g_daysMyEscrow = 0;
				//var g_daysTheirEscrow = 0;

				//<script>
				// The number of days the trade will be placed on hold if the corresponding party is sending items in the trade.
				// We round up, thus even a single second of escrow will be shown to the user.
				//var g_daysMyEscrow = 0;
				//var g_daysTheirEscrow = 3;
				//</script>

				string myEscrowDays = Common.GetBetween(tradeUrlResponse, "g_daysMyEscrow =", ";").Trim();
				string theirEscrowDays = Common.GetBetween(tradeUrlResponse, "g_daysTheirEscrow =", ";").Trim();

				if (myEscrowDays != "0")
				{
					return 0;

					//throw new Exception("Unauthorized. The Bot has a Escrow lock. Info: " + myEscrowDays);
				}

				//If you want to block users to trade to you if they have Escrow Lock
				//if (theirEscrowDays != "0" && assetsToReceive.Count > 0)//If we request any Items from the user we need to make sure he will not be Escrowed
				//	throw new Exception("Your Steam account is not secued by Mobile Authentification for longer than 7 days. Enable Mobile authentification to secure your accounts and also being able to sell skins on our Website. Read more here: http://store.steampowered.com/mobile");

				for (int i = 0; i < 5; i++)//Try sending trade up to 5 times(Steam doesnt often work on the first try)
				{
					response = fromBot.SteamWeb.Fetch(url, "POST", data, true, tradeUrlToSend);

					//something else returned and not a trade offer id
					if (response.Contains("tradeofferid"))
					{

						break;
					}
					else
					{
						if (response.Contains("26"))
						{
							Common.Log("Please reload the Inventory items or check you are not trying to trade any not tradable items. This might fix this issue.");
						}
						else
						{
							Common.Log(response);
						}

						if (i < 4)//dont sleep on last run
						{
							Thread.Sleep(1000 * (i + 1));
						}
					}
				}

				//If the response still doesnt have trade is then return
				if (!response.Contains("tradeofferid"))
				{
					return 0;
				}


				//AT THIS POINT TRADE HAS BEEN SEND

				bool anyConfirmation = false;
				try
				{
					TradeOfferCreationResponse_Root rootTradeOfferCreationResponse = JsonConvert.DeserializeObject<TradeOfferCreationResponse_Root>(response);
					tradeIdLong = Convert.ToInt64(rootTradeOfferCreationResponse.TradeOfferId);


					if (assetsToGive.Count() > 0)
					{
						//Bot sended items and needs to confirm the trades
						SteamGuardAccount steamGuardAccount = new SteamGuardAccount();

						steamGuardAccount.Session = new SessionData();
						steamGuardAccount.Session.SteamLogin = fromBot.SteamWeb.Token;
						steamGuardAccount.Session.SteamLoginSecure = fromBot.SteamWeb.TokenSecure;

						steamGuardAccount.Session.SessionID = fromBot.SteamWeb.SessionId;
						steamGuardAccount.Session.SteamID = fromBot.GetSteam64();

						steamGuardAccount.DeviceID = fromBot.DeviceId;
						steamGuardAccount.SharedSecret = fromBot.SharedSecret;
						steamGuardAccount.IdentitySecret = fromBot.IdentitySecret;

						Confirmation[] confirmations = null;

						for (int i = 0; i < 5; i++)
						{
							confirmations = steamGuardAccount.FetchConfirmations();
							if (confirmations.Length > 0)
							{
								anyConfirmation = true;
								break;
							}
							Thread.Sleep(500);
						}

						foreach (var confirmation in confirmations)
						{
							var isSuccess = steamGuardAccount.AcceptConfirmation(confirmation);

							if (!isSuccess)
							{
								steamGuardAccount.DenyConfirmation(confirmation);
							}
							else
							{
								Common.Log("Confirmed sended trade: " + tradeIdLong);
							}
						}
					}

					Thread newThread = new Thread(() => WaitForTradeToBeAccepted(Convert.ToInt64(rootTradeOfferCreationResponse.TradeOfferId), fromBot, unixTimestamp));
					newThread.Start();

					return tradeIdLong;
				}
				catch (Exception ex)
				{
					if (tradeIdLong != 0 || !anyConfirmation)
					{
						TradeManager.CancelTrade(tradeIdLong, fromBot);
					}

					return 0;
				}
			}
			catch (Exception ex)
			{
				//Critical Error
				TradeManager.CancelTrade(tradeIdLong, fromBot);
			}

			return 0;
		}

		/// <summary>
		/// Allows to run code depending on the current Trade status
		/// </summary>
		/// <param name="currentTradeState">Current status of the Trade</param>
		/// <param name="tradeId">Steam Trade Id</param>
		/// <param name="tradeBot">TradeBot info</param>
		private static void ProcessTrade(ETradeOfferState currentTradeState, long tradeId, TradeBot tradeBot)
		{
			switch (currentTradeState)
			{
				case ETradeOfferState.ETradeOfferError:
					break;//There was an error with retreiving the trade

				case ETradeOfferState.ETradeOfferNotFound:
					break;//Trade was not fuound

				//Not Active any more
				case ETradeOfferState.ETradeOfferStateInvalid:
				case ETradeOfferState.ETradeOfferStateCountered:
				case ETradeOfferState.ETradeOfferStateExpired:
				case ETradeOfferState.ETradeOfferStateCanceled:
				case ETradeOfferState.ETradeOfferStateDeclined:
				case ETradeOfferState.ETradeOfferStateInvalidItems:
				case ETradeOfferState.ETradeOfferStateCanceledBySecondFactor:
					//Set Status and close trade
					break;

				//Open Trades
				case ETradeOfferState.ETradeOfferStateActive:
				case ETradeOfferState.ETradeOfferStateCreatedNeedsConfirmation:
					//DO Nothing, wait until these trades change state
					break;

				//Accepted Trades
				case ETradeOfferState.ETradeOfferStateAccepted:
					//ProcessAcceptedTrade()
					return;


				//Trade in Escrow
				case ETradeOfferState.ETradeOfferStateInEscrow:
					//The Trade has been already saved. BotManager.CheckEscrowTrades() is now waiting for the items to get out of Escrow
					//ProcessAcceptedTradeInEscrow()
					return;

				default:
					//Cancel trade
					TradeManager.CancelTrade(tradeId, tradeBot);
					break;
			}
		}

		/// <summary>
		/// Checks after a trade has been created its status.
		/// </summary>
		/// <param name="tradeId">Steam Trade Id of the send Trade to check</param>
		/// <param name="bot">TradeBot info</param>
		/// <param name="unixTime">Time of the trade send</param>
		private static void WaitForTradeToBeAccepted(long tradeId, TradeBot bot, Int32 unixTime)
		{
			DateTimeOffset startTime = DateTimeOffset.Now;

			string urlFormat = "https://api.steampowered.com/IEconService/GetTradeOffers/v1/?key={0}&format=json&get_sent_offers=1&get_received_offers=0&get_descriptions=0&active_only=0&historical_only=0&time_historical_cutoff={1}";

			string url = string.Format(urlFormat, bot.ApiKey, unixTime);

			//Wait 5 min and after that Decline
			while (startTime.AddMinutes(5) > DateTimeOffset.Now)
			{
				Thread.Sleep(TimeSpan.FromSeconds(10));//Check All 10 Sec the trade status

				ETradeOfferState currentTradeState = TradeManager.GetTradeStatus(tradeId, bot, unixTime);
				ProcessTrade(currentTradeState, tradeId, bot);
			}

			TradeManager.CancelTrade(tradeId, bot);

			ETradeOfferState lastTradeState = TradeManager.GetTradeStatus(tradeId, bot, unixTime);
			ProcessTrade(lastTradeState, tradeId, bot);
		}
	}

	public class OfferAccessToken
	{
		[JsonProperty("trade_offer_access_token")]
		public string TradeOfferAccessToken { get; set; }
	}
}
