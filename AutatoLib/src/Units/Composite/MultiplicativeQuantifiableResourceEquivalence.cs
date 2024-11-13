namespace AutatoLib.Units.Composite;

public record MultiplicativeQuantifiableResourceEquivalence<TOut, TIn>(double QuantityOut, TOut ResourceOut, double QuantityIn, TIn ResourceIn);