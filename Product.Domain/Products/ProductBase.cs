using Product.Domain.Abstract;

namespace Product.Domain.Entities
{
    public class ProductBase : BaseEntity
    {
        public string Name { get; set; } 
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
