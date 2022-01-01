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
using URLShortener.Services.URLShortenerService;

namespace URLShortener.Controllers
{
	public class URLController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IURLShortenerService _shortener;

		public URLController(ApplicationDbContext db, IURLShortenerService shortener)
		{
			_db = db;
			_shortener = shortener;
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

			string domainName = $"{Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.Value}/";

			ShortenedURL sURL = await _db.ShortenedURLs.FirstOrDefaultAsync(url => url.Original == URLRequest.Original);
			if (sURL != null)
			{
				URLRequest.Short = domainName + sURL.Short;
				return View(URLRequest);
			}

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			sURL = await _shortener.ShortenURL(URLRequest.Original, userId);
			

			if (sURL != null)
				URLRequest.Short = domainName + sURL.Short;

			return View(URLRequest);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> UserURLs()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			ViewData["DomainName"] = $"{Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.Value}/";
			return View(await _db.ShortenedURLs.Where(url => url.CreatorId == userId).ToListAsync());
		}

		[HttpGet]
		public async Task<IActionResult> ShortRedirect(string shortURL)
		{
			if (shortURL == null)
				return NotFound();

			ShortenedURL sURL = await _db.ShortenedURLs.FirstOrDefaultAsync(url => url.Short == shortURL);

			if (sURL == null)
				return NotFound();

			sURL.Redirects++;
			_db.Update(sURL);
			await _db.SaveChangesAsync();

			return RedirectPermanent(sURL.Original);
		}
	}
}
