using Product.Domain.Abstract;

namespace Order.Domain.Order
{
    public class OrderDb : BaseEntity
    {
    
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Status { get; set; }
 
    }
}
