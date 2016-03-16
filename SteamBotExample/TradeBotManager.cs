using SteamBotExample.Steam2FA;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamBotExample
{
	public class TradeBot
	{
		public string Username = "";
		public string DisplayName = "";
		private string Password = "";
		public SteamWeb SteamWeb;
		private string MyUniqueId = "";
		private string MyUserNonce = "";

		SteamClient steamClient;
		CallbackManager manager;
		SteamUser steamUser;
		SteamFriends steamFriends;

		public bool isRunning = false;
		private byte[] SentryHash = null;
		public bool IsLoggedIn = false;

		public string ApiKey = "";
		private string TwoFactorCode = "";

		public string DeviceId = null;
		public string SharedSecret = null;
		public string IdentitySecret = null;

		private bool SetPlayingStatus = true;//set to false if you only want to set status to Online instead of "Playing"

		public TradeBot(BotInfo botInfo, byte[] sentryHash)
		{
			Username = botInfo.Username;
			Password = botInfo.Password;
			SentryHash = sentryHash;
			ApiKey = botInfo.ApiKey;

			DeviceId = botInfo.DeviceId;
			SharedSecret = botInfo.SharedSeecret;
			IdentitySecret = botInfo.IdentitySecret;	

			InitBot();
		}

		private void InitBot()
		{
			try
			{
				SteamWeb = new SteamWeb();
				steamClient = new SteamClient();
				manager = new CallbackManager(steamClient);
				steamUser = steamClient.GetHandler<SteamUser>();
				steamFriends = steamClient.GetHandler<SteamFriends>();

				new Callback<SteamClient.ConnectedCallback>(OnConnect, manager);
				new Callback<SteamClient.DisconnectedCallback>(OnDisconnected, manager);
				new Callback<SteamClient.CMListCallback>(OnCMListCallback, manager);

				new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, manager);
				new Callback<SteamUser.LoggedOffCallback>(OnLoggedOff, manager);
				new Callback<SteamUser.LoginKeyCallback>(OnLoginKeyCallback, manager);
				new Callback<SteamUser.UpdateMachineAuthCallback>(OnGuardAuth, manager);
				new Callback<SteamUser.AccountInfoCallback>(OnAccountInfoCallback, manager);

				isRunning = true;

				SteamGuardAccount steamGuardAccount = new SteamGuardAccount();
				steamGuardAccount.DeviceID = DeviceId;
				steamGuardAccount.SharedSecret = SharedSecret;
				string twoFactorCode = steamGuardAccount.GenerateSteamGuardCode();

				TwoFactorCode = twoFactorCode;


				steamClient.Connect();

				Log("Connectiong to Steam Network...");

				Thread t = new Thread(WhileLoop);
				t.Start();

				UserWebLogOn();
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}

		private void WhileLoop()
		{
			while (isRunning)
			{
				try
				{
					manager.RunWaitCallbacks(TimeSpan.FromMilliseconds(100));
				}
				catch (Exception ex)
				{

				}
			}
		}
		
		public SteamID GetSteam64()
		{
			return steamClient.SteamID;
		}

		private void Log(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			string prefix = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
			Console.WriteLine(prefix + message);
		}

		private void OnCMListCallback(SteamClient.CMListCallback callback)
		{
			//foreach (var server in callback.Servers)
			//{
			//	Log("CMListCallback: " + server.Address);		
			//}
		}

		private void OnConnect(SteamClient.ConnectedCallback callback)
		{
			if (callback.Result != EResult.OK)
			{
				Log("callback.Result: " + callback.Result);
				isRunning = false;
				return;
			}
			Log("Connected to SteamNetwork: " + callback.Result);


			try
			{

				Thread.Sleep(2000);
				steamUser.LogOn(new SteamUser.LogOnDetails
				{
					Username = Username,
					Password = Password,
					AuthCode = "",//No auth code needed, Bots should have 2FA on
					SentryFileHash = SentryHash,//No sentryfile needed, Bots should have 2FA on
					TwoFactorCode = TwoFactorCode
				});
			}
			catch (Exception ex)
			{
				Log("OnConnect: " + ex.ToString());
			}

		}

		private void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			Log("Disconected JobID " + callback.JobID.ToString());

			if (!IsLoggedIn)
			{
				Thread.Sleep(1000);
				Log("Disconected... Reconnecting...");
				steamClient.Connect();
			}
		}

		private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{
			Log("Login status: " + callback.Result);
			if (callback.Result == EResult.AccountLogonDenied)
			{
				Log("SteamGuard needed: " + callback.Result);
				return;
			}

		
			if (callback.Result != EResult.OK)
			{
				Log("Connect canceled: " + callback.Result);
				isRunning = false;
				return;
			}

			MyUserNonce = callback.WebAPIUserNonce;
			Thread.Sleep(500);

			Log("Login successful: " + callback.Result);
		}

		private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Log("LogOff: " + callback.Result);
		}

		private void OnLoginKeyCallback(SteamUser.LoginKeyCallback callback)
		{
			Log("OnLoginKeyCallback: " + callback.UniqueID.ToString());
			MyUniqueId = callback.UniqueID.ToString();
		}

		private void OnGuardAuth(SteamUser.UpdateMachineAuthCallback callback)
		{
			Log("Updatin sentry...");

			byte[] sentryHash = CryptoHelper.SHAHash(callback.Data);
			SentryHash = sentryHash;

			//File.WriteAllBytes("sentry.bin", callback.Data);

			steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
			{
				JobID = callback.JobID,
				FileName = callback.FileName,
				BytesWritten = callback.BytesToWrite,
				FileSize = callback.Data.Length,
				Offset = callback.Offset,
				Result = EResult.OK,
				LastError = 0,
				OneTimePassword = callback.OneTimePassword,
				SentryFileHash = sentryHash
			});

			Log("Done SteamGuard Auth");

		}
		private void OnAccountInfoCallback(SteamUser.AccountInfoCallback callback)
		{
			Log("OnAccountInfoCallback" + callback.PersonaName);

			DisplayName = callback.PersonaName;

			//TEST IF NEEDED
			steamFriends.SetPersonaState(EPersonaState.Online);


			if (SetPlayingStatus)
			{
				var id = 15444025664222527488;
				string message = "Trading Bot Test";
				var gamePlaying = new SteamKit2.ClientMsgProtobuf<SteamKit2.Internal.CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

				gamePlaying.Body.games_played.Add(new SteamKit2.Internal.CMsgClientGamesPlayed.GamePlayed
				{
					game_id = new GameID(id),
					game_extra_info = message + " " + DateTimeOffset.Now.ToString(),//Include starting date
				});
				steamClient.Send(gamePlaying);
			}
		}


		public void UserWebLogOn()
		{
			int maxTries = 300;
			int count = 0;
			do
			{
				try
				{
					count++;
					if (count > maxTries)
					{
						//if (checkIsRunning)
						isRunning = false;
						return;
					}

					if (MyUniqueId != "" && MyUserNonce != "")
					{
						IsLoggedIn = SteamWeb.Authenticate(MyUniqueId, steamClient, MyUserNonce);
					}

					Thread.Sleep(100);
				}
				catch (Exception ex)
				{

				}
			} while (!IsLoggedIn);

			//IS AUTHENTIFICATED!!!
		}
	}
}
