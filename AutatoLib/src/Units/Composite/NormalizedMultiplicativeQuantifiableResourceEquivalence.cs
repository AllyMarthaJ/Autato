namespace AutatoLib.Units.Composite;

public record NormalizedMultiplicativeQuantifiableResourceEquivalence<TOut, TIn>(
    double Quantity,
    TOut ResourceOut,
    TIn ResourceIn) : MultiplicativeQuantifiableResourceEquivalence<TOut, TIn>(Quantity, ResourceOut, 1, ResourceIn);