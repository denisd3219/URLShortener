using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Data;
using URLShortener.Models;
using URLShortener.Services.IDEncoder;

namespace URLShortener.Services.URLShortenerService
{
	public class URLShortenerService : IURLShortenerService
	{
		private readonly ApplicationDbContext _db;
		private readonly IIDEncoder _encoder;

		public URLShortenerService(ApplicationDbContext db, IIDEncoder encoder)
		{
			_db = db;
			_encoder = encoder;
		}

		public async Task<string> ShortenURL(string URL, string creatorId)
		{
			if(!ValidateURL(URL))
				return null;
			ShortenedURL sURL = new()
			{
				Original = URL,
				CreatorId = creatorId,
				Redirects = 0
			};
			_db.ShortenedURLs.Add(sURL);

			await _db.SaveChangesAsync();
			return _encoder.Encode(sURL.Id);
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
	}
}
