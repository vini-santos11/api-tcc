using Domain.Enumerables;
using System.Collections.Generic;

namespace Domain.Commands.Inventory
{
    public class ProductInventoryCommand
    {
        public long? ContactId { get; set; }
        public EOperation Operation { get; set; }
        public List<InsertProductsCommand> Products { get; set; }
    }
}
