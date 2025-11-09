using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Interfaces;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Infrastructure.Data.Sqlite
{
    public class SqliteVendaRepository : IVendaRepository
    {
        private readonly SqliteContext _context;

        public SqliteVendaRepository(SqliteContext context)
        {
            _context = context;
        }

        public void Add(Venda venda)
        {
            using var conn = _context.CreateConnection();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                // Primeiro verifica e atualiza o estoque
                cmd.CommandText = @"
UPDATE Produtos 
SET Quantidade = Quantidade - @vendaQuantidade
WHERE Id = @produtoId
  AND Quantidade >= @vendaQuantidade;";

                cmd.Parameters.AddWithValue("@produtoId", venda.ProdutoId);
                cmd.Parameters.AddWithValue("@vendaQuantidade", venda.Quantidade);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    transaction.Rollback();
                    throw new DomainException("Produto não encontrado ou estoque insuficiente.");
                }

                // Depois registra a venda
                cmd.CommandText = @"
INSERT INTO Vendas (ProdutoId, Quantidade, ValorTotal, Pagamento, Data)
VALUES (@produtoId, @quantidade, @valorTotal, @pagamento, @data);
SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@quantidade", venda.Quantidade);
                cmd.Parameters.AddWithValue("@valorTotal", venda.ValorTotal);
                cmd.Parameters.AddWithValue("@pagamento", (int)venda.Pagamento);
                cmd.Parameters.AddWithValue("@data", venda.Data);

                var id = Convert.ToInt32(cmd.ExecuteScalar());
                venda.DefinirId(id);

                transaction.Commit();
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
        }

        public IEnumerable<Venda> GetAll()
        {
            using var conn = _context.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT v.Id, v.ProdutoId, v.Quantidade, v.ValorTotal, v.Pagamento, v.Data,
       p.Preco as PrecoUnitario
FROM Vendas v
JOIN Produtos p ON p.Id = v.ProdutoId
ORDER BY v.Data DESC;";

            using var reader = cmd.ExecuteReader();
            var vendas = new List<Venda>();

            while (reader.Read())
            {
                var venda = new Venda(
                    reader.GetInt32(1), // ProdutoId
                    reader.GetInt32(2), // Quantidade
                    reader.GetDecimal(6), // PrecoUnitario
                    (TipoPagamento)reader.GetInt32(4));
                venda.DefinirId(reader.GetInt32(0));
                vendas.Add(venda);
            }

            return vendas;
        }

        public IEnumerable<Venda> GetByDateRange(DateTime start, DateTime end)
        {
            using var conn = _context.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT v.Id, v.ProdutoId, v.Quantidade, v.ValorTotal, v.Pagamento, v.Data,
       p.Preco as PrecoUnitario
FROM Vendas v
JOIN Produtos p ON p.Id = v.ProdutoId
WHERE v.Data BETWEEN @start AND @end
ORDER BY v.Data DESC;";

            cmd.Parameters.AddWithValue("@start", start.Date);
            cmd.Parameters.AddWithValue("@end", end.Date.AddDays(1).AddSeconds(-1));

            using var reader = cmd.ExecuteReader();
            var vendas = new List<Venda>();

            while (reader.Read())
            {
                var venda = new Venda(
                    reader.GetInt32(1), // ProdutoId
                    reader.GetInt32(2), // Quantidade
                    reader.GetDecimal(6), // PrecoUnitario
                    (TipoPagamento)reader.GetInt32(4));
                venda.DefinirId(reader.GetInt32(0));
                vendas.Add(venda);
            }

            return vendas;
        }
    }
}