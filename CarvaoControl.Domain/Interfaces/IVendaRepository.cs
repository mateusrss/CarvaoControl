using CarvaoControl.Domain.Entities;

namespace CarvaoControl.Domain.Interfaces
{
    public interface IVendaRepository
    {
        IEnumerable<Venda> GetAll();
        void Add(Venda venda);
        IEnumerable<Venda> GetByDateRange(DateTime start, DateTime end);
    }
}