using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace URLShortener.Models
{
	public class ShortenedURL
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Original { get; set; }
		public string CreatorId { get; set; }
		public int Redirects { get; set; }
	}
}
