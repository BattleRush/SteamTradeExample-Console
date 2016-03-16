﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamBotExample.Steam2FA
{
	public class SteamWebAuth
	{
		/// <summary>
		/// Perform a mobile login request
		/// </summary>
		/// <param name="url">API url</param>
		/// <param name="method">GET or POST</param>
		/// <param name="data">Name-data pairs</param>
		/// <param name="cookies">current cookie container</param>
		/// <returns>response body</returns>
		public static string MobileLoginRequest(string url, string method, NameValueCollection data = null, CookieContainer cookies = null, NameValueCollection headers = null)
		{
			return Request(url, method, data, cookies, headers, APIEndpoints.COMMUNITY_BASE + "/mobilelogin?oauth_client_id=DE45CD61&oauth_scope=read_profile%20write_profile%20read_client%20write_client");
		}
		public static string Fetch(string url, string method, NameValueCollection data = null, CookieContainer cookies = null, string referer = "")
		{
			return Request(url, method, data, cookies, null, referer);
		}
		public static string Request(string url, string method, NameValueCollection data = null, CookieContainer cookies = null, NameValueCollection headers = null, string referer = APIEndpoints.COMMUNITY_BASE)
		{
			string query = (data == null ? string.Empty : string.Join("&", Array.ConvertAll(data.AllKeys, key => String.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(data[key])))));
			if (method == "GET")
			{
				url += (url.Contains("?") ? "&" : "?") + query;
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;
			request.Accept = "text/javascript, text/html, application/xml, text/xml, */*";
			request.UserAgent = "Mozilla/5.0 (Linux; U; Android 4.1.1; en-us; Google Nexus 4 - 4.1.1 - API 16 - 768x1280 Build/JRO03S) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			request.Referer = referer;

			if (headers != null)
			{
				request.Headers.Add(headers);
			}

			if (cookies != null)
			{
				request.CookieContainer = cookies;
			}

			if (method == "POST")
			{
				request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
				request.ContentLength = query.Length;

				StreamWriter requestStream = new StreamWriter(request.GetRequestStream());
				requestStream.Write(query);
				requestStream.Close();
			}

			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						return null;
					}

					using (StreamReader responseStream = new StreamReader(response.GetResponseStream()))
					{
						string responseData = responseStream.ReadToEnd();

						return responseData;
					}
				}
			}
			catch (WebException ex)
			{
				using (WebResponse response = ex.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data2 = response.GetResponseStream())
					using (var reader = new StreamReader(data2))
					{
						string text = reader.ReadToEnd();
						return text;
					}
				}
			}
		}
	}

	// JSON Classes
	public class GetRsaKey
	{
		public bool success { get; set; }

		public string publickey_mod { get; set; }

		public string publickey_exp { get; set; }

		public string timestamp { get; set; }
	}

	public class SteamResult
	{
		public bool success { get; set; }

		public string message { get; set; }

		public bool captcha_needed { get; set; }

		public string captcha_gid { get; set; }

		public bool emailauth_needed { get; set; }

		public string emailsteamid { get; set; }

	}
}
