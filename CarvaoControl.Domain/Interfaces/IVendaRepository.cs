using CarvaoControl.Domain.Entities;

namespace CarvaoControl.Domain.Interfaces
{
    public interface IVendaRepository
    {
        Venda? ObterPorId(int id);
        IEnumerable<Venda> ObterTods();
        void Adcionar(Venda venda);
        void Remover(int id);
    }
}