using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamBotExample
{
	public class BotInfo
	{
		public bool Check2FA()
		{
			bool missing2FAInfo = false;
			if (string.IsNullOrWhiteSpace(DeviceId)
				|| string.IsNullOrWhiteSpace(SharedSeecret)
				|| string.IsNullOrWhiteSpace(IdentitySecret))
			{
				if (string.IsNullOrWhiteSpace(DeviceId))
				{
					Common.Log("No DeviceId found!");
				}
				if (string.IsNullOrWhiteSpace(SharedSeecret))
				{
					Common.Log("No SharedSeecret found!");
				}
				if (string.IsNullOrWhiteSpace(IdentitySecret))
				{
					Common.Log("No IdentitySecret found!");
				}

				missing2FAInfo = true;
			}
			return missing2FAInfo;
		}
		public string Username { get; set; }
		public string Password { get; set; }
		public string ApiKey { get; set; }

		//2FA
		public string DeviceId { get; set; }
		public string SharedSeecret { get; set; }
		public string IdentitySecret { get; set; }
	}
}
