using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Open.Numeric.Primes;

public abstract class PrimalityFloatBase<T> : PrimalityBase<T>
#if NET7_0_OR_GREATER
	where T : notnull, System.Numerics.IFloatingPoint<T>
#else
	where T : notnull, IEquatable<T>, IComparable<T>
#endif
{
#if NET7_0_OR_GREATER
	protected override IEnumerable<T> ValidPrimeTests(in T startingAt)
	{
		var n = T.Sign(startingAt) == -1
			? T.Floor(startingAt)
			: T.Ceiling(startingAt);

		return base.ValidPrimeTests(in n);
	}
#endif
}