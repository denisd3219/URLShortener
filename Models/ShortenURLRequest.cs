using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace URLShortener.Models
{
	public class ShortenURLRequest
	{
		[Required(ErrorMessage = "URL is required")]
		public string Original { get; set; }
		public string Short { get; set; }
	}
}
