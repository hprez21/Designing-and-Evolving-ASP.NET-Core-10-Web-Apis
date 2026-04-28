namespace Globomantics.API.Antipatterns
{
    public class BrittleProductDto
    {        
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? ProductDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string? InternalNotes { get; set; }
        public DateTime RowVersion { get; set; }
        
        public ProductType Type { get; set; }
        
        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierEmail { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new();
        public List<OrderHistoryDto> OrderHistory { get; set; } = new();
        public InventoryDto? Inventory { get; set; }
        public ShippingDto? Shipping { get; set; }
    }
    
    public enum ProductType
    {
        Physical = 1,
        Digital = 2,
        Subscription = 3        
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
    }

    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class InventoryDto
    {
        public int Quantity { get; set; }
    }

    public class ShippingDto
    {
        public decimal Weight { get; set; }
    }
}
