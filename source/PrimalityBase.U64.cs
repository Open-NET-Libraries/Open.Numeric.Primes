using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

/// <inheritdoc />
public abstract class PrimalityU64Base : PrimalityIntegerBase<ulong>
{
	/// <inheritdoc />
	protected override IEnumerable<ulong> ValidPrimeTests(in ulong startingAt = 2U)
		=> Candidates.StartingAt(startingAt);

	/// <inheritdoc />
	public override IEnumerator<ulong> GetEnumerator()
		=> StartingAt(2UL).GetEnumerator();

	/// <summary>
	/// Returns an enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="value">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.  Passing a negative number here will produce a negative set of prime numbers.</param>
	public IEnumerable<long> StartingAt(long value)
	{
		var absStart = (ulong)Math.Abs(value);
		var selection = StartingAt(absStart).TakeWhile(v => v < long.MaxValue);

		return value < 0
#if NET7_0_OR_GREATER
			? selection.Select(ConvertInt64Negative)
			: selection.Select(Convert.ToInt64);
#else
			? selection.Select(e => ConvertInt64Negative(e))
			: selection.Select(e => Convert.ToInt64(e));
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static long ConvertInt64Negative(ulong value)
			=> -Convert.ToInt64(value);
	}

	/// <inheritdoc />
	public override IEnumerable<KeyValuePair<ulong, ulong>> Indexed()
	{
		ulong count = 0L;
		foreach (var n in this)
		{
			count++;
			yield return new KeyValuePair<ulong, ulong>(count, n);
		}
	}

	/// <inheritdoc />
	public override IEnumerable<ulong> Factors(ulong value)
	{
		if (value == 0UL)
			goto exit;

		yield return 1UL;
		if (value == 1UL) yield break;
		ulong last = 1UL;

		// For larger numbers, a quick prime check can prevent large iterations.
		if (!IsFactorable(value))
			goto exit;

		foreach (var p in this)
		{
			var stop = value / last; // The list of possibilities shrinks for each test.
			if (p > stop) break; // Exceeded possibilities? 
			while ((value % p) == 0UL)
			{
				value /= p;
				yield return p;
				if (value == 1UL) yield break;
			}

			last = p;
		}

	exit:
		yield return value;
	}

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	public IEnumerable<long> Factors(long value)
	{
		if (value == 0L)
			goto exit;

		yield return value < 0L ? -1L : 1L;
		if (value < 0L) value = Math.Abs(value);
		if (value == 1L)
			yield break;

		var last = 1L;

		// For larger numbers, a quick prime check can prevent large iterations.
		if (!IsFactorable(value))
			goto exit;

		foreach (var p in StartingAt(2L))
		{
			var stop = value / last; // The list of possibilities shrinks for each test.
			if (p > stop) break; // Exceeded possibilities? 
			while ((value % p) == 0)
			{
				value /= p;
				yield return p;
				if (value == 1) yield break;
			}

			last = p;
		}

	exit:
		yield return value;
	}

	/// <inheritdoc />
	protected virtual bool IsFactorable(in long value)
		=> !IsPrime(in value);

#if !NET7_0_OR_GREATER
	/// <inheritdoc />
	public override ulong Next(in ulong after)
		=> StartingAt(after + 1UL).First();
#endif

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
	public long Next(in long after)
		=> StartingAt(after + 1L).First();

	/// <inheritdoc />
	public sealed override bool IsPrime(in ulong value)
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

				if (value % 2UL == 0 || value % 3UL == 0)
					return false;

				return IsPrimeInternal(in value);
		}
	}

	/// <inheritdoc cref="IsPrime(in ulong)" />
	public bool IsPrime(in int value)
		=> IsPrime(Convert.ToUInt64(Math.Abs(value)));

	/// <inheritdoc cref="IsPrime(in ulong)" />
	public bool IsPrime(in uint value)
		=> IsPrime(Convert.ToUInt64(value));

	/// <inheritdoc cref="IsPrime(in ulong)" />
	public bool IsPrime(in long value)
		=> IsPrime(Convert.ToUInt64(Math.Abs(value)));
}
