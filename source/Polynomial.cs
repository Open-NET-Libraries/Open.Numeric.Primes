using System.Numerics;

namespace Open.Numeric.Primes
{
	public static class Polynomial
	{
		const uint MAX_UINT_DIVISOR = 65536U;

		internal static bool IsPrimeInternal(uint value)
		{
			uint divisor = 6;
			while (divisor * divisor - 2 * divisor + 1 <= value)
			{
				if (value % (divisor - 1) == 0)
					return false;

				if (value % (divisor + 1) == 0)
					return false;

				divisor += 6;

				if (divisor > MAX_UINT_DIVISOR)
					return IsPrimeInternal(value, divisor);
			}
			return true;
		}

		const ulong MAX_ULONG_DIVISOR = 4294967296UL;

		internal static bool IsPrimeInternal(in ulong value, ulong divisor = 6)
		{
			if (divisor > MAX_ULONG_DIVISOR)
				return IsPrime(value, divisor);

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

		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public static bool IsPrime(uint value)
		{
			switch (value)
			{
				// 0 and 1 are not prime numbers
				case 0U:
				case 1U:
					return false;
				case 2U:
				case 3U:
					return true;

				default:

					if (value % 2 == 0 || value % 3 == 0)
						return false;

					return IsPrimeInternal(value);
			}
		}

		/// <summary>
		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public static bool IsPrime(in ulong value)
		{
			switch (value)
			{
				// 0 and 1 are not prime numbers
				case 0UL:
				case 1UL:
					return false;
				case 2UL:
				case 3UL:
					return true;

				default:

					if (value % 2 == 0 || value % 3 == 0)
						return false;

					return IsPrimeInternal(in value);
			}
		}

		/// <summary>
		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public static bool IsBigPrime(in BigInteger value)
		{
			if (value.IsZero)
				return false;

			return value.Sign == -1
				? value != BigInteger.MinusOne && primeCheck(BigInteger.Abs(value))
				: !value.IsOne && primeCheck(in value);

			static bool primeCheck(in BigInteger v)
			{
				if (v == BIG.TWO || v == BIG.THREE)
					return true;

				if (v.IsEven)
					return false;

				if (v <= ulong.MaxValue)
					return IsPrime((ulong)v);

				return v % 3 != 0
					   && IsPrime(v, 6);
			}
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

		public class U32 : PrimalityU32Base
		{
			// ReSharper disable once MemberHidesStaticFromOuterClass
			protected override bool IsPrimeInternal(uint value)
			=> Polynomial.IsPrimeInternal(value);
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
