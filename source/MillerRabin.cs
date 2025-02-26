﻿using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Open.Numeric.Primes;

/// <summary>
/// Miller-Rabin prime utility.
/// </summary>
public static class MillerRabin
{
	static readonly ReadOnlyMemory<ulong> AR1 = new ulong[] { 2, 7, 61 };
	static readonly ReadOnlyMemory<ulong> AR2 = new ulong[] { 2, 3, 5, 7, 11, 13, 17 };
	static readonly ReadOnlyMemory<ulong> AR3 = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
	static readonly ReadOnlyMemory<BigInteger> AR4 = new BigInteger[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

	/// <inheritdoc cref="Polynomial.IsPrime(in ulong)"/>
	public static bool IsPrime(in ulong value)
		=> value switch
		{
			// 0 and 1 are not prime numbers
			0UL or 1UL => false,
			2UL or 3UL => true,
			_ => (value & 1) != 0
				&& value % 3UL != 0
				&& IsPrimeInternal(in value),
		};

	/* Based on: https://stackoverflow.com/questions/4236673/sample-code-for-fast-primality-testing-in-c-sharp#4236870 */

	internal static bool IsPrimeInternal(in ulong value)
	{
		if (value > 9223372036854775783UL)
			return IsPrimeBigIntInternal(value);

		ReadOnlySpan<ulong> ar
			= value < 4_759_123_142UL
			? AR1.Span
			: value < 3_415_500_717_283_212UL
			? AR2.Span
			: AR3.Span;

		var d = value - 1;
		var s = 0;

		while ((d & 1) == 0) { d >>= 1; s++; }

		for (var i = 0; i < ar.Length; i++)
		{
			ref readonly var b = ref ar[i];
			var a = value - 2;
			var now = a > b
				? Pow(in b, in d, in value)
				: Pow(in a, in d, in value);

			if (now == 1) continue;
			if (now == value - 1) continue;
			int j;
			for (j = 1; j < s; j++)
			{
				now = Mul(in now, in now, in value);
				if (now == value - 1) break;
			}

			if (j == s) return false;
		}

		return true;
	}


	internal static bool IsPrimeBigIntInternal(in BigInteger value)
	{
		ReadOnlySpan<BigInteger> ar = AR4.Span;
		BigInteger d = value - 1;
		var s = 0;

		while ((d & 1) == 0) { d >>= 1; s++; }

		for (var i = 0; i < ar.Length; i++)
		{
			ref readonly var b = ref ar[i];
			var a = value - 2;
			var now = a > b
				? Pow(in b, in d, in value)
				: Pow(in a, in d, in value);

			if (now == 1) continue;
			if (now == value - 1) continue;
			int j;
			for (j = 1; j < s; j++)
			{
				now = Mul(in now, in now, in value);
				if (now == value - 1) break;
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

	static BigInteger Mul(in BigInteger a, in BigInteger b, in BigInteger mod)
	{
		int i;
		var now = BigInteger.Zero;
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

	static ulong Pow(in ulong a, in ulong p, in ulong mod)
		=> p switch
		{
			0 => 1,
			1 => Mul(1, in a, in mod),
			_ => (p & 1) == 0
			   ? PowX(Mul(in a, in a, in mod), p / 2, in mod)
			   : Mul(Pow(in a, p - 1, in mod), in a, in mod),
		};

	static BigInteger Pow(in BigInteger a, in BigInteger p, in BigInteger mod)
	{
		if (p.Equals(BigInteger.Zero)) return BigInteger.One;
		if (p.Equals(BigInteger.One)) return Mul(1, in a, in mod);

		// Passed simple tests.
		return (p & 1) == 0
			? PowX(Mul(in a, in a, in mod), p / 2, in mod)
			: Mul(Pow(in a, p - 1, in mod), in a, in mod);
	}

	static ulong PowX(ulong a, ulong p, in ulong mod)
	{
	retry:
		if (p == 0) return 1;
		if ((p & 1) != 0) return Mul(Pow(a, p - 1, in mod), in a, in mod);
		a = Mul(in a, in a, in mod);
		p /= 2;
		goto retry;
	}

	static BigInteger PowX(BigInteger a, BigInteger p, in BigInteger mod)
	{
	retry:
		if (p == 0) return 1;
		if ((p & 1) != 0) return Mul(Pow(a, p - 1, in mod), in a, in mod);
		a = Mul(in a, in a, in mod);
		p /= 2;
		goto retry;
	}

	/// <summary>
	/// Determines if a prime number is probably prime.
	/// </summary>
	public static bool IsProbablePrime(in BigInteger source, int certainty = 10)
		=> source.Sign == -1
			? source != BigInteger.MinusOne && IsProbablePrimeInternal(BigInteger.Abs(source), certainty)
			: !source.IsOne && IsProbablePrimeInternal(in source, certainty);

	/* Based on: https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C.23 */
	static bool IsProbablePrimeInternal(in BigInteger source, int certainty = 10)
	{
		if (source == Big.Two || source == Big.Three)
			return true;

		if (source.IsEven)
			return false;

		var d = source - 1;
		var s = 0;

		while ((d & 1) == 0)
		{
			d /= 2;
			s++;
		}

		// There is no built-in method for generating random BigInteger values.
		// Instead, random BigIntegers are constructed from randomly generated
		// byte arrays of the same length as the source.
		var rng = RandomNumberGenerator.Create();
		var len = source.ToByteArray().Length; // .LongLength?
		var bytes = new byte[len]; // .LongLength?

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

	/// <summary>
	/// Miller-Rabin prime discovery class for 64 bit unsigned integers (<see cref="ulong"/>).
	/// </summary>
	public class U64 : PrimalityU64Base
	{
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsPrimeInternal(in ulong value)
			=> MillerRabin.IsPrime(in value);
	}

	/// <summary>
	/// Miller-Rabin prime discovery class for <see cref="BigInteger"/>.
	/// </summary>
	public class BigInt : PrimalityBigIntBase
	{
		/// <inheritdoc />
		protected override bool IsPrimeInternal(in BigInteger value)
			=> IsProbablePrime(in value) && Polynomial.IsPrime(in value);
	}
}
