using System.Diagnostics.CodeAnalysis;
using AutatoLib.Units.Composite;

namespace AutatoLib.Units.Conversions;

public class NormalizedMultiplicativeQuantifiableConverter<T, TConverter>
    where T : MultiplicativeQuantifiableResource
    where TConverter : NormalizedMultiplicativeQuantifiableResourceEquivalence<T, T> {
    
    // Can we convert from time A to time B?
    private readonly Dictionary<T, MultiplicativeQuantifiableResourceNode<T>> _nodes = new();

    public void Add(T element) {
        // You can't register a converter before a time, so there's nothing more to do here.
        this._nodes.TryAdd(element, new MultiplicativeQuantifiableResourceNode<T>());
    }

    public void RegisterConverter(TConverter equivalence) {
        if (!this._nodes.TryGetValue(equivalence.ResourceIn, out var timeFromNode)) {
            throw new ArgumentException("Source element is not registered.");
        }

        if (
            !this._nodes.TryGetValue(equivalence.ResourceOut, out var timeToNode)) {
            throw new ArgumentException("Target element is not registered.");
        }

        // Add the initial conversion
        timeFromNode.Conversions.TryAdd(equivalence.ResourceOut, equivalence.Quantity);

        // The shortest path between any two conversions should be 1.

        // If I Add A -> B, A should receive everything B has.
        foreach (var conversion in timeToNode.Conversions) {
            timeFromNode.Conversions.TryAdd(conversion.Key, conversion.Value * equivalence.Quantity);
        }

        // If I add A -> B, any dependents of A should 
        // receive -> B as well, by quantity of A, if they have it.
        foreach (var node in this._nodes) {
            if (node.Value.Conversions.TryGetValue(equivalence.ResourceIn, out var quantity)) {
                node.Value.Conversions.TryAdd(equivalence.ResourceOut, quantity * equivalence.Quantity);
            }
        }
    }

    public double Convert(T from, T to, double quantity, bool strict = false) {
        if (this._nodes[from].TryToOther(to, quantity, out var timeQuantity)) {
            return timeQuantity;
        }
        
        if (strict) {
            throw new ArgumentException("Conversion is not registered.");
        }
        return this._nodes[to].ToInverseOther(from, quantity);
    }
}

internal class MultiplicativeQuantifiableResourceNode<T> where T : notnull {
    // To what unit of time and quantity of time will it produce?
    public Dictionary<T, double> Conversions { get; } = new();

    public double ToOther(T mqrOther, double quantity) {
        return quantity * this.Conversions[mqrOther];
    }

    public double ToInverseOther(T mqrOther, double quantity) {
        return 1 / this.ToOther(mqrOther, quantity);
    }

    public bool TryToOther(T mqrOther, double quantity, [MaybeNullWhen(false)] out double mqrResultQuantity) {
        if (!this.Conversions.TryGetValue(mqrOther, out var mqrQuantity)) {
            mqrResultQuantity = default;
            return false;
        }

        mqrResultQuantity = quantity * mqrQuantity;
        return true;
    }
}