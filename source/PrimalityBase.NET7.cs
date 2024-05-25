#if NET7_0_OR_GREATER
namespace Open.Numeric.Primes;

public abstract partial class PrimalityBase<T>
	where T : notnull, INumber<T>
{
	/// <inheritdoc cref="IsPrime(T)"/>
	[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
	public virtual bool IsPrime(in T value)
	{
		Debug.Assert(!T.IsInfinity(value));

		if (value < Number<T>.Two)
			return T.IsNegative(value) && IsPrime(-value);

		if (!T.IsInteger(value))
			return false;

		if (value - Number<T>.Two <= T.One)
			return true;

		if (T.IsEvenInteger(value) || T.IsZero(value % Number<T>.Three))
			return false;

		return IsPrimeInternal(in value);
	}

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
	public T Next(in T after)
		=> StartingAt(after + T.One).First();

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	public virtual IEnumerable<T> Factors(T value)
	{
		if (!T.IsInteger(value) || T.IsZero(value) || value == T.One)
			goto exit;

		yield return T.One;

		// For larger numbers, a quick prime check can prevent large iterations.
		if (IsPrime(in value))
			goto exit;

		var last = T.One;
		foreach (var p in this)
		{
			var stop = value / last; // The list of possibilities shrinks for each test.
			if (p > stop) break; // Exceeded possibilities? 
			while (T.IsZero(value % p))
			{
				value /= p;
				yield return p;
				if (value == T.One) yield break;
			}

			last = p;
		}

	exit:
		yield return value;
	}
}
#endif