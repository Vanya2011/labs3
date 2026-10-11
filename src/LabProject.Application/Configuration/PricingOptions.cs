namespace LabProject.Application.Configuration
{
    public class PricingOptions
    {
        public const string SectionName = "Pricing";
        public bool EnableGlobalDiscount { get; set; } = false;
        public decimal GlobalDiscountPercentage { get; set; } = 0m;
        public decimal MinimumPriceForDiscount { get; set; } = 0m;
    }
}
