using System;

namespace Domain.Querys.History
{
    public class MovementHistoryQuery
    {
        public long ProductId { get; set; }
        public string Product { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BuyDate { get; set; }
    }
}
