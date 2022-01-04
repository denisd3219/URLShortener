using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace URLShortener.Validations
{
    public class PingValidation : ValidationAttribute
    {
        public override bool IsValid(object URL)
        {
			if (!Uri.TryCreate((string)URL, UriKind.Absolute, out Uri urlCheck))
				return false;

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
