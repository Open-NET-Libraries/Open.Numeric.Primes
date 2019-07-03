using Open.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Open.Numeric.Primes
{
	public class TrialDivision : PrimalityU64Base
	{
		public static readonly IReadOnlyList<ulong> FirstKnown
			= (new List<ulong>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, })
				.AsReadOnly();

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

			foreach (var p in AllNumbers())
			{
				if (p > sqr || value == p)
					return true;
				if (value % p == 0)
					return false;
			}

			throw new Exception("Unexpectedly exited prime test loop.");
		}

		protected virtual IEnumerable<ulong> AllNumbers()
		{
			var known = new LinkedList<ulong>();
			var last = 0UL;
			foreach (var k in FirstKnown)
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
						known.AddLast(n);
						yield return n;
						pN = null;
					}
					else if (n == p || (n % p) == 0)
					{
						pN = null;
					}
					else
					{
						pN = pN.Next;
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
	}

	public class TrialDivisionMemoized : TrialDivision
	{

		LazyList<ulong> Memoized;

		/// <summary>
		/// Returns a memoized enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <returns>A memoized enumerable that will iterate every prime starting at the starting value</returns>
		protected override IEnumerable<ulong> AllNumbers()
			=> LazyInitializer
				.EnsureInitialized(ref Memoized,
					() => NumbersMemoizable().Memoize());

		protected IEnumerable<ulong> NumbersMemoizable()
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
