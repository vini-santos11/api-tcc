using System;
using System.Data;

namespace Domain.Interfaces
{
    public interface IDBContext : IDisposable
    {
        IDbConnection Connection { get; }
    }
}
