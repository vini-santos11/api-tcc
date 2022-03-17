

using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Infra.Data.Context
{
    public class DBContext : IDBContext
    {
        public IDbConnection Connection { get; }

        public DBContext(IConfiguration config)
        {
            Connection = new MySqlConnection(config.GetConnectionString("Connection"));
            Connection.Open();
        }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();

            GC.SuppressFinalize(this);
        }
    }
}
