using Open.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Open.Numeric.Primes;

public static class TrialDivision
{
	public static readonly ImmutableArray<int> FirstKnownInt32
		= ImmutableArray.Create(
			2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
			67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137,
			139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211,
			223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
			293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379,
			383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461,
			463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541);

	//static readonly int LastKnownInt32 = FirstKnownInt32.Last();

	public class U32 : PrimalityU32Base
	{
		public static readonly ImmutableArray<uint> FirstKnown
			= FirstKnownInt32.Select(Convert.ToUInt32).ToImmutableArray();

		static readonly uint LastKnown = FirstKnown.Last();

		/// <inheritdoc />
		public override ParallelQuery<uint> InParallel(in uint startingAt, int? degreeOfParallelism = null)
		{
			Debug.Assert(!degreeOfParallelism.HasValue || degreeOfParallelism >= 0);

			var sa = startingAt;
			var source = sa > LastKnown
				? ValidPrimeTests(in sa)
				: FirstKnown
					.SkipWhile(v => v < sa)
					.Concat(ValidPrimeTests(LastKnown + 2));

			var tests = source
				.AsParallel()
				.AsOrdered();

			if (degreeOfParallelism > 1)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(v => IsPrime(in v));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsPrimeInternal(in uint value)
			=> IsPrimeInternal(value);

		protected override bool IsPrimeInternal(uint value)
		{
			if (value < LastKnown)
				return FirstKnown.BinarySearch(value) != -1;

			var sqr = (uint)Math.Sqrt(value);
			for (var p = 5U; p <= sqr; p += 2U)
			{
				if (value % p == 0U)
					return false;
			}

			return true;
		}

		protected IEnumerable<uint> AllPrimesAfter(LinkedList<uint> known)
		{
			Debug.Assert(known.Count != 0);
			var last = known.Last!.Value;

			foreach (var n in ValidPrimeTests(last + 2))
			{
				last = 1U;
				var pN = known.First;
				do
				{
					var p = pN!.Value;
					var stop = n / last; // The list of possibilities shrinks for each test.
					if (p > stop)
					{
						_ = known.AddLast(n);
						yield return n;
						pN = null;
					}
					else
					{
						pN = n == p || (n % p) == 0U ? null : pN.Next;
					}

					last = p;
				}
				while (pN != null);
			}
		}

		protected virtual IEnumerable<uint> AllPrimes()
		{
			foreach (var f in FirstKnown) // precomputed
				yield return f;

			foreach (var k in AllPrimesAfter(new LinkedList<uint>(FirstKnown)))
				yield return k;
		}

		public override IEnumerator<uint> GetEnumerator()
			=> AllPrimes().GetEnumerator();

		/// <inheritdoc />
		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="startingAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
		/// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
		public override IEnumerable<uint> StartingAt(in uint startingAt)
		{
			var sa = startingAt;
			return AllPrimes().SkipWhile(n => n < sa);
		}

		protected override bool IsFactorable(in uint value)
			=> true; // Do not do prime check first.

		public class Memoized : U32
		{
			LazyList<uint>? _memoized;

			/// <summary>
			/// Returns a memoized enumerable that will iterate every prime starting at the starting value.
			/// </summary>
			/// <returns>A memoized enumerable that will iterate every prime starting at the starting value</returns>
			protected override IEnumerable<uint> AllPrimes()
				=> LazyInitializer
					.EnsureInitialized(ref _memoized,
						() => AllPrimesMemoizable().Memoize())!;

			protected override bool IsPrimeInternal(uint value)
			{
				if (value < LastKnown)
					return FirstKnown.BinarySearch(value) != -1;

				var sqr = (uint)Math.Sqrt(value);
				foreach(var p in AllPrimes())
				{
					if (p > sqr) break;
					if (value % p == 0U)
						return false;
				}

				return true;
			}

			protected IEnumerable<uint> AllPrimesMemoizable()
			{
				uint last = 1;
				foreach (var n in FirstKnown)
				{
					yield return n;
					last = n;
				}

				/*
				 * Note: here is where things start to recurse but should work perfectly
				 * as the next primes can only be discovered by their predecessors.
				 */
				foreach (var n in StartingAt(last + 1).Where(v => IsPrime(in v)))
					yield return n;
			}
		}
	}

	public class U64 : PrimalityU64Base
	{
		public static readonly ImmutableArray<ulong> FirstKnown
			= FirstKnownInt32.Select(Convert.ToUInt64).ToImmutableArray();

		static readonly ulong LastKnown = FirstKnown.Last();

		/// <inheritdoc />
		public override ParallelQuery<ulong> InParallel(in ulong startingAt, int? degreeOfParallelism = null)
		{
			Debug.Assert(!degreeOfParallelism.HasValue || degreeOfParallelism >= 0);

			var sa = startingAt;
			var source = sa > LastKnown
				? ValidPrimeTests(in sa)
				: FirstKnown
					.SkipWhile(v => v < sa)
					.Concat(ValidPrimeTests(LastKnown + 2));

			var tests = source
				.AsParallel()
				.AsOrdered();

			if (degreeOfParallelism > 1)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(v => IsPrime(in v));
		}

		// const ulong MAX_ULONG_SQUARE_ROOT = 4294967296;

		protected override bool IsPrimeInternal(in ulong value)
		{
			if (value < LastKnown)
				return FirstKnown.BinarySearch(value) != -1;

			var sqr = (ulong)Math.Sqrt(value);
			for (var p = 5UL; p <= sqr; p += 2UL)
			{
				if (value % p == 0) return false;
			}

			return true;
		}

		protected IEnumerable<ulong> AllPrimesAfter(LinkedList<ulong> known)
		{
			Debug.Assert(known.Count != 0);
			var last = known.Last!.Value;

			foreach (var n in ValidPrimeTests(last + 2))
			{
				last = 1L;
				var pN = known.First;
				do
				{
					var p = pN!.Value;
					var stop = n / last; // The list of possibilities shrinks for each test.
					if (p > stop)
					{
						_ = known.AddLast(n);
						yield return n;
						pN = null;
					}
					else
					{
						pN = n == p || (n % p) == 0UL ? null : pN.Next;
					}

					last = p;
				}
				while (pN is not null);
			}
		}

		protected virtual IEnumerable<ulong> AllPrimes()
		{
			foreach (var f in FirstKnown)
				yield return f;

			foreach (var k in AllPrimesAfter(new LinkedList<ulong>(FirstKnown)))
				yield return k;
		}

		public override IEnumerator<ulong> GetEnumerator()
			=> AllPrimes().GetEnumerator();

		/// <inheritdoc />
		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="startingAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
		public override IEnumerable<ulong> StartingAt(in ulong startingAt)
		{
			var sa = startingAt;
			return AllPrimes().SkipWhile(n => n < sa);
		}

		protected override bool IsFactorable(in ulong value)
			=> true; // Do not do prime check first.

		public class Memoized : U64
		{
			LazyList<ulong>? _memoized;

			/// <summary>
			/// Returns a memoized enumerable that will iterate every prime starting at the starting value.
			/// </summary>
			protected override IEnumerable<ulong> AllPrimes()
				=> LazyInitializer
					.EnsureInitialized(ref _memoized,
						() => AllPrimesMemoizable().Memoize())!;

			protected override bool IsPrimeInternal(in ulong value)
			{
				if (value < LastKnown)
					return FirstKnown.BinarySearch(value) != -1;

				var sqr = (uint)Math.Sqrt(value);
				foreach (var p in AllPrimes())
				{
					if (p > sqr) break;
					if (value % p == 0U)
						return false;
				}

				return true;
			}

			protected IEnumerable<ulong> AllPrimesMemoizable()
			{
				ulong last = 1;
				foreach (var n in FirstKnown)
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
