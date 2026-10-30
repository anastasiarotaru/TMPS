namespace DeliverySOLID;

// BUILDER: construieste un Parcel pas cu pas, cu metode fluente.
// Verificarile se fac in Build(), inainte sa se creeze coletul.
public class ParcelBuilder
{
    private const double MaxWeightKg = 30.0;

    private string recipientName = "";
    private string recipientEmail = "";
    private Address? destination;
    private double weightKg;
    private string service = "standard";

    public ParcelBuilder To(string name, string email)
    {
        recipientName = name;
        recipientEmail = email;
        return this;
    }

    public ParcelBuilder Destination(Address address)
    {
        destination = address;
        return this;
    }

    public ParcelBuilder Weight(double kg)
    {
        weightKg = kg;
        return this;
    }

    public ParcelBuilder Service(string name)
    {
        service = name;
        return this;
    }

    // Verifica datele, cere un cod de la Singleton si calculeaza pretul cu strategia primita.
    public Parcel Build(IPricingStrategy pricing)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
        {
            throw new ArgumentException("Numele destinatarului lipseste.");
        }

        if (destination is null)
        {
            throw new ArgumentException("Adresa de livrare lipseste.");
        }

        if (weightKg <= 0 || weightKg > MaxWeightKg)
        {
            throw new ArgumentException($"Greutatea trebuie sa fie intre 0 si {MaxWeightKg} kg");
        }

        string trackingId = TrackingIdGenerator.Instance.Next();
        double price = pricing.CalculatePrice(weightKg);

        return new Parcel(trackingId, recipientName, recipientEmail,
                          destination, weightKg, service, price);
    }
}
