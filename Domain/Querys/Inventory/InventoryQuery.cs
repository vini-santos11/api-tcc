using Domain.Attributes;
using Domain.Converters;
using System;
using System.Text.Json.Serialization;

namespace Domain.Querys.Inventory
{
    public class InventoryQuery
    {
        public long ProductId { get; set; }
        [Excel(ColumnTitle = "Produto")]
        public string Name { get; set; }
        [Excel(ColumnTitle = "Quantidade")]
        public decimal Amount { get; set; }
        [Excel(ColumnTitle = "Data de atualização")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime LastUpdate { get; set; }
    }
}
