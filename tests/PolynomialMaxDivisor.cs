﻿using System.Numerics;
using Xunit;

namespace Open.Numeric.Primes.Tests;

public static class PolynomialMaxDivisor
{
	[Fact]
	public static void PolynomialMaxUintDivisor()
	{
		// ulong.MaxValue = divisor * divisor - 2 * divisor + 1
		// ulong.MaxValue - 1 = divisor * divisor - 2 * divisor
		BigInteger max = uint.MaxValue;
		BigInteger divisor;
		for (BigInteger i = 1000; ; i++)
		{
			if (i * i - 2 * i + 1 > max)
			{
				divisor = i - 1;
				break;
			}
		}

		Assert.Equal(divisor, 65536);
	}

	[Fact]
	public static void PolynomialMaxUlongDivisor()
	{
		// ulong.MaxValue = divisor * divisor - 2 * divisor + 1
		// ulong.MaxValue - 1 = divisor * divisor - 2 * divisor
		BigInteger max = ulong.MaxValue;
		BigInteger divisor;
		for (BigInteger i = 4292747400; ; i++)
		{
			if (i * i - 2 * i + 1 > max)
			{
				divisor = i - 1;
				break;
			}
		}

		Assert.Equal(divisor, 4294967296);
	}
}
