namespace DeliverySOLID;

// SINGLETON: exista o singura instanta a generatorului de coduri AWB.
// Asa nu pot aparea doua colete cu acelasi cod, chiar daca sunt create din locuri diferite.
public sealed class TrackingIdGenerator
{
    // Lazy: instanta se creeaza doar la prima utilizare. Lazy<T> este thread-safe implicit.
    private static readonly Lazy<TrackingIdGenerator> instance =
        new Lazy<TrackingIdGenerator>(() => new TrackingIdGenerator());

    private readonly object gate = new object();
    private int next = 1;

    // Constructor privat: nimeni din afara clasei nu poate face new TrackingIdGenerator().
    private TrackingIdGenerator() { }

    public static TrackingIdGenerator Instance => instance.Value;

    // Returneaza urmatorul cod, de forma RO000001, RO000002, ...
    public string Next()
    {
        lock (gate)
        {
            string id = $"RO{next:D6}";
            next++;
            return id;
        }
    }
}
