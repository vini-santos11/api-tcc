using Domain.Attributes;
using System;

namespace Domain.Querys.Product
{
    public class ProductQuery
    {
        public long Id { get; set; }
        [Excel(ColumnTitle = "Nome")]
        public string Name { get; set; }
        [Excel(ColumnTitle = "Descrição")]
        public string Description { get; set; }
        [Excel(ColumnTitle = "Unidade de Medida Padrão")]
        public string DefaultMeansurement { get; set; }
        [Excel(ColumnTitle = "Preço", IsCurrency = true, Decimals = 2)]
        public decimal Price { get; set; }
        public string ImageName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
