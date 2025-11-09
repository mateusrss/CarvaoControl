using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Exceptions;
using CarvaoControl.Domain.Services;
using CarvaoControl.Infrastructure.Services;
using System;

namespace CarvaoControl.Application.Services
{
    public class EstoqueAppService
    {
        private readonly EstoqueService _estoqueService;
        private readonly LoggingService _log;
        private readonly PersistenceService? _persistence;

        public EstoqueAppService(EstoqueService estoqueService, LoggingService log, PersistenceService? persistence = null)
        {
            _estoqueService = estoqueService;
            _log = log;
            _persistence = persistence;
        }

        public void AjustarEstoque(int produtoId, int novaQuantidade, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new DomainException("É necessário informar o motivo do ajuste.");

            var produto = _estoqueService.ListarProdutos().FirstOrDefault(p => p.Id == produtoId)
                ?? throw new DomainException("Produto não encontrado.");

            var quantidadeAnterior = produto.Quantidade;
            var diferenca = novaQuantidade - quantidadeAnterior;

            if (diferenca > 0)
            {
                produto.AdicionarEstoque(diferenca);
            }
            else if (diferenca < 0)
            {
                produto.ReduzirEstoque(Math.Abs(diferenca));
            }

            try
            {
                _persistence?.SaveProdutos(_estoqueService.ListarProdutos());
                
                _log.LogAudit(
                    "AjusteEstoque",
                    $"Produto: {produto.Nome} (ID: {produto.Id}), " +
                    $"Quantidade: {quantidadeAnterior} → {novaQuantidade}, " +
                    $"Motivo: {motivo}"
                );
            }
            catch (Exception ex)
            {
                _log.LogError("Falha ao salvar ajuste de estoque", ex);
                throw;
            }
        }
    }
}