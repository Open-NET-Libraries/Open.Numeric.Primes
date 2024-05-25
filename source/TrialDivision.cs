using Open.Collections;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes;

/// <summary>
/// Trial-division prime utility.
/// </summary>
public static class TrialDivision
{
	/*
	 * A note about Math.Sqrt(n):
	 * It's used here to approximate the largest possible factor.
	 * The maximum contiguous integer that can be represented by a double precision number is 9,007,199,254,740,991.
	 * The good news is, if precision is lost, Math.Sqrt(n) should round up so although not perfect, it still serves its purpose.
	 */

	static readonly int[] FirstKnownInt32Array = [
		2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,
		67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137,
		139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211,
		223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
		293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379,
		383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461,
		463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541];

	/// <summary>
	/// The initial known primes to use instead of calculating them on every check.
	/// </summary>
	public static readonly ReadOnlyMemory<int> FirstKnownInt32 = FirstKnownInt32Array;

	//static readonly int LastKnownInt32 = FirstKnownInt32.Last();

	/// <summary>
	/// Trial-division for <see cref="uint"/>.
	/// </summary>
	public class U32 : PrimalityU32Base
	{
		static readonly uint[] FirstKnownArray
			= FirstKnownInt32Array.Select(Convert.ToUInt32).ToArray();

		/// <inheritdoc cref="FirstKnownInt32"/>
		public static readonly ReadOnlyMemory<uint> FirstKnown
			= FirstKnownInt32Array.Select(Convert.ToUInt32).ToArray();

		static readonly uint LastKnown = FirstKnownArray[FirstKnownArray.Length - 1];

		/// <inheritdoc />
		public override ParallelQuery<uint> InParallel(in uint startingAt, int? degreeOfParallelism = null)
		{
			Debug.Assert(!degreeOfParallelism.HasValue || degreeOfParallelism >= 0);

			var sa = startingAt;
			var source = sa > LastKnown
				? ValidPrimeTests(in sa)
				: FirstKnownArray
					.SkipWhile(v => v < sa)
					.Concat(ValidPrimeTests(LastKnown + 2));

			var tests = source
				.AsParallel()
				.AsOrdered();

			if (degreeOfParallelism > 1)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(v => IsPrime(in v));
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsPrimeInternal(in uint value)
			=> IsPrimeInternal(value);

		/// <inheritdoc />
		protected override bool IsPrimeInternal(uint value)
		{
			if (value < LastKnown)
				return FirstKnown.Span.BinarySearch(value) != -1;

			var sqr = Math.Sqrt(value);
			for (var p = 5U; p <= sqr; p += 2U)
			{
				if (value % p == 0U)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Find all primes after the provided known values.
		/// </summary>
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

		/// <summary>
		/// Iterates all possible primes.
		/// </summary>
		protected virtual IEnumerable<uint> AllPrimes()
		{
			foreach (var f in FirstKnownArray) // precomputed
				yield return f;

			foreach (var k in AllPrimesAfter(new LinkedList<uint>(FirstKnownArray)))
				yield return k;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		protected override bool IsFactorable(in uint value)
			=> true; // Do not do prime check first.

		/// <summary>
		/// Trial-division prime discovery class for 32 bit unsigned integers (<see cref="uint"/>) that remembers the primes used.
		/// </summary>
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

			/// <inheritdoc />
			protected override bool IsPrimeInternal(uint value)
			{
				if (value < LastKnown)
					return FirstKnown.Span.BinarySearch(value) != -1;

				var sqr = Math.Sqrt(value);
				foreach (var p in AllPrimes())
				{
					if (p > sqr) break;
					if (value % p == 0U)
						return false;
				}

				return true;
			}

			/// <summary>
			/// Iterates all prime numbers caching any previous prime factors.
			/// </summary>
			protected IEnumerable<uint> AllPrimesMemoizable()
			{
				uint last = 1;
				foreach (var n in FirstKnownArray)
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

	/// <summary>
	/// Trial-division prime discovery class for 64 bit unsigned integers (<see cref="ulong"/>).
	/// </summary>
	public class U64 : PrimalityU64Base
	{
		static readonly ulong[] FirstKnownArray
			= FirstKnownInt32Array.Select(Convert.ToUInt64).ToArray();

		/// <summary>
		/// The initial known primes to use instead of calculating them on every check.
		/// </summary>
		public static readonly ReadOnlyMemory<ulong> FirstKnown = FirstKnownArray;

		static readonly ulong LastKnown = FirstKnownArray[FirstKnownArray.Length - 1];

		/// <inheritdoc />
		public override ParallelQuery<ulong> InParallel(in ulong startingAt, int? degreeOfParallelism = null)
		{
			Debug.Assert(!degreeOfParallelism.HasValue || degreeOfParallelism >= 0);

			var sa = startingAt;
			var source = sa > LastKnown
				? ValidPrimeTests(in sa)
				: FirstKnownArray
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

		/// <inheritdoc />
		protected override bool IsPrimeInternal(in ulong value)
		{
			if (value < LastKnown)
				return FirstKnown.Span.BinarySearch(value) != -1;

			var sqr = Math.Sqrt(value);
			for (var p = 5UL; p <= sqr; p += 2UL)
			{
				if (value % p == 0) return false;
			}

			return true;
		}

		private IEnumerable<ulong> AllPrimesAfter(LinkedList<ulong> known)
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

		/// <summary>
		/// Iterates all possible primes.
		/// </summary>
		protected virtual IEnumerable<ulong> AllPrimes()
		{
			foreach (var f in FirstKnownArray)
				yield return f;

			foreach (var k in AllPrimesAfter(new LinkedList<ulong>(FirstKnownArray)))
				yield return k;
		}

		/// <inheritdoc />
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

		/// <inheritdoc />
		protected override bool IsFactorable(in ulong value)
			=> true; // Do not do prime check first.

		/// <summary>
		/// Trial-division prime discovery class for 64 bit unsigned integers (<see cref="ulong"/>) that remembers the primes used.
		/// </summary>
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

			/// <inheritdoc />
			protected override bool IsPrimeInternal(in ulong value)
			{
				if (value < LastKnown)
					return FirstKnown.Span.BinarySearch(value) != -1;

				var sqr = Math.Sqrt(value);
				foreach (var p in AllPrimes())
				{
					if (p > sqr) break;
					if (value % p == 0U)
						return false;
				}

				return true;
			}

			private IEnumerable<ulong> AllPrimesMemoizable()
			{
				ulong last = 1;
				foreach (var n in FirstKnownArray)
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
