﻿using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Operation")]
    public class App_Operation : CreatedModel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
