namespace Open.Numeric.Primes;

/// <inheritdoc/>
public abstract class PrimalityBigIntBase : PrimalityIntegerBase<BigInteger>
{
	/// <inheritdoc />
	public override IEnumerator<BigInteger> GetEnumerator()
		=> StartingAt(BigInteger.One).GetEnumerator();

	/// <inheritdoc />
	protected override IEnumerable<BigInteger> ValidPrimeTests(in BigInteger startingAt)
		=> Candidates.StartingAt(startingAt);

	/// <inheritdoc />
	public new IEnumerable<KeyValuePair<BigInteger, BigInteger>> Indexed()
	{
		var count = BigInteger.Zero;
		foreach (var n in this)
		{
			count++;
			yield return new KeyValuePair<BigInteger, BigInteger>(count, n);
		}
	}

	/// <inheritdoc />
	public override IEnumerable<BigInteger> Factors(BigInteger value)
	{
		if (value == BigInteger.Zero)
			goto exit;

		yield return value.Sign == -1
			? BigInteger.MinusOne
			: BigInteger.One;

		if (value.IsOne || value == BigInteger.MinusOne)
			yield break;

		value = BigInteger.Abs(value);
		var last = BigInteger.One;

		// For larger numbers, a quick prime check can prevent large iterations.
		if (!IsFactorable(in value))
			goto exit;

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

	exit:
		yield return value;
	}

#if !NET7_0_OR_GREATER
	/// <inheritdoc />
	public override BigInteger Next(in BigInteger after)
		=> StartingAt(after + BigInteger.One).First();
#endif

	/// <inheritdoc cref="PrimalityBase{T}.Next(in T)" />
	public BigInteger Next(float after)
		=> after < 0
			? Next((BigInteger)Math.Floor(after))
			: Next((BigInteger)after);

	/// <inheritdoc cref="PrimalityBase{T}.Next(in T)" />
	public BigInteger Next(in double after)
		=> after < 0
			? Next((BigInteger)Math.Floor(after))
			: Next((BigInteger)after);

	/// <inheritdoc />
	public sealed override bool IsPrime(in BigInteger value)
	{
		return !value.IsZero
			&& (value.Sign == -1
			? value != BigInteger.MinusOne && primeCheck(BigInteger.Abs(value))
			: !value.IsOne && primeCheck(in value));

		[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
		bool primeCheck(in BigInteger v)
		{
			Debug.Assert(v > BigInteger.Zero);

			if (v == Big.Two || v == Big.Three)
				return true;

			if (v.IsEven)
				return false;

			if (v <= ulong.MaxValue)
				return Prime.Numbers.IsPrime((ulong)v);

			return v % Big.Three != 0 && IsPrimeInternal(in v);
		}
	}
}
