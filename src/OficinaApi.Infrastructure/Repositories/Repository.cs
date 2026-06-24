using Microsoft.EntityFrameworkCore;
using OficinaApi.Domain.Entities;
using OficinaApi.Infrastructure.Data;

/// <summary>
/// Implementação genérica do Repository para qualquer entidade.
/// Fornece operações CRUD padrão usando Entity Framework Core.
/// Aplicamos SOLID (Dependency Inversion) - depende da abstração (DbContext) injetada.
/// Esta classe implementa o padrão Repository, abstraindo acesso ao banco de dados.
/// </summary>
namespace OficinaApi.Infrastructure.Repositories
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
    {
        /// <summary>
        /// DbContext injetado via construtor.
        /// Utilizamos injeção de dependência para facilitar testes e flexibilidade.
        /// </summary>
        protected readonly OficinaDbContext _context;

        /// <summary>
        /// DbSet da entidade para operações CRUD.
        /// Acessível aos repositórios derivados.
        /// </summary>
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Construtor que recebe o DbContext.
        /// </summary>
        public Repository(OficinaDbContext context)
        {
            // Injeção de dependência do DbContext
            // Permite que o repository acesse o banco de dados
            _context = context;
            // Obtém o DbSet para a entidade genérica
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Busca uma entidade por Id de forma assincronamente.
        /// Operação assincronamente permite que threads não fiquem bloqueadas aguardando banco.
        /// </summary>
        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            // FindAsync usa cache do DbContext e é otimizada para busca por chave primária
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retorna todas as entidades assincronamente.
        /// Sem include de relações para evitar N+1 queries.
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            // ToListAsync converte a IQueryable em lista, executando a query de forma assincronamente
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Adiciona uma nova entidade.
        /// Não persiste no banco até SaveChangesAsync ser chamado.
        /// </summary>
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            // Adiciona à memória do context
            await _dbSet.AddAsync(entity);
            // Retorna a entidade para que possa ser usada após a adição
            return entity;
        }

        /// <summary>
        /// Atualiza uma entidade existente.
        /// O DbContext rastreia mudanças automaticamente.
        /// </summary>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            // Marca como modificado
            _dbSet.Update(entity);
            // Retorna a entidade atualizada
            return entity;
        }

        /// <summary>
        /// Deleta uma entidade por Id.
        /// </summary>
        public async Task<bool> DeleteAsync(TId id)
        {
            // Busca a entidade
            var entity = await _dbSet.FindAsync(id);
            
            if (entity == null)
            {
                // Não encontrou, retorna falso
                return false;
            }

            // Remove do DbSet
            _dbSet.Remove(entity);
            // Sucesso
            return true;
        }

        /// <summary>
        /// Verifica se uma entidade existe.
        /// </summary>
        public async Task<bool> ExistsAsync(TId id)
        {
            // Qualquer método que verifica existência sem carregar dados completos
            return await _dbSet.FindAsync(id) != null;
        }

        /// <summary>
        /// Salva as mudanças no banco de dados de forma assincronamente.
        /// Essencial após operações de modificação (Add, Update, Delete).
        /// Retorna o número de linhas afetadas.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            // SaveChangesAsync executa todas as operações pendentes no banco
            // Retorna quantas linhas foram afetadas
            return await _context.SaveChangesAsync();
        }
    }
}
