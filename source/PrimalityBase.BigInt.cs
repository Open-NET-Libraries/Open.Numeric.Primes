using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Open.Numeric.Primes;

public abstract class PrimalityBigIntBase : PrimalityBase<BigInteger>
{
	protected override IEnumerable<BigInteger> ValidPrimeTests(BigInteger staringAt)
	{
		var sign = staringAt.Sign;
		if (sign == 0) sign = 1;
		var n = BigInteger.Abs(staringAt);

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
		// ReSharper disable once IteratorNeverReturns
	}

	/// <inheritdoc />
	public override IEnumerator<BigInteger> GetEnumerator()
		=> StartingAt(BigInteger.One).GetEnumerator();

	protected IEnumerable<BigInteger> ValidPrimeTests()
		=> ValidPrimeTests(BigInteger.One);

	/// <inheritdoc />
	public override IEnumerable<KeyValuePair<BigInteger, BigInteger>> Indexed()
	{
		var count = BigInteger.Zero;
		foreach (var n in this)
		{
			count++;
			yield return new KeyValuePair<BigInteger, BigInteger>(count, n);
		}
	}

	/// <inheritdoc />
	public override ParallelQuery<BigInteger> InParallel(in BigInteger staringAt, ushort? degreeOfParallelism = null)
	{
		var testsBig = ValidPrimeTests(staringAt)
			.AsParallel().AsOrdered();

		if (degreeOfParallelism.HasValue)
			testsBig = testsBig.WithDegreeOfParallelism(degreeOfParallelism.Value);

		return testsBig.Where(IsPrime);
	}

	/// <inheritdoc />
	public override ParallelQuery<BigInteger> InParallel(ushort? degreeOfParallelism = null)
		=> InParallel(in Big.Two, degreeOfParallelism);

	/// <inheritdoc />
	public override IEnumerable<BigInteger> Factors(BigInteger value)
	{
		if (value != BigInteger.Zero)
		{
			yield return value.Sign == -1
				? BigInteger.MinusOne
				: BigInteger.One;

			if (value.IsOne || value == BigInteger.MinusOne)
				yield break;

			value = BigInteger.Abs(value);
			var last = BigInteger.One;

			// For larger numbers, a quick prime check can prevent large iterations.
			if (IsFactorable(in value))
			{
				foreach (var p in this)
				{
					var stop = value / last; // The list of possibilities shrinks for each test.
					if (p > stop) break; // Exceeded possibilities? 
					while ((value % p) == 0)
					{
						value /= p;
						yield return p;
						if (value.IsOne) yield break;
					}

					last = p;
				}
			}
		}

		yield return value;
	}

	/// <inheritdoc />
	public override BigInteger Next(in BigInteger after)
		=> StartingAt(after + BigInteger.One).First();

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.</param>
	/// <returns>The next prime after the number provided.</returns>
	public BigInteger Next(float after)
		=> Next((BigInteger)after);

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.</param>
	/// <returns>The next prime after the number provided.</returns>
	public BigInteger Next(double after)
		=> Next((BigInteger)after);

	/// <inheritdoc />
	public sealed override bool IsPrime(in BigInteger value)
	{
		if (value.IsZero)
			return false;

		[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
		bool primeCheck(in BigInteger v)
		{
			if (v == Big.Two || v == Big.Three)
				return true;

			if (v.IsEven)
				return false;

			if (v <= ulong.MaxValue)
				return Prime.Numbers.IsPrime((ulong)v);

			return v % Big.Three != 0 && IsPrimeInternal(in v);
		}

		return value.Sign == -1
			? value != BigInteger.MinusOne && primeCheck(BigInteger.Abs(value))
			: !value.IsOne && primeCheck(in value);
	}

	protected abstract bool IsPrimeInternal(in BigInteger value);
}
