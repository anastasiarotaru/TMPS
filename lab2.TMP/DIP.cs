namespace DeliverySOLID;

// Abstractie: cum se trimite un mesaj clientului.
public interface INotifier
{
    void Send(string recipient, string message);
}

// Afiseaza mesajele in consola.
public class ConsoleNotifier : INotifier
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"  [notificare -> {recipient}] {message}");
    }
}

// Pastreaza mesajele in memorie. Util pentru verificari.
public class InMemoryNotifier : INotifier
{
    public List<string> Sent { get; } = new();

    public void Send(string recipient, string message)
    {
        Sent.Add($"{recipient}: {message}");
    }
}

// Clasa de nivel inalt. Depinde doar de abstractiile INotifier si IPricingStrategy.
public class DeliveryService
{
    private static readonly ParcelStatus[] Flow =
    {
        ParcelStatus.Registered,
        ParcelStatus.PickedUp,
        ParcelStatus.InTransit,
        ParcelStatus.Delivered
    };

    private readonly INotifier notifier;
    private readonly Dictionary<string, IPricingStrategy> services;
    private readonly Dictionary<string, Parcel> parcels = new();

    public DeliveryService(INotifier notifier, Dictionary<string, IPricingStrategy> services)
    {
        this.notifier = notifier;
        this.services = services;
    }

    public Parcel Register(string recipientName, string recipientEmail,
                           Address destination, double weightKg, string service)
    {
        if (!services.ContainsKey(service))
        {
            throw new ArgumentException(
                $"Serviciu necunoscut '{service}'. Disponibile: {string.Join(", ", services.Keys)}");
        }

        // Builder construieste coletul; el valideaza datele si cere codul de la Singleton.
        Parcel parcel = new ParcelBuilder()
            .To(recipientName, recipientEmail)
            .Destination(destination)
            .Weight(weightKg)
            .Service(service)
            .Build(services[service]);

        parcels[parcel.TrackingId] = parcel;

        notifier.Send(recipientEmail,
            $"Coletul {parcel.TrackingId} a fost inregistrat. Pret: {parcel.Price:F2} lei.");
        return parcel;
    }

    // Trece coletul in urmatoarea etapa. Nu se pot sari etape.
    public void Advance(string trackingId, ParcelStatus newStatus)
    {
        Parcel parcel = Get(trackingId);
        int index = Array.IndexOf(Flow, parcel.Status);

        if (index + 1 >= Flow.Length || Flow[index + 1] != newStatus)
        {
            throw new InvalidOperationException(
                $"Tranzitie invalida: {parcel.Status} -> {newStatus}");
        }

        parcel.Status = newStatus;
        notifier.Send(parcel.RecipientEmail,
            $"Coletul {trackingId}: status nou - {newStatus}.");
    }

    public Parcel Get(string trackingId)
    {
        if (!parcels.TryGetValue(trackingId, out Parcel? parcel))
        {
            throw new KeyNotFoundException($"Nu exista niciun colet cu codul {trackingId}");
        }

        return parcel;
    }
}
