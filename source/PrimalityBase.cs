using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes
{
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
	public abstract class PrimalityBase<T> : IEnumerable<T>
		where T : struct
	{

		protected abstract IEnumerable<T> ValidPrimeTests(T startingAt);

		/// <summary>
		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public abstract bool IsPrime(in T value);

		/// <summary>
		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public bool IsPrime(T value) => IsPrime(in value);

		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="value">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
		/// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
		public virtual IEnumerable<T> StartingAt(T value)
			=> ValidPrimeTests(value)
				.Where(IsPrime);

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
		// ReSharper disable once UnusedMemberInSuper.Global
		public abstract ParallelQuery<T> InParallel(in T staringAt, ushort? degreeOfParallelism = null);

		/// <summary>
		/// Returns a parallel enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
		/// <returns>An ordered parallel enumerable of primes.</returns>
		public abstract ParallelQuery<T> InParallel(ushort? degreeOfParallelism = null);

		protected virtual bool IsFactorable(in T value)
			=> !IsPrime(in value);

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0, 1 or -1.
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
		public abstract IEnumerable<T> Factors(T value);

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

		/// <summary>
		/// Finds the next prime number after the number given.
		/// </summary>
		/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
		/// <returns>The next prime after the number provided.</returns>s
		public abstract T Next(in T after);

		/// <inheritdoc />
		public abstract IEnumerator<T> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public abstract class PrimalityU64Base : PrimalityBase<ulong>
	{
		// ReSharper disable once OptionalParameterHierarchyMismatch
		protected override IEnumerable<ulong> ValidPrimeTests(ulong staringAt = 2UL)
		{
			var n = staringAt;
			if (n > 2UL)
			{
				if (n % 2UL == 0)
					n++;
			}
			else
			{
				yield return 2UL;
				n = 3UL;
			}

			for (; n < ulong.MaxValue - 1UL; n += 2UL)
				yield return n;
		}

		/// <inheritdoc />
		public override IEnumerator<ulong> GetEnumerator()
			=> StartingAt(2UL).GetEnumerator();

		/// <summary>
		/// Returns an enumerable that will iterate every prime starting at the starting value.
		/// </summary>
		/// <param name="value">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.  Passing a negative number here will produce a negative set of prime numbers.</param>
		/// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		public IEnumerable<long> StartingAt(long value)
		{
			var absStart = (ulong)Math.Abs(value);
			if (value < 0)
			{
				return StartingAt(absStart)
					.TakeWhile(v => v < long.MaxValue)
					.Select(v => -Convert.ToInt64(v));
			}

			return StartingAt(absStart)
				.TakeWhile(v => v < long.MaxValue)
				.Select(Convert.ToInt64);
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
		public override ParallelQuery<ulong> InParallel(in ulong staringAt, ushort? degreeOfParallelism = null)
		{
			var tests = ValidPrimeTests(staringAt)
				.AsParallel().AsOrdered();

			if (degreeOfParallelism.HasValue)
				tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return tests.Where(IsPrime);
		}

		/// <inheritdoc />
		public override ParallelQuery<ulong> InParallel(ushort? degreeOfParallelism = null)
			=> InParallel(2UL, degreeOfParallelism);

		/// <inheritdoc />
		public override IEnumerable<ulong> Factors(ulong value)
		{
			if (value != 0UL)
			{
				yield return 1;
				if (value == 1) yield break;
				ulong last = 1;

				// For larger numbers, a quick prime check can prevent large iterations.
				if (IsFactorable(value))
				{
					foreach (var p in this)
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
				}
			}

			yield return value;
		}

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0, 1 or -1.
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
		public IEnumerable<long> Factors(long value)
		{
			if (value != 0L)
			{
				yield return value < 0L ? -1L : 1L;
				if (value < 0L) value = Math.Abs(value);
				if (value == 1L)
					yield break;

				var last = 1L;

				// For larger numbers, a quick prime check can prevent large iterations.
				if (IsFactorable(value))
				{
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
				}
			}
			yield return value;
		}

		protected bool IsFactorable(in long value)
			=> !IsPrime(in value);

		/// <inheritdoc />
		public override ulong Next(in ulong after)
			=> StartingAt(after + 1).First();

		/// <summary>
		/// Finds the next prime number after the number given.
		/// </summary>
		/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
		/// <returns>The next prime after the number provided.</returns>
		public long Next(in long after)
			=> StartingAt(after + 1).First();

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

		/// <summary>
		/// Returns true if the value provided is prime.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value provided is prime</returns>
		public bool IsPrime(in long value)
			=> IsPrime(Convert.ToUInt64(Math.Abs(value)));

		/// <summary>
		/// Should only check for primes that aren't divisible by 2 or 3.
		/// </summary>
		protected abstract bool IsPrimeInternal(in ulong value);
	}

	public abstract class PrimalityBigIntBase : PrimalityBase<BigInteger>
	{
		protected override IEnumerable<BigInteger> ValidPrimeTests(BigInteger staringAt)
		{
			var sign = staringAt.Sign;
			if (sign == 0) sign = 1;
			var n = BigInteger.Abs(staringAt);

			if (n > BIG.TWO)
			{
				if (n % BIG.TWO == 0)
					n++;
			}
			else
			{
				yield return sign * BIG.TWO;
				n = BIG.THREE;
			}

			while (true)
			{
				yield return sign * n;
				n += BIG.TWO;
			}
			// ReSharper disable once IteratorNeverReturns
		}

		/// <inheritdoc />
		public override IEnumerator<BigInteger> GetEnumerator()
			=> StartingAt(BigInteger.One).GetEnumerator();

		protected IEnumerable<BigInteger> ValidPrimeTests()
			=> ValidPrimeTests(BigInteger.One);

		/// <inheritdoc />
		public override IEnumerable<KeyValuePair<BigInteger, BigInteger>> Indexed()
		{
			var count = BigInteger.Zero;
			foreach (var n in this)
			{
				count++;
				yield return new KeyValuePair<BigInteger, BigInteger>(count, n);
			}
		}

		/// <inheritdoc />
		public override ParallelQuery<BigInteger> InParallel(in BigInteger staringAt, ushort? degreeOfParallelism = null)
		{
			var testsBig = ValidPrimeTests(staringAt)
				.AsParallel().AsOrdered();

			if (degreeOfParallelism.HasValue)
				testsBig = testsBig.WithDegreeOfParallelism(degreeOfParallelism.Value);

			return testsBig.Where(IsPrime);
		}

		/// <inheritdoc />
		public override ParallelQuery<BigInteger> InParallel(ushort? degreeOfParallelism = null)
			=> InParallel(in BIG.TWO, degreeOfParallelism);


		/// <inheritdoc />
		public override IEnumerable<BigInteger> Factors(BigInteger value)
		{
			if (value != BigInteger.Zero)
			{
				yield return value.Sign == -1
					? BigInteger.MinusOne
					: BigInteger.One;

				if (value.IsOne || value == BigInteger.MinusOne)
					yield break;

				value = BigInteger.Abs(value);
				var last = BigInteger.One;

				// For larger numbers, a quick prime check can prevent large iterations.
				if (IsFactorable(in value))
				{
					foreach (var p in this)
					{
						var stop = value / last; // The list of possibilities shrinks for each test.
						if (p > stop) break; // Exceeded possibilities? 
						while ((value % p) == 0)
						{
							value /= p;
							yield return p;
							if (value.IsOne) yield break;
						}
						last = p;
					}
				}
			}

			yield return value;
		}

		/// <inheritdoc />
		public override BigInteger Next(in BigInteger after)
			=> StartingAt(after + BigInteger.One).First();

		/// <summary>
		/// Finds the next prime number after the number given.
		/// </summary>
		/// <param name="after">The excluded lower boundary to start with.</param>
		/// <returns>The next prime after the number provided.</returns>
		public BigInteger Next(float after)
			=> Next((BigInteger)after);

		/// <summary>
		/// Finds the next prime number after the number given.
		/// </summary>
		/// <param name="after">The excluded lower boundary to start with.</param>
		/// <returns>The next prime after the number provided.</returns>
		public BigInteger Next(double after)
			=> Next((BigInteger)after);

		/// <inheritdoc />
		public sealed override bool IsPrime(in BigInteger value)
		{
			if (value.IsZero)
				return false;

			bool primeCheck(in BigInteger v)
			{
				if (v == BIG.TWO || v == BIG.THREE)
					return true;

				if (v.IsEven)
					return false;

				if (v <= ulong.MaxValue)
					return Prime.Numbers.IsPrime((ulong)v);

				return v % BIG.THREE != 0 && IsPrimeInternal(in v);
			}

			return value.Sign == -1
				? value != BigInteger.MinusOne && primeCheck(BigInteger.Abs(value))
				: !value.IsOne && primeCheck(in value);
		}

		protected abstract bool IsPrimeInternal(in BigInteger value);
	}
}
