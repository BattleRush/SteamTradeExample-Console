using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample
{
	public static class Common
	{
		public static long GetSystemUnixTime()
		{
			return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static byte[] HexStringToByteArray(string hex)
		{
			int hexLen = hex.Length;
			byte[] ret = new byte[hexLen / 2];
			for (int i = 0; i < hexLen; i += 2)
			{
				ret[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return ret;
		}
		public static string GetBetween(string strSource, string startString, string endString, int startPos = 0)
		{
			try
			{
				int posIndex;
				int endIndex;
				int length = startString.Length;

				string strResult;

				strResult = String.Empty;
				posIndex = strSource.IndexOf(startString, startPos);
				endIndex = strSource.IndexOf(endString, posIndex + length);
				if (posIndex > -1 && endIndex > -1)
					strResult = strSource.Substring(posIndex + length, endIndex - (posIndex + length));

				return strResult;
			}
			catch (Exception)
			{
				return "";
			}
		}
		public static void Log(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			string prefix = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
			Console.WriteLine(prefix + message);
		}
	}
}
