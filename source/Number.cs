using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

#if NET7_0_OR_GREATER
/// <summary>
/// A small set of commonly reused numbers of type <typeparamref name="T"/>.
/// </summary>
internal static class Number<T>
	where T : notnull, INumber<T>
{
	internal static readonly T Two = T.One + T.One;
	internal static readonly T Three = T.One + Two;
	internal static readonly T Six = Three + Three;
}
#endif

/// <summary>
/// A useful set of prime detection functions.
/// Unique overloads for certain number types including BigInteger in order to ensure efficiency and compiler optimizations.
/// </summary>
public static class Number
{
	/// <summary>
	/// Validates if a number is prime.
	/// </summary>
	/// <param name="value">Value to verify.</param>
	/// <returns><see langword="true"/> if the provided value is a prime number; otherwise <see langword="false"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in ulong value)
		=> Prime.Numbers.IsPrime(in value);

	// Overload for use with simplified delegate use.
	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(ulong value)
		=> IsPrime(in value);

	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(int value)
		=> Polynomial.IsPrime((uint)Math.Abs(value));

	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(uint value)
		=> Polynomial.IsPrime(value);

	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in BigInteger value)
		=> Prime.Numbers.Big.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in long value)
		=> Prime.Numbers.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(ulong)"/>
	public static bool IsPrime(in double value)
	{
		if (value % 1d != 0d)
			return false;

		var abs = Math.Abs(value);
		return value <= ulong.MaxValue
			? IsPrime((ulong)abs)
			: IsPrime((BigInteger)abs);
	}

	/// <inheritdoc cref="IsPrime(ulong)"/>
	public static bool IsPrime(in decimal value)
	{
		switch (value)
		{
			case decimal.Zero:
			case decimal.One:
			case decimal.MinusOne:
				return false;
		}

		if (value % 1 != 0)
			return false;

		var v = Math.Abs(value);
		return v <= ulong.MaxValue
			? IsPrime((ulong)v)
			: IsPrime((BigInteger)v);
	}
}
