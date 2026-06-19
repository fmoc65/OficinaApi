/// <summary>
/// Interface específica para o Repository de Cliente.
/// Estende IRepository com métodos específicos de Cliente se necessário.
/// Aplicamos SOLID (Liskov Substitution Principle) - pode substituir IRepository<Cliente, Guid>.
/// </summary>
namespace OficinaApi.Infrastructure.Repositories
{
    public interface IClienteRepository : IRepository<Domain.Entities.Cliente, Guid>
    {
        /// <summary>
        /// Busca um cliente por nome.
        /// Método específico do domínio de Cliente.
        /// </summary>
        Task<Domain.Entities.Cliente?> GetByNomeAsync(string nome);

        /// <summary>
        /// Busca um cliente por telefone.
        /// Útil para verificar duplicatas.
        /// </summary>
        Task<Domain.Entities.Cliente?> GetByTelefoneAsync(string telefone);

        /// <summary>
        /// Retorna clientes com seus carros e ordens de serviço.
        /// Carrega dados relacionados em uma única consulta (eager loading).
        /// </summary>
        Task<IEnumerable<Domain.Entities.Cliente>> GetAllWithRelationsAsync();
    }
}
