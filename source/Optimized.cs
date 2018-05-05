using System.Numerics;

namespace Open.Numeric.Primes
{
	public class Optimized : PrimalityU64Base
	{
		protected override bool IsPrimeInternal(ulong value)
		{
			if (value < 805000000) // Aproximate value where Polynomial prime detection stops being better than MillerRabin.
				return Polynomial.IsPrimeInternal(value);

			return MillerRabin.IsPrime(value);
		}

		public readonly BigInt Big = new BigInt();

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(BigInteger value)
			{
				if (value <= ulong.MaxValue)
					return MillerRabin.IsPrime((ulong)value);

				if (!MillerRabin.IsProbablePrime(value))
					return false; // false is the only deterministic result.

				// Lucas-Selfridge here? :(

				return Polynomial.IsPrime(value, 6);
			}
		}
	}
}
