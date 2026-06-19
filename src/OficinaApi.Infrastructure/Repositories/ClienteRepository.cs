using Microsoft.EntityFrameworkCore;
using OficinaApi.Domain.Entities;
using OficinaApi.Infrastructure.Data;

/// <summary>
/// Implementação específica do Repository para Cliente.
/// Estende o Repository genérico com métodos específicos do domínio de Cliente.
/// Aplicamos SOLID (Dependency Inversion e Interface Segregation).
/// </summary>
namespace OficinaApi.Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente, Guid>, IClienteRepository
    {
        /// <summary>
        /// Construtor que passa o DbContext ao Repository genérico.
        /// </summary>
        public ClienteRepository(OficinaDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Busca um cliente pelo nome.
        /// Implementação específica de negócio: buscar por campo não-chave.
        /// </summary>
        public async Task<Cliente?> GetByNomeAsync(string nome)
        {
            // LINQ para buscar cliente com nome exato (case-sensitive por padrão)
            // AsNoTracking porque não vamos modificar, apenas ler
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nome == nome);
        }

        /// <summary>
        /// Busca um cliente pelo telefone.
        /// Útil para validar duplicatas antes de criar novo cliente.
        /// </summary>
        public async Task<Cliente?> GetByTelefoneAsync(string telefone)
        {
            // LINQ para buscar cliente com telefone específico
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Telefone == telefone);
        }

        /// <summary>
        /// Retorna clientes com todas as suas relações carregadas.
        /// Eager loading (Include) traz dados relacionados em uma única query.
        /// Isso evita N+1 queries que ocorreriam se carregássemos lazy.
        /// </summary>
        public async Task<IEnumerable<Cliente>> GetAllWithRelationsAsync()
        {
            // Include carrega os Carros e OrdensServico de cada Cliente
            // Isso deixa a entidade rastreada para não usar AsNoTracking
            return await _dbSet
                .Include(c => c.Carros)
                .Include(c => c.OrdensServico)
                .ToListAsync();
        }
    }
}
