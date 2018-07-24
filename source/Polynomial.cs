using System.Numerics;

namespace Open.Numeric.Primes
{
	public static class Polynomial
	{
		const ulong MAX_ULONG_DIVISOR = 25043747693UL;

		internal static bool IsPrimeInternal(in ulong value)
		{
			ulong divisor = 6;
			while (divisor * divisor - 2 * divisor + 1 <= value)
			{
				if (value % (divisor - 1) == 0)
					return false;

				if (value % (divisor + 1) == 0)
					return false;

				divisor += 6;

				if (divisor > MAX_ULONG_DIVISOR)
					return IsPrime(value, divisor);
			}
			return true;
		}

		public static bool IsPrime(in ulong value)
		{
			switch (value)
			{
				// 0 and 1 are not prime numbers
				case 0:
				case 1:
					return false;
				case 2:
				case 3:
					return true;

				default:

					if (value % 2 == 0 || value % 3 == 0)
						return false;

					return IsPrimeInternal(in value);
			}
		}

		public static bool IsBigPrime(in BigInteger value)
		{
			if (value.IsZero)
				return false;

			bool primeCheck(in BigInteger v)
			{
				if (v == BIG.TWO || v == BIG.THREE)
					return true;

				if (v.IsOne || v.IsEven)
					return false;

				if (v <= ulong.MaxValue)
					return IsPrime((ulong)v);

				return v % 3 != 0
					   && IsPrime(v, 6);
			}

			return value.Sign == -1
				? primeCheck(BigInteger.Abs(value))
				: primeCheck(in value);
		}

		internal static bool IsPrime(in BigInteger value, BigInteger divisor)
		{
			while (divisor * divisor - 2 * divisor + 1 <= value)
			{
				if (value % (divisor - 1) == 0)
					return false;

				if (value % (divisor + 1) == 0)
					return false;

				divisor += 6;
			}

			return true;
		}

		public class U64 : PrimalityU64Base
		{
			// ReSharper disable once MemberHidesStaticFromOuterClass
			protected override bool IsPrimeInternal(in ulong value)
			=> Polynomial.IsPrimeInternal(in value);
		}

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(in BigInteger value)
			=> Polynomial.IsPrime(in value, 6UL);
		}
	}

}
