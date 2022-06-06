using Domain.Attributes;
using System;

namespace Domain.Models.Base
{
    public abstract class CreatedModel
    {
        [Created]
        public long? CreatedBy { get; set; }
        [Created]
        public DateTime? CreatedAt { get; set; }
    }
}
