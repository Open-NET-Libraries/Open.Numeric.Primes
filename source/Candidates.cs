using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes;

/// <summary>
/// A collection of static methods for acquiring all prime candidates.
/// </summary>
public static class Candidates
{
	/// <summary>
	/// An increasing list of all odd numbers excluding one, but also including two,
	/// beginning with the <paramref name="value"/> provided.
	/// </summary>
	public static IEnumerable<uint> StartingAt(uint value)
	{
		const uint zero = 0U;
		const uint one = 1U;
		const uint two = 2U;
		const uint three = 3U;
		const uint max = uint.MaxValue;

		if (value > two)
		{
			if (value % two == zero)
				value++;
		}
		else
		{
			yield return two;
			value = three;
		}

		for (; value < max - one; value += two)
			yield return value;
	}

	/// <inheritdoc cref="StartingAt(uint)"/>
	public static IEnumerable<ulong> StartingAt(ulong n)
	{
		const ulong zero = 0U;
		const ulong one = 1U;
		const ulong two = 2U;
		const ulong three = 3U;
		const ulong max = ulong.MaxValue;

		if (n > two)
		{
			if (n % two == zero)
				n++;
		}
		else
		{
			yield return two;
			n = three;
		}

		for (; n < max - one; n += two)
			yield return n;
	}

	private static IEnumerable<BigInteger> ValidPrimeTests(BigInteger startingAt)
	{
		Debug.Assert(startingAt.Sign != -1);

		var sign = startingAt.Sign;
		if (sign == 0) sign = 1;
		var n = BigInteger.Abs(startingAt);

		if (n > Big.Two)
		{
			if (n % Big.Two == 0)
				n++;
		}
		else
		{
			yield return sign * Big.Two;
			n = Big.Three;
		}

		while (true)
		{
			yield return sign * n;
			n += Big.Two;
		}
	}

	/// <inheritdoc cref="StartingAt(uint)"/>
	/// <remarks>Results will retain the sign of the <paramref name="value"/>.</remarks>
	public static IEnumerable<BigInteger> StartingAt(
#if NETSTANDARD2_0
		in
#endif
		BigInteger value)
		=> value.Sign == -1
			? ValidPrimeTests(-value).Select(e => -e)
			: ValidPrimeTests(value);

#if NET7_0_OR_GREATER
	private static IEnumerable<TN> ValidPrimeTestsPos<TN>(TN n)
		where TN : INumber<TN>
	{
		Debug.Assert(!TN.IsNegative(n));

		var two = Number<TN>.Two;

		if (n > two)
		{
			if (TN.IsEvenInteger(n))
				n++;
		}
		else
		{
			yield return two;
			n = Number<TN>.Three;
		}

		while (true)
		{
			yield return n;

			try
			{
				n = checked(n + two);
			}
			catch (System.OverflowException)
			{
				break;
			}
		}
	}

	private static IEnumerable<TN> ValidPrimeTestsNeg<TN>(TN n)
		where TN : INumber<TN>
	{
		Debug.Assert(!TN.IsPositive(n));

		var two = -Number<TN>.Two;

		if (n < two)
		{
			if (TN.IsEvenInteger(n))
				n--;
		}
		else
		{
			yield return two;
			n = -Number<TN>.Three;
		}

		while (true)
		{
			yield return n;

			try
			{
				n = checked(n + two);
			}
			catch (System.OverflowException)
			{
				break;
			}
		}
	}

	/// <inheritdoc cref="StartingAt(BigInteger)"/>
	public static IEnumerable<T> StartingAt<T>(in T value)
		where T : notnull, INumber<T>
	{
		Debug.Assert(value is not null);
		return !T.IsInteger(value)
			? throw new System.ArgumentException("Must be an integer.", nameof(value))
			: T.Sign(value) == -1
			? ValidPrimeTestsNeg(value)
			: ValidPrimeTestsPos(value);
	}

	/// <inheritdoc cref="StartingAt(BigInteger)"/>
	public static IEnumerable<T> StartingAtFloat<T>(in T value)
		where T : notnull, IFloatingPoint<T>
	{
		Debug.Assert(value is not null);
		return T.Sign(value) == -1
			? ValidPrimeTestsNeg(T.Floor(value))
			: ValidPrimeTestsPos(T.Ceiling(value));
	}
#endif
}
