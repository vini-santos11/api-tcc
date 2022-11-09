namespace Domain.Commands.Inventory
{
    public class InsertProductsCommand
    {
        public long ProductId { get; set; }
        public decimal Amount { get; set; }
    }
}
