using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

public abstract partial class PrimalityBase<T> : IEnumerable<T>
{
	// Find IsPrime and more defined in other partials.

	// Exists for easy delegate use.
	/// <summary>
	/// Returns <see langword="true"/> if the value provided is prime.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsPrime(T value) => IsPrime(in value);

	protected abstract bool IsPrimeInternal(in T value);

	/// <summary>
	/// Returns an enumerable of key-value pairs that will iterate every prime starting at the starting value where the key is the count (index starting at 1) of the set.
	/// So the first entry is always {Key=1, Value=2}.
	/// </summary>
	public abstract IEnumerable<KeyValuePair<T, T>> Indexed();

	/// <summary>
	/// Returns a parallel enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
	/// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
	/// <returns>An ordered parallel enumerable of primes.</returns>
	public abstract ParallelQuery<T> InParallel(in T staringAt, ushort? degreeOfParallelism = null);

	/// <inheritdoc cref="InParallel(in T, ushort?)"/>
	/// <summary>
	/// Returns a parallel enumerable that will iterate every prime.
	/// </summary>
	public abstract ParallelQuery<T> InParallel(ushort? degreeOfParallelism = null);

	protected virtual bool IsFactorable(in T value)
		=> !IsPrime(in value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0 or 1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public IEnumerable<T> Factors(in T value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => !value.Equals(v))
			: Factors(value);

	/// <inheritdoc />
	public abstract IEnumerator<T> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}