using CarvaoControl.Domain.Entities;

namespace CarvaoControl.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Produto? GetById(int id);
        IEnumerable<Produto> GetAll();
        void Add(Produto produto);
        void Update(Produto produto);
        void UpdateQuantidade(int id, int novaQuantidade);
    }
}
