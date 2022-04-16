using Open.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Open.Numeric.Primes;

public static class TrialDivision
{
	public static readonly ImmutableArray<ushort> FirstKnown
		= ImmutableArray.Create<ushort>(2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541);

	//static readonly ushort LastKnown = FirstKnown.Last();

	public class U32 : PrimalityU32Base
	{
		public static readonly ImmutableArray<uint> FirstKnown32 = FirstKnown.Select(Convert.ToUInt32).ToImmutableArray();

		/// <inheritdoc />
		public override ParallelQuery<uint> InParallel(in uint staringAt, ushort? degreeOfParallelism = null)
		{
			var sa = staringAt;
			var tests = AllNumbers().SkipWhile(v => v < sa) // This is the difference.
				.AsParallel().AsOrdered();

			if (degreeOfParallelism.HasValue)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(v => IsPrime(in v));
		}

		protected override bool IsPrimeInternal(uint value)
		{
			var sqr = (uint)Math.Sqrt(value);
			for (var p = 5U; p <= sqr; p += 2U)
			{
				if (value % p == 0) return false;
			}

			return true;
		}

		protected virtual IEnumerable<uint> AllNumbers()
		{
			var known = new LinkedList<uint>();
			var last = 0U;
			foreach (var k in FirstKnown32) // precomputed
			{
				yield return k;
				last = k;
				known.AddLast(k);
			}

			foreach (var n in ValidPrimeTests(last + 2))
			{
				last = 1U;
				var pN = known.First;
				do
				{
					var p = pN.Value;
					var stop = n / last; // The list of possibilities shrinks for each test.
					if (p > stop)
					{
						_ = known.AddLast(n);
						yield return n;
						pN = null;
					}
					else
					{
						pN = n == p || (n % p) == 0 ? null : pN.Next;
					}

					last = p;
				}
				while (pN != null);
			}
		}

		public override IEnumerator<uint> GetEnumerator()
			=> AllNumbers().GetEnumerator();

		/// <inheritdoc />
		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="startingAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
		/// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
		public override IEnumerable<uint> StartingAt(uint startingAt)
			=> AllNumbers().SkipWhile(n => n < startingAt);

		protected override bool IsFactorable(in uint value)
			=> true; // Do not do prime check first.

		public class Memoized : U32
		{
			LazyList<uint>? _memoized;

			/// <summary>
			/// Returns a memoized enumerable that will iterate every prime starting at the starting value.
			/// </summary>
			/// <returns>A memoized enumerable that will iterate every prime starting at the starting value</returns>
			protected override IEnumerable<uint> AllNumbers()
				=> LazyInitializer
					.EnsureInitialized(ref _memoized,
						() => NumbersMemoizable().Memoize())!;

			protected IEnumerable<uint> NumbersMemoizable()
			{
				uint last = 1;
				foreach (var n in FirstKnown32)
				{
					yield return n;
					last = n;
				}

				/*
				 * Note: here is where things start to recurse but should work perfectly
				 * as the next primes can only be discovered by their predecessors.
				 */
				foreach (var n in ValidPrimeTests(last + 1).Where(v => IsPrime(in v)))
					yield return n;
			}
		}
	}

	public class U64 : PrimalityU64Base
	{
		public static readonly ImmutableArray<ulong> FirstKnown64 = FirstKnown.Select(Convert.ToUInt64).ToImmutableArray();

		/// <inheritdoc />
		public override ParallelQuery<ulong> InParallel(in ulong staringAt, ushort? degreeOfParallelism = null)
		{
			var sa = staringAt;
			var tests = AllNumbers().SkipWhile(v => v < sa) // This is the difference.
				.AsParallel().AsOrdered();

			if (degreeOfParallelism.HasValue)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(v => IsPrime(in v));
		}

		// const ulong MAX_ULONG_SQUARE_ROOT = 4294967296;

		protected override bool IsPrimeInternal(in ulong value)
		{
			var sqr = (ulong)Math.Sqrt(value);
			for (var p = 5UL; p <= sqr; p += 2UL)
			{
				if (value % p == 0) return false;
			}

			return true;
		}

		protected virtual IEnumerable<ulong> AllNumbers()
		{
			var known = new LinkedList<ulong>();
			var last = 0UL;
			foreach (var k in FirstKnown64)
			{
				yield return k;
				last = k;
				known.AddLast(k);
			}

			foreach (var n in ValidPrimeTests(last + 2))
			{
				last = 1L;
				var pN = known.First;
				do
				{
					var p = pN.Value;
					var stop = n / last; // The list of possibilities shrinks for each test.
					if (p > stop)
					{
						_ = known.AddLast(n);
						yield return n;
						pN = null;
					}
					else
					{
						pN = n == p || (n % p) == 0 ? null : pN.Next;
					}

					last = p;
				}
				while (pN != null);
			}
		}

		public override IEnumerator<ulong> GetEnumerator()
			=> AllNumbers().GetEnumerator();

		/// <inheritdoc />
		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="startingAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
		/// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
		public override IEnumerable<ulong> StartingAt(ulong startingAt)
			=> AllNumbers().SkipWhile(n => n < startingAt);

		protected override bool IsFactorable(in ulong value)
			=> true; // Do not do prime check first.

		public class Memoized : U64
		{
			LazyList<ulong>? _memoized;

			/// <summary>
			/// Returns a memoized enumerable that will iterate every prime starting at the starting value.
			/// </summary>
			/// <returns>A memoized enumerable that will iterate every prime starting at the starting value</returns>
			protected override IEnumerable<ulong> AllNumbers()
				=> LazyInitializer
					.EnsureInitialized(ref _memoized,
						() => NumbersMemoizable().Memoize())!;

			protected IEnumerable<ulong> NumbersMemoizable()
			{
				ulong last = 1;
				foreach (var n in FirstKnown64)
				{
					yield return n;
					last = n;
				}

				/*
				 * Note: here is where things start to recurse but should work perfectly
				 * as the next primes can only be discovered by their predecessors.
				 */
				foreach (var n in ValidPrimeTests(last + 1).Where(v => IsPrime(in v)))
					yield return n;
			}
		}
	}
}
