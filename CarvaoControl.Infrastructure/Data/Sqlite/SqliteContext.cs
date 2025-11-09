using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using CarvaoControl.Domain.Interfaces;
using CarvaoControl.Domain.Entities;
using System.Collections.Generic;
using System;
using System.IO;

namespace CarvaoControl.Infrastructure.Data.Sqlite
{
    public class SqliteContext : IDisposable
    {
        private readonly string _connectionString;
        private static readonly object _lock = new object();
        private static bool _initialized;
        private SqliteConnection? _sharedConnection;

        public SqliteContext(string? basePath = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbPath = Path.Combine(basePath ?? Path.Combine(appData, "CarvaoControl"), "carvao.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            _connectionString = $"Data Source={dbPath}";
            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                using var transaction = conn.BeginTransaction();

                try
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Produtos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL,
    Preco DECIMAL(10,2) NOT NULL CHECK (Preco >= 0),
    Quantidade INTEGER NOT NULL CHECK (Quantidade >= 0)
);

CREATE TABLE IF NOT EXISTS Vendas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProdutoId INTEGER NOT NULL,
    Quantidade INTEGER NOT NULL CHECK (Quantidade > 0),
    ValorTotal DECIMAL(10,2) NOT NULL CHECK (ValorTotal >= 0),
    Pagamento INTEGER NOT NULL CHECK (Pagamento IN (0, 1, 2)),
    Data DATETIME NOT NULL,
    FOREIGN KEY(ProdutoId) REFERENCES Produtos(Id)
);

CREATE INDEX IF NOT EXISTS idx_vendas_data ON Vendas(Data);
CREATE INDEX IF NOT EXISTS idx_vendas_produto ON Vendas(ProdutoId);
";
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    _initialized = true;
                }
                catch
                {
                    try { transaction.Rollback(); } catch { }
                    throw;
                }
            }
        }

        public SqliteConnection CreateConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            try
            {
                conn.Open();
                return conn;
            }
            catch
            {
                conn.Dispose();
                throw;
            }
        }

        public SqliteConnection GetSharedConnection()
        {
            if (_sharedConnection?.State != System.Data.ConnectionState.Open)
            {
                _sharedConnection?.Dispose();
                _sharedConnection = CreateConnection();
            }
            return _sharedConnection;
        }

        public void Dispose()
        {
            if (_sharedConnection != null)
            {
                try { _sharedConnection.Close(); } catch { }
                try { _sharedConnection.Dispose(); } catch { }
                _sharedConnection = null;
            }
        }
    }
}