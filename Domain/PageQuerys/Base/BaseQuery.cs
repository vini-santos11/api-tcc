using BenMais.Domain.Helpers;
using BenMais.Domain.Page.Base;
using System;
using System.Text.Json.Serialization;

namespace Domain.PageQuerys.Base
{
    public class BaseQuery
    {
        private string _query = string.Empty;

        public string Query { get => ConcatQuery(_query); set => _query = value; }
        public string Order { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public EExecution Execution { get; set; } = EExecution.PAGE;
        public EPagination Pagination { get; set; } = EPagination.YES;
        public EOrientation Sort { get; set; } = EOrientation.ASC;
        [JsonIgnore]
        public string Text { get => _query; }

        public static string ConcatQuery(string value)
        {
            return string.Concat("%", value, "%");
        }

        private DateTime? _end;
        private DateTime? _start;

        protected DateTime? Start { get => _start.StartOfDay(); set => _start = value; }
        protected DateTime? End { get => _end.EndOfDay(); set => _end = value; }
    }
}
