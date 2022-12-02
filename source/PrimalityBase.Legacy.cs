#if !NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

public abstract partial class PrimalityBase<T>
	where T : notnull, IEquatable<T>, IComparable<T>
{
	/// <inheritdoc cref="IsPrime(T)"/>
	public abstract bool IsPrime(in T value);

	public abstract IEnumerable<T> Factors(T value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract IEnumerable<T> ValidPrimeTests(in T staringAt)

	/// <summary>
	/// Returns an enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="value">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
	public virtual IEnumerable<T> StartingAt(in T value)
		=> ValidPrimeTests(in value)
			.Where(v => IsPrime(v));

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
	public abstract T Next(in T after);
}
#endif