namespace Stock_Managemnet.Models
{
    public enum ProductType
    {
        FG = 0,
        RawMaterial = 1
    }

    public static class ProductTypeLabels
    {
        public const string FinishedGoods = "FG";
        public const string RawMaterial = "Raw Material";

        public static string ToLabel(ProductType type) =>
            type == ProductType.RawMaterial ? RawMaterial : FinishedGoods;

        public static ProductType FromLabel(string label)
        {
            if (string.Equals(label, RawMaterial, System.StringComparison.OrdinalIgnoreCase))
                return ProductType.RawMaterial;

            return ProductType.FG;
        }
    }
}
