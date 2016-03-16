using Newtonsoft.Json;
using SteamBotExample.Steam2FA;
using SteamBotExample.SteamApi.IEconItems_730.GetPlayerItems;
using SteamBotExample.SteamApi.SteamWeb;
using SteamBotExample.SteamApi.SteamWeb.Trade;
using SteamBotExample.Trade;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamBotExample
{
	class Program
	{
		#region Configuration
		private const string SteamAPIKey = "MY_STEAM_API_KEY";

		private const int AppId = 730;//TF2 = 440; Dota2 = 570; CSGO = 730;
		private const int ContextId = 2;//Can change depending on the AppId 

		private const bool LoadSteamAPIInventory = true;//Steam Api is very unreliable and often doesnt respond properly (only needed for float values)

		//Trade Bot (Make sure 2FA is already enabled)
		//Read https://github.com/geel9/SteamAuth how to add Mobile auth with the SteamGuardAccount class
		private static BotInfo BotInfo = new BotInfo()
		{
			Username = "",
			Password = "",
			ApiKey = "",

			//2FA
			DeviceId = "",
			SharedSeecret = "",
			IdentitySecret = "",
		};

		private static long Steam64IdToSend = 1;//Set Steam64Id
		private static string UserTradeUrl = "https://steamcommunity.com/tradeoffer/new/?partner=PARTNERID&token=TOKEN";

		#endregion

		static void Main(string[] args)
		{
			Common.Log("Started");

			bool isMissingAny2FAInfo = BotInfo.Check2FA();
			if (isMissingAny2FAInfo)
			{
				SetUp2FA();
			}

			steamWebInventory_Root inventoryItems = GetUserInventory(Steam64IdToSend);

			//Login into a bot
			//Make Sure to have already 2FA on your account activated

			TradeBot myTradeBot = new TradeBot(BotInfo, null);

			if (myTradeBot.isRunning)
			{
				//Take 5 last Items from Users Inventory
				//Check to remove non tradable items(description.Value.Tradable)

				//Loop trough inventoryItems.RgInventory to decide which items you want
				//See GetUserInventory() how to get the description
				List<long> itemsFromUsers = inventoryItems.RgInventory.Skip(5).Take(5).Select(i => Convert.ToInt64(i.Value.Id)).ToList();

				long steamTradeId = TradeManager.SendTrade(myTradeBot, Steam64IdToSend, UserTradeUrl, new List<long>(), itemsFromUsers, AppId, ContextId);
				if (steamTradeId > 0)
				{
					Common.Log("Success sending a trade(" + steamTradeId + ")");
				}
				else
				{
					Common.Log("Failed to send a trade", ConsoleColor.Red);
				}
			}
			else
			{
				Common.Log("Trade bot is not LoggedIn", ConsoleColor.Red);
			}

			Console.ReadLine();
		}

		private static void SetUp2FA()
		{
			UserLogin userLogin = new UserLogin(BotInfo.Username, BotInfo.Password);
			LoginResult loginResult = userLogin.DoLogin();

			if (loginResult == LoginResult.NeedEmail)
			{
				Common.Log("Please enter the Email code: ");

				string emailCode = Console.ReadLine();

				Common.Log("Code: " + emailCode);

				userLogin.EmailCode = emailCode;
				loginResult = userLogin.DoLogin();
			}
			else
			{
				Common.Log("2FA Missing. " + loginResult);
			}

			if (loginResult == LoginResult.LoginOkay)
			{
				AuthenticatorLinker authLinker = new AuthenticatorLinker(userLogin.Session);

				var result = authLinker.AddAuthenticator();

				if (result == AuthenticatorLinker.LinkResult.AwaitingFinalization)
				{
					if (string.IsNullOrWhiteSpace(authLinker.PhoneNumber))
					{
						Common.Log("No Phone number has been set. Please enter your phone number:");

						string phoneNumber = Console.ReadLine();

						authLinker.PhoneNumber = phoneNumber;
					}

					Common.Log("--------------------------");
					Common.Log("!!! WARNING !!! Please save the following codes somewhere. If you fail to save them before calling FinalizeAddAuthenticator() you might be restricted from your account.", ConsoleColor.Red);
					Common.Log("Current Mobile phone number: " + authLinker.PhoneNumber, ConsoleColor.Red);
					Common.Log("SharedSecret: " + authLinker.LinkedAccount.SharedSecret, ConsoleColor.DarkRed);
					Common.Log("RevocationCode: " + authLinker.LinkedAccount.RevocationCode, ConsoleColor.DarkRed);
					Common.Log("DeviceID: " + authLinker.LinkedAccount.DeviceID, ConsoleColor.DarkRed);
					Common.Log("TokenGID: " + authLinker.LinkedAccount.TokenGID, ConsoleColor.DarkRed);
					Common.Log("Secret1: " + authLinker.LinkedAccount.Secret1, ConsoleColor.DarkRed);
					Common.Log("SerialNumber: " + authLinker.LinkedAccount.SerialNumber, ConsoleColor.DarkRed);
					Common.Log("IdentitySecret: " + authLinker.LinkedAccount.IdentitySecret, ConsoleColor.DarkRed);
					Common.Log("--------------------------");

					Common.Log("Please enter the SMS code: ");

					string smsCode = Console.ReadLine();

					Common.Log("SMS Code: " + smsCode);

					var finalizeResult = authLinker.FinalizeAddAuthenticator(smsCode);
					if (finalizeResult != AuthenticatorLinker.FinalizeResult.Success)
					{
						//Set botinfo
						BotInfo.DeviceId = authLinker.LinkedAccount.DeviceID;
						BotInfo.SharedSeecret = authLinker.LinkedAccount.SharedSecret;
						BotInfo.IdentitySecret = authLinker.LinkedAccount.IdentitySecret;

						Common.Log("2FA finalized.");

						Common.Log(finalizeResult.ToString(), ConsoleColor.DarkYellow);
					}
				}
			}
		}
		private static steamWebInventory_Root GetUserInventory(long steam64Id)
		{
			bool loadingSuccessUserInventory = false;
			bool loadingSuccess = false;

			string steamWebInventoryUrlFormat = "https://steamcommunity.com/profiles/{0}/inventory/json/{1}/{2}/";
			string steamWebInventoryUrl = string.Format(steamWebInventoryUrlFormat, steam64Id, AppId, ContextId);

			getPlayerItems_Root steamApiInventory = new getPlayerItems_Root();
			steamWebInventory_Root steamWebInventory = new steamWebInventory_Root();

			Common.Log("Loading Inventory for User: " + steam64Id);

			using (WebClient client = new WebClient())
			{
				for (int i = 0; i < 100; i++)//Try up to 100 times(because Steam doesnt work always)
				{
					Common.Log("Loading try " + i, ConsoleColor.Cyan);

					try
					{
						if (!loadingSuccessUserInventory)
						{
							string htmlCode = client.DownloadString(steamWebInventoryUrl);
							steamWebInventory = JsonConvert.DeserializeObject<steamWebInventory_Root>(htmlCode);

							if (steamWebInventory.Success == false)
							{
								Common.Log("Loading SteamWeb Inventory failed", ConsoleColor.Yellow);
								Common.Log(htmlCode, ConsoleColor.Yellow);
							}
							else
							{
								Common.Log("Loading SteamWeb Inventory successful", ConsoleColor.Green);
								loadingSuccessUserInventory = true;
							}
						}

						if (LoadSteamAPIInventory)
						{
							steamApiInventory = GetPlayerItems(steam64Id);
							Common.Log("Loading SteamApi Inventory successful", ConsoleColor.Green);
						}

						loadingSuccess = true;
						break;
					}
					catch (Exception ex)
					{
						Common.Log(ex.Message, ConsoleColor.Red);
						Thread.Sleep(50);
					}
				}

				if (loadingSuccess == true)
				{
					//Output current users Items
					//You can loop here trough the Inventory and get float values from the ApiInventory and return these
					foreach (var item in steamWebInventory.RgInventory)
					{
						try
						{
							//ClassId_InstanceId -> Description key
							var description = steamWebInventory.RgDescriptions.SingleOrDefault(i => i.Key == (item.Value.ClassId + "_" + item.Value.InstanceId));
							Console.ForegroundColor = ConsoleColor.White;
							Console.WriteLine("[" + steamWebInventory.RgInventory.ToList().IndexOf(item) + "] " + description.Value.MarketHashName + " (" + item.Value.Id + ")");
						}
						catch (Exception ex)
						{
							Common.Log(ex.Message, ConsoleColor.DarkYellow);
						}
					}

					Common.Log(steam64Id + " has " + steamWebInventory.RgInventory.Count + " item(s) in their Invetory", ConsoleColor.Green);
					Common.Log(steam64Id + " has " + steamApiInventory.Result.Items.Count() + " item(s) in their API Invetory", ConsoleColor.Green);
				}
				else
				{
					Common.Log("Steam didnt respond with a valid reply. Please try later.", ConsoleColor.Red);
					
				}
			}
			return steamWebInventory;
		}

		private static getPlayerItems_Root GetPlayerItems(long steamId64)
		{
			string urlFormat = "https://api.steampowered.com/IEconItems_730/GetPlayerItems/v0001/?key={0}&steamid={1}";
			string finalUrl = string.Format(urlFormat, SteamAPIKey, steamId64);

			using (WebClient client = new WebClient())
			{
				string response = client.DownloadString(finalUrl);
				return JsonConvert.DeserializeObject<getPlayerItems_Root>(response);
			}
		}
	}
}