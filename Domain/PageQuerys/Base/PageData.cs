using System.Collections.Generic;

namespace Domain.Page.Base
{
    public class PageData<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}