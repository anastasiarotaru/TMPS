using DeliverySOLID;

// 1. FACTORY METHOD: fiecare fabrica creeaza strategia de pret pentru un serviciu.
var factories = new Dictionary<string, PricingFactory>(StringComparer.OrdinalIgnoreCase)
{
    { "standard", new StandardPricingFactory() },
    { "express", new ExpressPricingFactory() },
    { "overnight", new OvernightPricingFactory() }
};

// Din fiecare fabrica obtinem strategia de pret (OCP: un serviciu nou = o fabrica noua + o linie aici).
var services = factories.ToDictionary(
    kv => kv.Key,
    kv => kv.Value.CreatePricing(),
    StringComparer.OrdinalIgnoreCase);

// 2. Dependintele sunt date din exterior (DIP).
var delivery = new DeliveryService(new ConsoleNotifier(), services);
var labels = new LabelPrinter();

Console.WriteLine("=== SERVICIUL DE LIVRARE A COLETELOR ===");
Console.WriteLine("--------------------------------------------------------------");

Console.Write("Numele destinatarului: ");
string name = Console.ReadLine() ?? "";
Console.Write("E-mail destinatar: ");
string email = Console.ReadLine() ?? "";
Console.Write("Strada: ");
string street = Console.ReadLine() ?? "";
Console.Write("Oras: ");
string city = Console.ReadLine() ?? "";
Console.Write("Greutate (kg): ");
if (!double.TryParse(Console.ReadLine(), out double weight))
{
    Console.WriteLine("Eroare: greutatea trebuie sa fie un numar.");
    return;
}
Console.Write("Serviciu (standard / express / overnight): ");
string service = Console.ReadLine() ?? "";

Parcel parcel;
try
{
    parcel = delivery.Register(name, email, new Address(street, city), weight, service);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Eroare: {ex.Message}");
    return;
}

// 3. Eticheta (SRP) si parcurgerea etapelor.
labels.PrintLabel(parcel);

Console.WriteLine("\n=== PARCURGEREA ETAPELOR ===");
delivery.Advance(parcel.TrackingId, ParcelStatus.PickedUp);
delivery.Advance(parcel.TrackingId, ParcelStatus.InTransit);
delivery.Advance(parcel.TrackingId, ParcelStatus.Delivered);

Console.WriteLine($"\nStatus final: {parcel.Status}");
