using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models
{
    [Table("db_tcc.App_Contact")]
    public class AppContact : UpdatedModel
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Gender { get; set; }
        public string DocumentNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime Birthdate { get; set; }
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public int PersonTypeId { get; set; }
    }
}
