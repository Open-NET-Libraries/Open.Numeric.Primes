using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Open.Numeric.Primes
{
	public static class MillerRabin
	{
		static readonly ulong[] AR1 = new ulong[] { 2, 7, 61 };
		static readonly ulong[] AR2 = new ulong[] { 2, 3, 5, 7, 11, 13, 17 };
		static readonly ulong[] AR3 = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };

		/* Based on: https://stackoverflow.com/questions/4236673/sample-code-for-fast-primality-testing-in-c-sharp#4236870 */
		public static bool IsPrime(ulong n)
		{
			ulong[] ar;
			if (n < 4759123141UL) ar = AR1;
			else if (n < 341550071728321UL) ar = AR2;
			else ar = AR3;

			ulong d = n - 1;
			int s = 0;
			while ((d & 1) == 0) { d >>= 1; s++; }
			for (int i = 0; i < ar.Length; i++)
			{
				ulong a = Math.Min(n - 2, ar[i]);
				ulong now = Pow(a, d, n);
				if (now == 1) continue;
				if (now == n - 1) continue;
				int j;
				for (j = 1; j < s; j++)
				{
					now = Mul(now, now, n);
					if (now == n - 1) break;
				}
				if (j == s) return false;
			}
			return true;
		}

		static ulong Mul(ulong a, ulong b, ulong mod)
		{
			int i;
			ulong now = 0;
			for (i = 63; i >= 0; i--) if (((a >> i) & 1) == 1) break;
			for (; i >= 0; i--)
			{
				now <<= 1;
				while (now > mod) now -= mod;
				if (((a >> i) & 1) == 1) now += b;
				while (now > mod) now -= mod;
			}
			return now;
		}

		static BigInteger Mul(BigInteger a, BigInteger b, BigInteger mod)
		{
			int i;
			BigInteger now = 0;
			for (i = 63; i >= 0; i--) if (((a >> i) & 1) == 1) break;
			for (; i >= 0; i--)
			{
				now <<= 1;
				while (now > mod) now -= mod;
				if (((a >> i) & 1) == 1) now += b;
				while (now > mod) now -= mod;
			}
			return now;
		}

		static ulong Pow(ulong a, ulong p, ulong mod)
		{
			if (p == 0) return 1;
			if (p % 2 == 0) return Pow(Mul(a, a, mod), p / 2, mod);
			return Mul(Pow(a, p - 1, mod), a, mod);
		}


		/* Based on: https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C.23 */
		public static bool IsProbablePrime(this BigInteger source, int certainty = 10)
		{
			if (source == 2 || source == 3)
				return true;
			if (source < 2 || source % 2 == 0)
				return false;

			BigInteger d = source - 1;
			int s = 0;

			while (d % 2 == 0)
			{
				d /= 2;
				s += 1;
			}

			// There is no built-in method for generating random BigInteger values.
			// Instead, random BigIntegers are constructed from randomly generated
			// byte arrays of the same length as the source.
			RandomNumberGenerator rng = RandomNumberGenerator.Create();
			byte[] bytes = new byte[source.ToByteArray().Length]; // .LongLength?
			BigInteger a;

			for (int i = 0; i < certainty; i++)
			{
				do
				{
					rng.GetBytes(bytes);
					a = new BigInteger(bytes);
				}
				while (a < 2 || a >= source - 2);

				BigInteger x = BigInteger.ModPow(a, d, source);
				if (x == 1 || x == source - 1)
					continue;

				for (int r = 1; r < s; r++)
				{
					x = BigInteger.ModPow(x, 2, source);
					if (x == 1)
						return false;
					if (x == source - 1)
						break;
				}

				if (x != source - 1)
					return false;
			}

			return true;
		}


		public class U64 : PrimalityU64Base
		{
			protected override bool IsPrimeInternal(ulong value)
			{
				return MillerRabin.IsPrime(value);
			}
		}
	}
}
