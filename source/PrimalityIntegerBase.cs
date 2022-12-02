namespace Open.Numeric.Primes;

public abstract class PrimalityIntegerBase<T> : PrimalityBase<T>
#if NET7_0_OR_GREATER
	where T : struct, System.Numerics.IBinaryInteger<T>
#endif
{ }
