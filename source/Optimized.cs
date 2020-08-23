using System;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes
{
	public class Optimized : PrimalityU64Base
	{
		const uint PERF_PIVOT = 805000000;

		protected override bool IsPrimeInternal(in ulong value)
			=> value < PERF_PIVOT
				? Polynomial.IsPrimeInternal(Convert.ToUInt32(value))
				: MillerRabin.IsPrime(in value);

		public readonly BigInt Big = new BigInt();

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(in BigInteger value)
			{
				if (value <= ulong.MaxValue)
				{
					if (value < PERF_PIVOT) Polynomial.IsPrimeInternal(Convert.ToUInt32(value));
					return MillerRabin.IsPrime((ulong)value);
				}

				return MillerRabin.IsProbablePrime(in value) && Polynomial.IsPrime(in value, 6);

				// Lucas-Selfridge here? :(
			}

			/// <inheritdoc />
			public override ParallelQuery<BigInteger> InParallel(in BigInteger staringAt, ushort? degreeOfParallelism = null)
			{
				if (staringAt >= ulong.MaxValue)
					return base.InParallel(in staringAt, degreeOfParallelism);

				return (new Optimized())
					.InParallel((ulong)staringAt, degreeOfParallelism)
					.Select(u => new BigInteger(u))
					.Concat(base.InParallel(ulong.MaxValue, degreeOfParallelism));
			}
		}
	}
}
