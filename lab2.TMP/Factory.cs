namespace DeliverySOLID;

// FACTORY METHOD: clasa de baza declara metoda de creare.
// Fiecare subclasa decide ce strategie de pret se creeaza.
public abstract class PricingFactory
{
    public abstract IPricingStrategy CreatePricing();
}

public class StandardPricingFactory : PricingFactory
{
    public override IPricingStrategy CreatePricing() => new StandardPricing();
}

public class ExpressPricingFactory : PricingFactory
{
    public override IPricingStrategy CreatePricing() =>
        new ExpressPricing(new StandardPricing());
}

public class OvernightPricingFactory : PricingFactory
{
    public override IPricingStrategy CreatePricing() =>
        new OvernightPricing(new StandardPricing());
}
