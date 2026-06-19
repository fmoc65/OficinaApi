/// <summary>
/// Entidade Cliente do domínio.
/// Representa um cliente da oficina com seus dados básicos.
/// Utilizamos SOLID (Single Responsibility) - esta classe é responsável apenas pela representação de Cliente.
/// </summary>
namespace OficinaApi.Domain.Entities
{
    public class Cliente : Common.BaseEntity
    {
        /// <summary>
        /// Nome completo do cliente.
        /// Campo obrigatório pois todo cliente deve ter um nome identificável.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Telefone do cliente para contato.
        /// Essencial para comunicação com o cliente sobre serviços e ordens de serviço.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// Endereço do cliente.
        /// Importante para localização do cliente e possível atendimento no local.
        /// </summary>
        public string Endereco { get; set; } = string.Empty;

        /// <summary>
        /// Coleção de carros pertencentes ao cliente.
        /// Relacionamento 1:N - um cliente pode ter múltiplos carros.
        /// </summary>
        public ICollection<Carro> Carros { get; set; } = new List<Carro>();

        /// <summary>
        /// Coleção de ordens de serviço do cliente.
        /// Relacionamento 1:N - um cliente pode ter múltiplas ordens de serviço.
        /// </summary>
        public ICollection<OrdenServico> OrdensServico { get; set; } = new List<OrdenServico>();
    }
}
