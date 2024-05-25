namespace Open.Numeric.Primes;

/// <summary>
/// Polynomial prime utility.
/// </summary>
public static class Polynomial
{
	/// <summary>
	/// Returns <see langword="true"/> if the <paramref name="value"/> is prime.
	/// </summary>
	/// <param name="value">The value to validate.</param>
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

				if (value % 2U == 0U || value % 3U == 0U)
					return false;

				return IsUIntPrime(value);
		}
	}

	/// <inheritdoc cref="IsPrime(uint)"/>
	public static bool IsPrime(int value)
		=> IsPrime((uint)Math.Abs(value));

	/// <inheritdoc cref="IsPrime(uint)"/>
	public static bool IsPrime(in long value)
		=> IsPrime((ulong)Math.Abs(value));

	/// <inheritdoc cref="IsPrime(uint)"/>
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

				if (value % 2UL == 0UL || value % 3UL == 0UL)
					return false;

				return IsULongPrime(in value);
		}
	}

	/// <inheritdoc cref="IsPrime(uint)"/>
	public static bool IsPrime(in BigInteger value)
	{
		return !value.IsZero
			&& (value.Sign == -1
				? value != BigInteger.MinusOne && primeCheck(BigInteger.Abs(value))
				: !value.IsOne && primeCheck(in value));

		[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
		static bool primeCheck(in BigInteger v)
		{
			Debug.Assert(v > BigInteger.Zero);

			if (v == Big.Two || v == Big.Three)
				return true;

			if (v.IsEven)
				return false;

			if (v <= ulong.MaxValue)
				return IsPrime((ulong)v);

			return v % Big.Three != BigInteger.Zero
				&& IsBigIntPrime(in v, 6);
		}
	}

	internal static bool IsUIntPrime(uint value)
	{
		const uint MAX_UINT_DIVISOR = 65536U;

		uint divisor = 6;
		while (divisor * divisor - 2 * divisor + 1 <= value)
		{
			if (value % (divisor - 1) == 0)
				return false;

			if (value % (divisor + 1) == 0)
				return false;

			divisor += 6;

			if (divisor > MAX_UINT_DIVISOR)
				return IsULongPrime(value, divisor);
		}

		return true;
	}

	internal static bool IsULongPrime(in ulong value, ulong divisor = 6)
	{
		const ulong MAX_ULONG_DIVISOR = 4294967296UL;

		if (divisor > MAX_ULONG_DIVISOR)
			return IsBigIntPrime(value, (BigInteger)divisor);

		while (divisor * divisor - 2UL * divisor + 1UL <= value)
		{
			if (value % (divisor - 1UL) == 0)
				return false;

			if (value % (divisor + 1UL) == 0)
				return false;

			divisor += 6UL;

			if (divisor > MAX_ULONG_DIVISOR)
				return IsBigIntPrime(value, (BigInteger)divisor);
		}

		return true;
	}

	internal static bool IsBigIntPrime(in BigInteger value, BigInteger divisor)
	{
		Debug.Assert(value > BigInteger.Zero);
		Debug.Assert(divisor > BigInteger.Zero);

		while (divisor * divisor - Big.Two * divisor + BigInteger.One <= value)
		{
			if (value % (divisor - BigInteger.One) == 0)
				return false;

			if (value % (divisor + BigInteger.One) == 0)
				return false;

			divisor += Big.Six;
		}

		return true;
	}

#if NET7_0_OR_GREATER
	/// <inheritdoc cref="IsPrime(uint)"/>
	[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
	public static bool IsPrime<T>(in T value)
		where T : INumber<T>
	{
		return T.IsInteger(value)
			&& (T.Sign(value) == -1
				? IsPrimeCore(T.Abs(value))
				: IsPrimeCore(in value));

		static bool IsPrimeCore(in T value)
		{
			Debug.Assert(T.IsPositive(value));

			if (value < Number<T>.Two)
				return false;

			if (value - Number<T>.Two <= T.One)
				return true;

			return !T.IsEvenInteger(value)
				&& !T.IsZero(value % Number<T>.Three)
				&& IsTPrime(in value);
		}
	}

	internal static bool IsTPrime<T>(in T value, T divisor)
		where T : notnull, INumber<T>
	{
		Debug.Assert(T.IsPositive(value));
		Debug.Assert(T.IsPositive(divisor));

		while (divisor * divisor - Number<T>.Two * divisor + T.One <= value)
		{
			if (T.IsZero(value % (divisor - T.One)))
				return false;

			if (T.IsZero(value % (divisor + T.One)))
				return false;

			divisor += Number<T>.Six;
		}

		return true;
	}

	internal static bool IsTPrime<T>(in T value)
		where T : notnull, INumber<T>
		=> IsTPrime(in value, Number<T>.Six);
#endif

	/// <summary>
	/// Polynomial utility for <see cref="uint"/>.
	/// </summary>
	public class U32 : PrimalityU32Base
	{
		/// <inheritdoc />
		// ReSharper disable once MemberHidesStaticFromOuterClass
		protected override bool IsPrimeInternal(uint value)
			=> IsUIntPrime(value);

		/// <inheritdoc />
		protected override bool IsPrimeInternal(in uint value)
			=> IsUIntPrime(value);
	}

	/// <summary>
	/// Polynomial utility for <see cref="ulong"/>.
	/// </summary>
	public class U64 : PrimalityU64Base
	{
		/// <inheritdoc />
		// ReSharper disable once MemberHidesStaticFromOuterClass
		protected override bool IsPrimeInternal(in ulong value)
			=> IsULongPrime(in value);
	}

	/// <summary>
	/// Polynomial utility for <see cref="BigInteger"/>.
	/// </summary>
	public class BigInt : PrimalityBigIntBase
	{
		/// <inheritdoc />
		protected override bool IsPrimeInternal(in BigInteger value)
			=> IsBigIntPrime(in value, 6UL);
	}
}
