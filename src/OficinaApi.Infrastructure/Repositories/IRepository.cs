/// <summary>
/// Interface genérica IRepository para operações CRUD base.
/// Utilizamos uma interface genérica para aplicar o princípio DRY (Don't Repeat Yourself).
/// Cada repository específico herda dessa interface, reutilizando métodos comuns.
/// O padrão Repository abstrai o acesso ao banco de dados, permitindo testes unitários e flexibilidade.
/// </summary>
namespace OficinaApi.Infrastructure.Repositories
{
    public interface IRepository<TEntity, in TId> where TEntity : class
    {
        /// <summary>
        /// Busca uma entidade por Id.
        /// </summary>
        Task<TEntity?> GetByIdAsync(TId id);

        /// <summary>
        /// Retorna todas as entidades.
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Adiciona uma nova entidade.
        /// </summary>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Atualiza uma entidade existente.
        /// </summary>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Deleta uma entidade.
        /// </summary>
        Task<bool> DeleteAsync(TId id);

        /// <summary>
        /// Verifica se uma entidade existe.
        /// </summary>
        Task<bool> ExistsAsync(TId id);

        /// <summary>
        /// Salva as mudanças no banco de dados.
        /// Necessário após operações de modificação.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
