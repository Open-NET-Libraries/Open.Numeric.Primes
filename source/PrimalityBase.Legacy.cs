﻿#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;

namespace Open.Numeric.Primes;

public abstract partial class PrimalityBase<T>
	where T : notnull, IEquatable<T>, IComparable<T>
{
	/// <inheritdoc cref="IsPrime(T)"/>
	public abstract bool IsPrime(in T value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	public abstract IEnumerable<T> Factors(T value);

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
	public abstract T Next(in T after);
}
#endif