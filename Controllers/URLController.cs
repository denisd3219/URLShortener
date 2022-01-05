using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Data;
using URLShortener.Models;
using URLShortener.Services.IDEncoder;
using URLShortener.Services.URLShortenerService;
using X.PagedList;

namespace URLShortener.Controllers
{
	public class URLController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IURLShortenerService _shortener;
		private readonly IIDEncoder _encoder;

		public URLController(ApplicationDbContext db, IURLShortenerService shortener, IIDEncoder encoder)
		{
			_db = db;
			_shortener = shortener;
			_encoder = encoder;
		}

		[Authorize]
		[HttpGet]
		public IActionResult Shorten()
		{
			ShortenURLRequest url = new();
			return View(url);
		}

		[Authorize]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public async Task<IActionResult> Shorten(ShortenURLRequest URLRequest)
		{
			if (!ModelState.IsValid)
				return View(URLRequest);
			ViewData["DomainName"] = $"{Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.Value}/";

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			URLRequest.Short = await _shortener.ShortenURL(URLRequest.Original, userId);
			
			return View(URLRequest);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> UserURLs(int pageNum)
		{
			if (pageNum < 1)
				pageNum = 1;
			ViewBag.DomainName = $"{Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.Value}/";
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			return View(await _db.ShortenedURLs
				.Where(url => url.CreatorId == userId)
				.ToPagedListAsync(pageNum, 20));
		}

		[HttpGet]
		public async Task<IActionResult> ShortRedirect(string shortURL)
		{
			if (shortURL == null)
				return NotFound();

			ShortenedURL sURL = await _db.ShortenedURLs.FindAsync(_encoder.Decode(shortURL));

			if (sURL == null)
				return NotFound();

			sURL.Redirects++;
			_db.Update(sURL);
			await _db.SaveChangesAsync();

			return Redirect(sURL.Original);
		}
	}
}
