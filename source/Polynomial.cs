using System.Numerics;

namespace Open.Numeric.Primes
{
	public static class Polynomial
	{
		const ulong MAX_ULONG_DIVISOR = 25043747693UL;

		internal static bool IsPrimeInternal(ulong value)
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

		public static bool IsPrime(ulong value)
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

					return IsPrimeInternal(value);
			}
		}

		public static bool IsPrime(BigInteger value)
		{
			if (value.IsZero || value.IsOne)
				return false;
			value = BigInteger.Abs(value);
			if (value.IsOne)
				return false;

			if (value <= ulong.MaxValue)
				return IsPrime((ulong)value);

			return value % 2 != 0
				&& value % 3 != 0
				&& IsPrime(value, 6);
		}

		internal static bool IsPrime(BigInteger value, BigInteger divisor)
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
			protected override bool IsPrimeInternal(ulong value)
			{
				return Polynomial.IsPrimeInternal(value);
			}
		}

		public class BigInt : PrimalityBigIntBase
		{
			protected override bool IsPrimeInternal(BigInteger value)
			{
				return Polynomial.IsPrime(value, 6UL);
			}
		}
	}

}
