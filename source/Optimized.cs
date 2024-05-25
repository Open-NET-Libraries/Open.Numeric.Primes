namespace Open.Numeric.Primes;

/// <summary>
/// A prime utility that choses the optimal algorithm depending the size of the number.
/// </summary>
public class Optimized : PrimalityU64Base
{
	const uint PERF_PIVOT = 805000000;

	/// <inheritdoc />
	protected override bool IsPrimeInternal(in ulong value)
		=> value < PERF_PIVOT
			? Polynomial.IsULongPrime(in value)
			: MillerRabin.IsPrimeInternal(in value);

	/// <summary>
	/// The static instance of <see cref="BigInt"/>.
	/// </summary>
	public readonly BigInt Big = new();

	/// <summary>
	/// A prime utility that choses the optimal algorithm depending the size of the number for <see cref="BigInteger"/>.
	/// </summary>
	public class BigInt : PrimalityBigIntBase
	{
		/// <inheritdoc />
		protected override bool IsPrimeInternal(in BigInteger value)
			=> value > ulong.MaxValue
				? MillerRabin.IsProbablePrime(in value) && Polynomial.IsBigIntPrime(in value, 6)
				: value < PERF_PIVOT
				? Polynomial.IsUIntPrime(Convert.ToUInt32(value))
				: MillerRabin.IsPrime((ulong)value);
		// Lucas-Selfridge here? :(

		/// <inheritdoc />
		public override ParallelQuery<BigInteger> InParallel(in BigInteger startingAt, int? degreeOfParallelism = null)
			=> startingAt >= ulong.MaxValue
				? base.InParallel(in startingAt, degreeOfParallelism)
				: Prime.Numbers
					.InParallel((ulong)startingAt, degreeOfParallelism)
					.Select(u => new BigInteger(u))
					.Concat(base.InParallel(ulong.MaxValue, degreeOfParallelism));
	}
}

#if NET7_0_OR_GREATER
/// <summary>
/// A prime utility that choses the optimal algorithm depending the type of the number
/// and can process new <see cref="INumber{TSelf}"/> types.
/// </summary>
public class Optimized<T> : PrimalityBase<T>
	where T : notnull, INumber<T>
{
	/// <inheritdoc />
	public override IEnumerator<T> GetEnumerator()
		=> StartingAt(Number<T>.Two).GetEnumerator();

	/// <inheritdoc />
	public override bool IsPrime(in T value)
	{
		if (value is uint ui)
			return Number.IsPrime(ui);
		if (value is ulong ul)
			return Number.IsPrime(in ul);
		if(value is int i)
			return Number.IsPrime(i);
		if(value is long l)
			return Number.IsPrime(in l);
		if (value is double d)
			return Number.IsPrime(in d);
		if (value is float f)
			return Number.IsPrime(f);
		if (value is decimal dec)
			return Number.IsPrime(dec);
		if (value is BigInteger bi)
			return Number.IsPrime(in bi);

		// No specific type defined? Use Numerics.
		return base.IsPrime(in value);
	}

	/// <inheritdoc />
	protected override bool IsPrimeInternal(in T value)
		=> Polynomial.IsPrime(in value);
}
#endif