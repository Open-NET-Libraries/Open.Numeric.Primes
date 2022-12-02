using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

public abstract class PrimalityU32Base : PrimalityIntegerBase<uint>
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected override IEnumerable<uint> ValidPrimeTests(in uint startingAt = 2U)
		=> Candidates.StartingAt(startingAt);

	/// <inheritdoc />
	public override IEnumerator<uint> GetEnumerator()
		=> StartingAt(2U).GetEnumerator();

	/// <summary>
	/// Returns an enumerable that will iterate every prime starting at the starting value.
	/// </summary>
	/// <param name="value">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.  Passing a negative number here will produce a negative set of prime numbers.</param>
	public IEnumerable<int> StartingAt(int value)
	{
		var absStart = (uint)Math.Abs(value);
		var selection = StartingAt(absStart).TakeWhile(v => v < int.MaxValue);

		return value < 0
#if NET7_0_OR_GREATER
			? selection.Select(ConvertInt32Negative)
			: selection.Select(Convert.ToInt32);
#else
			? selection.Select(e=>ConvertInt32Negative(e))
			: selection.Select(e=>Convert.ToInt32(e));
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int ConvertInt32Negative(uint value)
			=> -Convert.ToInt32(value);
	}

	/// <inheritdoc />
	public override IEnumerable<KeyValuePair<uint, uint>> Indexed()
	{
		uint count = 0U;
		foreach (var n in this)
		{
			count++;
			yield return new KeyValuePair<uint, uint>(count, n);
		}
	}

	/// <inheritdoc />
	public override IEnumerable<uint> Factors(uint value)
	{
		if (value == 0U)
			goto exit;

		yield return 1U;
		if (value == 1U) yield break;

		// For larger numbers, a quick prime check can prevent large iterations.
		if (!IsFactorable(value))
			goto exit;

		var last = 1U;
		foreach (var p in this)
		{
			var stop = value / last; // The list of possibilities shrinks for each test.
			if (p > stop) break; // Exceeded possibilities? 
			while ((value % p) == 0U)
			{
				value /= p;
				yield return p;
				if (value == 1U) yield break;
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
	public IEnumerable<int> Factors(int value)
		=> value < 0
		? Factors(-value).Select(e=>-e)
		: Factors(value);

	protected virtual bool IsFactorable(in int value)
		=> !IsPrime(in value);

#if !NET7_0_OR_GREATER
	/// <inheritdoc />
	public override uint Next(in uint after)
		=> StartingAt(after + 1).First();
#endif

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
	public int Next(in int after)
		=> StartingAt(after + 1).First();

	/// <inheritdoc />
	public sealed override bool IsPrime(in uint value)
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

				if (value % 2U == 0 || value % 3U == 0)
					return false;

				return IsPrimeInternal(in value);
		}
	}

	/// <summary>
	/// Returns <see langword="true"/> if the value provided is prime.
	/// </summary>
	/// <param name="value">The value to validate.</param>
	public bool IsPrime(in int value)
		=> value < 0
			? IsPrime((uint)-value)
			: IsPrime((uint)value);

	/// <summary>
	/// Should only check for primes that aren't divisible by 2 or 3.
	/// </summary>
	protected abstract bool IsPrimeInternal(uint value);
}
