using System;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes;

public class Optimized : PrimalityU64Base
{
	const uint PERF_PIVOT = 805000000;

	protected override bool IsPrimeInternal(in ulong value)
		=> value < PERF_PIVOT
			? Polynomial.IsPrimeInternal(Convert.ToUInt32(value))
			: MillerRabin.IsPrime(in value);

	public readonly BigInt Big = new();

	public class BigInt : PrimalityBigIntBase
	{
		protected override bool IsPrimeInternal(in BigInteger value)
			=> value > ulong.MaxValue
				? MillerRabin.IsProbablePrime(in value) && Polynomial.IsPrime(in value, 6)
				: value < PERF_PIVOT
				? Polynomial.IsPrimeInternal(Convert.ToUInt32(value))
				: MillerRabin.IsPrime((ulong)value);
				// Lucas-Selfridge here? :(

		/// <inheritdoc />
		public override ParallelQuery<BigInteger> InParallel(in BigInteger staringAt, ushort? degreeOfParallelism = null)
			=> staringAt >= ulong.MaxValue
				? base.InParallel(in staringAt, degreeOfParallelism)
				: new Optimized()
					.InParallel((ulong)staringAt, degreeOfParallelism)
					.Select(u => new BigInteger(u))
					.Concat(base.InParallel(ulong.MaxValue, degreeOfParallelism));
	}
}
