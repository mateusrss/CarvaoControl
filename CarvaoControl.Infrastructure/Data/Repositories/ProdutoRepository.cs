using CarvaoControl.Domain.Entities;

namespace CarvaoControl.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        IEnumerable<Produto> Listar();
        Produto? ObterPorId(int id);
        void Adicionar(Produto produto);
        void Atualizar(Produto produto);
        void Remover(int id);
    }
}
