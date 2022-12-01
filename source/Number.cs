using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

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

	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(uint value)
		=> Prime.Numbers.IsPrime(value);

	// Overload for use with simplified delegate use.
	/// <inheritdoc cref="IsPrime(ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(ulong value)
		=> Prime.Numbers.IsPrime(in value);

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
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (value % 1 != 0)
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
