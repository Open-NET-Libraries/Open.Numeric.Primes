using System;
using System.Linq;
using System.Numerics;

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
				: new Optimized()
					.InParallel((ulong)startingAt, degreeOfParallelism)
					.Select(u => new BigInteger(u))
					.Concat(base.InParallel(ulong.MaxValue, degreeOfParallelism));
	}
}
