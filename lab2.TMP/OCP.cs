namespace DeliverySOLID;

// Abstractie: orice regula de pret implementeaza aceasta interfata.
public interface IPricingStrategy
{
    double CalculatePrice(double weightKg);
}

// Livrare standard: 5 lei taxa de baza + 1,50 lei pe kg.
public class StandardPricing : IPricingStrategy
{
    public double CalculatePrice(double weightKg) => Math.Round(5.0 + 1.5 * weightKg, 2);
}

// Livrare express: pretul standard dublat.
public class ExpressPricing : IPricingStrategy
{
    private readonly IPricingStrategy baseStrategy;

    public ExpressPricing(IPricingStrategy baseStrategy)
    {
        this.baseStrategy = baseStrategy;
    }

    public double CalculatePrice(double weightKg) =>
        Math.Round(baseStrategy.CalculatePrice(weightKg) * 2.0, 2);
}

// Livrare in ziua urmatoare: pretul de baza plus un supliment fix de 15 lei.
public class OvernightPricing : IPricingStrategy
{
    private readonly IPricingStrategy baseStrategy;

    public OvernightPricing(IPricingStrategy baseStrategy)
    {
        this.baseStrategy = baseStrategy;
    }

    public double CalculatePrice(double weightKg) =>
        Math.Round(baseStrategy.CalculatePrice(weightKg) + 15.0, 2);
}
