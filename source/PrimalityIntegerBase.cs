using System;

namespace Open.Numeric.Primes;

public abstract class PrimalityIntegerBase<T> : PrimalityBase<T>
#if NET7_0_OR_GREATER
	where T : notnull, System.Numerics.IBinaryInteger<T>
#else
	where T : notnull, IEquatable<T>, IComparable<T>
#endif
{ }
