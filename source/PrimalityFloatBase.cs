using System.Collections.Generic;
using System.Diagnostics;

namespace Open.Numeric.Primes;

public abstract partial class PrimalityFloatBase<T> : PrimalityBase<T>
#if NET7_0_OR_GREATER
	where T : System.Numerics.IFloatingPoint<T>
#endif
{
#if NET7_0_OR_GREATER
	protected override IEnumerable<T> ValidPrimeTests(in T staringAt)
	{
		var n = T.Sign(staringAt) == -1
			? T.Floor(staringAt)
			: T.Ceiling(staringAt);

		return base.ValidPrimeTests(in n);
	}
#endif
}