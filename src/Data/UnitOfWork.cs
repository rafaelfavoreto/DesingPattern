using src.Data.Repositories;

namespace src.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
        }

        private IDepartamentoRepository _departamentorepository;  

        public IDepartamentoRepository  DepartamentoRepository
        {
            //propriedade publica de get verifica se a instancia está null e sim cria uma nova 
            get => _departamentorepository ?? (_departamentorepository = new DepartamentoRepository(_context));
        }

        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}