using System.Threading.Tasks;
using URLShortener.Models;

namespace URLShortener.Services.URLShortenerService
{
	public interface IURLShortenerService
	{
		Task<string> ShortenURL(string URL, string creatorId);
	}
}