using System;
using System.Collections.Generic;

namespace Domain.Exceptions
{
    public class ConflictedException : Exception
    {
        private readonly IEnumerable<object> _conflicts;

        public ConflictedException(string message, IEnumerable<object> conflicts) : base(message)
        {
            _conflicts = conflicts;
        }

        public IEnumerable<object> Conflicts()
        {
            return _conflicts;
        }
    }
}
