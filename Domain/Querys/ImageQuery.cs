using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Querys
{
    public class ImageQuery
    {
        public long Id { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
