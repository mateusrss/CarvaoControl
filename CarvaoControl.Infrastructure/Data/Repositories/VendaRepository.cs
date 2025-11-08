using CarvaoControl.Domain.Entities;

namespace CarvaoControl.Domain.Interfaces
{
    public interface IVendaRepository
    {
        IEnumerable<Venda> Listar();
        void Adicionar(Venda venda);
        IEnumerable<Venda> ListarPorData(DateTime data);
    }
}
