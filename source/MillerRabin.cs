using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Open.Numeric.Primes
{
	public static class MillerRabin
	{
		static readonly ulong[] AR1 = { 2, 7, 61 };
		static readonly ulong[] AR2 = { 2, 3, 5, 7, 11, 13, 17 };
		static readonly ulong[] AR3 = { 2, 3, 5, 7, 11, 13, 17, 19, 23 };

		/* Based on: https://stackoverflow.com/questions/4236673/sample-code-for-fast-primality-testing-in-c-sharp#4236870 */
		public static bool IsPrime(in ulong n)
		{
			ReadOnlySpan<ulong> ar;
			if (n < 4759123141UL) ar = AR1;
			else if (n < 341550071728321UL) ar = AR2;
			else ar = AR3;

			var d = n - 1;
			var s = 0;
			while ((d & 1) == 0) { d >>= 1; s++; }
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < ar.Length; i++)
			{
				var a = Math.Min(n - 2, ar[i]);
				var now = Pow(a, d, in n);
				if (now == 1) continue;
				if (now == n - 1) continue;
				int j;
				for (j = 1; j < s; j++)
				{
					now = Mul(in now, in now, in n);
					if (now == n - 1) break;
				}
				if (j == s) return false;
			}
			return true;
		}

		static ulong Mul(in ulong a, in ulong b, in ulong mod)
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

		// ReSharper disable once UnusedMember.Local
		static BigInteger Mul(in BigInteger a, in BigInteger b, in BigInteger mod)
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

		static ulong Pow(ulong a, ulong p, in ulong mod)
		{
			retry:
			if (p == 0) return 1;
			if (p % 2 != 0) return Mul(Pow(a, p - 1, in mod), in a, in mod);
			a = Mul(in a, in a, in mod);
			p = p / 2;
			goto retry;
		}


		/* Based on: https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C.23 */
		public static bool IsProbablePrime(in BigInteger source, int certainty = 10)
		{
			if (source == 2 || source == 3)
				return true;
			if (source < 2 || source % 2 == 0)
				return false;

			var d = source - 1;
			var s = 0;

			while (d % 2 == 0)
			{
				d /= 2;
				s += 1;
			}

			// There is no built-in method for generating random BigInteger values.
			// Instead, random BigIntegers are constructed from randomly generated
			// byte arrays of the same length as the source.
			var rng = RandomNumberGenerator.Create();
			var bytes = new byte[source.ToByteArray().Length]; // .LongLength?

			for (var i = 0; i < certainty; i++)
			{
				BigInteger a;
				do
				{
					rng.GetBytes(bytes);
					a = new BigInteger(bytes);
				}
				while (a < 2 || a >= source - 2);

				var x = BigInteger.ModPow(a, d, source);
				if (x == 1 || x == source - 1)
					continue;

				for (var r = 1; r < s; r++)
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
			protected override bool IsPrimeInternal(in ulong value)
				=> MillerRabin.IsPrime(in value);
		}

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(in BigInteger value)
				=> IsProbablePrime(in value);
		}
	}
}
