using System;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes;

public class Optimized : PrimalityU64Base
{
	const uint PERF_PIVOT = 805000000;

	protected override bool IsPrimeInternal(in ulong value)
		=> value < PERF_PIVOT
			? Polynomial.IsULongPrime(in value)
			: MillerRabin.IsPrimeInternal(in value);

	public readonly BigInt Big = new();

	public class BigInt : PrimalityBigIntBase
	{
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
