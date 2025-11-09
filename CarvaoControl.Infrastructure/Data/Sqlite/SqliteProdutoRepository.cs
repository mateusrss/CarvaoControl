using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Interfaces;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Infrastructure.Data.Sqlite
{
    public class SqliteProdutoRepository : IProdutoRepository
    {
        private readonly SqliteContext _context;

        public SqliteProdutoRepository(SqliteContext context)
        {
            _context = context;
        }

        public void Add(Produto produto)
        {
            using var conn = _context.CreateConnection();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = @"
INSERT INTO Produtos (Nome, Preco, Quantidade)
VALUES (@nome, @preco, @quantidade);
SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@nome", produto.Nome);
                cmd.Parameters.AddWithValue("@preco", produto.Preco);
                cmd.Parameters.AddWithValue("@quantidade", produto.Quantidade);

                var id = Convert.ToInt32(cmd.ExecuteScalar());
                produto.DefinirId(id);

                transaction.Commit();
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
        }

        public void Update(Produto produto)
        {
            using var conn = _context.CreateConnection();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = @"
UPDATE Produtos 
SET Nome = @nome, Preco = @preco, Quantidade = @quantidade
WHERE Id = @id;";

                cmd.Parameters.AddWithValue("@id", produto.Id);
                cmd.Parameters.AddWithValue("@nome", produto.Nome);
                cmd.Parameters.AddWithValue("@preco", produto.Preco);
                cmd.Parameters.AddWithValue("@quantidade", produto.Quantidade);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    transaction.Rollback();
                    throw new DomainException("Produto não encontrado.");
                }

                transaction.Commit();
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
        }

        public IEnumerable<Produto> GetAll()
        {
            using var conn = _context.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT Id, Nome, Preco, Quantidade 
FROM Produtos 
ORDER BY Nome;";

            using var reader = cmd.ExecuteReader();
            var produtos = new List<Produto>();

            while (reader.Read())
            {
                var produto = new Produto(
                    reader.GetString(1),
                    reader.GetDecimal(2),
                    reader.GetInt32(3));
                produto.DefinirId(reader.GetInt32(0));
                produtos.Add(produto);
            }

            return produtos;
        }

        public Produto? GetById(int id)
        {
            using var conn = _context.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT Id, Nome, Preco, Quantidade 
FROM Produtos 
WHERE Id = @id;";

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var produto = new Produto(
                reader.GetString(1),
                reader.GetDecimal(2),
                reader.GetInt32(3));
            produto.DefinirId(reader.GetInt32(0));
            return produto;
        }

        public void UpdateQuantidade(int id, int novaQuantidade)
        {
            using var conn = _context.CreateConnection();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = @"
UPDATE Produtos 
SET Quantidade = @quantidade
WHERE Id = @id
  AND @quantidade >= 0;"; // Garantia adicional além do CHECK constraint

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@quantidade", novaQuantidade);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    transaction.Rollback();
                    throw new DomainException("Produto não encontrado ou quantidade inválida.");
                }

                transaction.Commit();
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
        }
    }
}