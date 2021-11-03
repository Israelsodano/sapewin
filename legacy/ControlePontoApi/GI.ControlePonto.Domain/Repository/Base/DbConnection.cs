using System;
using System.Data;

namespace GI.ControlePonto.Domain.Repository.Base
{
    public class DbConnection<T> : IDisposable where T : ContextBase<T>
    {
        private readonly IDbConnection connection;
        public DbConnection(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void Dispose() =>
            connection.Dispose();

        public IDbConnection GetConnection() =>
            connection;
    }
}