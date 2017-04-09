﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace VSTranslator.Translation
{
	internal static class Utils
	{
		public static string GetHttpResponse(String url, String data)
		{
			WebClient client = new WebClient();
			client.Headers.Add("user-agent", "User-Agent: Mozilla/5.0");
			client.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
			byte[] rawData = client.UploadData(url, Encoding.UTF8.GetBytes(data));
			return Encoding.UTF8.GetString(rawData);
		}
				
		public static string CreateQuerystring(Dictionary<string, string> args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string name in args.Keys)
			{
				sb.Append(HttpUtility.UrlEncode(name));
				sb.Append("=");
				sb.Append(HttpUtility.UrlEncode(args[name]));
				sb.Append("&");
			}			
			return sb.ToString(0, Math.Max(sb.Length - 1, 0));
		}
	}
}
