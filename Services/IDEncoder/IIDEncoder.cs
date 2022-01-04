using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URLShortener.Services.IDEncoder
{
	public interface IIDEncoder
	{
		string Encode(int i);
		int Decode(string s);
	}
}
