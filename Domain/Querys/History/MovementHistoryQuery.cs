using System;

namespace Domain.Querys.History
{
    public class MovementHistoryQuery
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Contact { get; set; }
        public DateTime BuyDate { get; set; }
    }
}
