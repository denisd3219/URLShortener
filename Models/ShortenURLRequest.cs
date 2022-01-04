using System.ComponentModel.DataAnnotations;
using URLShortener.Validations;

namespace URLShortener.Models
{
	public class ShortenURLRequest
	{
		[Required(ErrorMessage = "URL is required")]
		[Display(Name = "Original URL")]
		[PingValidation(ErrorMessage = "URL is unreachable")]
		public string Original { get; set; }
		public string Short { get; set; }
	}
}
