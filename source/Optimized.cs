using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes
{
	public class Optimized : PrimalityU64Base
	{
		protected override bool IsPrimeInternal(in ulong value)
		{
			return value < 805000000
				? Polynomial.IsPrimeInternal(in value)
				: MillerRabin.IsPrime(in value);
		}

		public readonly BigInt Big = new BigInt();

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(in BigInteger value)
			{
				if (value <= ulong.MaxValue)
					return MillerRabin.IsPrime((ulong)value);

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
