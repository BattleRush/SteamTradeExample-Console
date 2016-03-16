﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.Steam2FA
{
	public static class APIEndpoints
	{
		public const string STEAMAPI_BASE = "https://api.steampowered.com";
		public const string COMMUNITY_BASE = "https://steamcommunity.com";
		public const string TWO_FACTOR_BASE = STEAMAPI_BASE + "/ITwoFactorService/%s/v0001";
		public static string TWO_FACTOR_TIME_QUERY = TWO_FACTOR_BASE.Replace("%s", "QueryTime");
	}
}
