using Domain.Enumerables;

namespace Domain.Commands.Inventory
{
    public class ProductInventoryCommand
    {
        public long ProductId { get; set; }
        public long? ContactId { get; set; }
        public decimal Amount { get; set; }
        public EOperation Operation { get; set; }
    }
}
