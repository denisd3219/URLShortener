using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.IDEncoder
{
	public class Base36IDEncoder : IIDEncoder
	{
        private static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly int Base = Alphabet.Length;

        public string Encode(int i)
        {
            if (i == 0) return Alphabet[0].ToString();

            StringBuilder sb = new();
			for (; i > 0; i /= Base)
                sb.Append(Alphabet[i % Base]);

            sb.Reverse();
            return sb.ToString();
        }

        public int Decode(string s)
        {
            var i = 0;
            foreach (var c in s)
                i = (i * Base) + Alphabet.IndexOf(c);
            return i;
        }
    }
    static class SBReverse
    {
        public static void Reverse(this StringBuilder sb)
        {
            char t;
            int end = sb.Length - 1;
            int start = 0;

            while (end - start > 0)
            {
                t = sb[end];
                sb[end] = sb[start];
                sb[start] = t;
                start++;
                end--;
            }
        }
    }
}
