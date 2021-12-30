using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Data;
using URLShortener.Models;

namespace URLShortener.Services.URLShortenerService
{
	public class URLShortenerService : IURLShortenerService
	{
		private readonly ApplicationDbContext _db;

		public URLShortenerService(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<ShortenedURL> ShortenURL(string URL, string creatorId)
		{
			if(!ValidateURL(URL))
				return null;

			string s = ShortenString(creatorId + URL);
			if (s == null)
				return null;

			ShortenedURL sURL = new()
			{
				Original = URL,
				Short = s,
				CreatorId = creatorId,
				Redirects = 0
			};

			_db.ShortenedURLs.Add(sURL);
			await _db.SaveChangesAsync();

			return sURL;
		}

		private bool ValidateURL(string URL)
		{
			Uri urlCheck = new(URL);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
			request.Timeout = 10000;
			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private string ShortenString(string input)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] utfBytes = new UTF8Encoding().GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(utfBytes);

				Random random = new();
	
				int tries = 0;
				while (tries < 32)
				{
					StringBuilder sb = new();
					for (int i = 0; i < 3; i++)
						sb.Append(hashBytes[random.Next(0, hashBytes.Length)].ToString("X2"));

					string s = sb.ToString();
					if (!_db.ShortenedURLs.Where(url => url.Short == s).Any())
						return s;

					tries++;
				}
				return null;
			}
		}
	}
}
