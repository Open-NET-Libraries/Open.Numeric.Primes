using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

#if NET7_0_OR_GREATER
	protected virtual IEnumerable<T> ValidPrimeTests(in T startingAt)
		=> Candidates.StartingAt(in startingAt);
#else
	protected abstract IEnumerable<T> ValidPrimeTests(in T startingAt);
#endif

	/// <summary>
	/// Returns an enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="value">
	/// Allows for skipping ahead any integer
	/// before checking for inclusive and subsequent primes.
	/// </param>
	public virtual IEnumerable<T> StartingAt(in T value)
		=> ValidPrimeTests(in value)
#if NET7_0_OR_GREATER
			.Where(IsPrime);
#else
			.Where(v => IsPrime(v));
#endif

	/// <summary>
	/// Returns key-value pairs of every prime starting where the key is the index (starting at 1) of the set.
	/// </summary>
	/// <remarks>The first entry is always {Key=1, Value=2}.</remarks>
	public abstract IEnumerable<KeyValuePair<T, T>> Indexed();

	/// <summary>
	/// Returns a parallel enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="startingAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
	/// <param name="degreeOfParallelism">The optional maximum degree of parallelism.</param>
	/// <returns>An ordered parallel enumerable of primes.</returns>
	public virtual ParallelQuery<T> InParallel(in T startingAt, int? degreeOfParallelism = null)
	{
		Debug.Assert(!degreeOfParallelism.HasValue || degreeOfParallelism >= 0);

		var tests = ValidPrimeTests(in startingAt)
			.AsParallel().AsOrdered();

		if (degreeOfParallelism > 1)
			tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

		return tests
#if NET7_0_OR_GREATER
			.Where(IsPrime);
#else
			.Where(v => IsPrime(v));
#endif
	}

	/// <inheritdoc cref="InParallel(in T, int?)"/>
	public ParallelQuery<T> InParallel(int? degreeOfParallelism = null)
		=> InParallel(default!, degreeOfParallelism);

	protected virtual bool IsFactorable(in T value)
		=> !IsPrime(in value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0 or 1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public IEnumerable<T> Factors(T value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => !value.Equals(v))
			: Factors(value);

	/// <inheritdoc />
	public abstract IEnumerator<T> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}