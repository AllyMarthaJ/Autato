namespace AutatoLib.Units.Composite;

// Aka a "rate"; yield [Quantity] [Unit] / [Every].
// e.g. in Minecraft, the standard tick rate is 20 ticks / sec.
// Represented as TimePerTime(20, Tick, Second). Its inverse is therefore
// TimePerTime(1 / 20, Second, Tick).
public record TimePerTime(double Quantity, Time ResourceOut, Time ResourceIn)
    : NormalizedMultiplicativeQuantifiableResourceEquivalence<Time, Time>(Quantity, ResourceOut, ResourceIn);