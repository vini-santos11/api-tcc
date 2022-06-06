using Domain.Attributes;
using System;

namespace Domain.Models.Base
{
    public abstract class UpdatedModel : CreatedModel
    {
        [Updated]
        public long? UpdatedBy { get; set; }
        [Updated]
        public DateTime? UpdatedAt { get; set; }
    }
}
