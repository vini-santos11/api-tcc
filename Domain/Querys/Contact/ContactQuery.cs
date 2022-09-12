using Domain.Attributes;
using Domain.Converters;
using System;
using System.Text.Json.Serialization;

namespace Domain.Querys.Contact
{
    public class ContactQuery
    {
        public long Id { get; set; }
        [Excel(ColumnTitle = "Nome")]
        public string Name { get; set; }
        [Excel(ColumnTitle = "Sexo")]
        public string Gender { get; set; }
        [Excel(ColumnTitle = "N° Documento")]
        public string DocumentNumber { get; set; }
        [Excel(ColumnTitle = "Data de Nascimento")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? BirthDate { get; set; }
        [Excel(ColumnTitle = "Telefone")]
        public string Phone { get; set; }
        [Excel(ColumnTitle = "Endereço")]
        public string Address { get; set; }
        [Excel(ColumnTitle = "Email")]
        public string Email { get; set; }
        [Excel(ColumnTitle = "Fisíca ou Jurídica")]
        public string PersonType { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
    }
}
