/// <summary>
/// Entidade Carro do domínio.
/// Representa um veículo cadastrado na oficina.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas pela representação de Carro.
/// </summary>
namespace OficinaApi.Domain.Entities
{
    public class Carro : Common.BaseEntity
    {
        /// <summary>
        /// Modelo/marca do carro.
        /// Campo obrigatório para identificação do veículo.
        /// </summary>
        public string Modelo { get; set; } = string.Empty;

        /// <summary>
        /// Ano de fabricação do carro.
        /// Importante para diagnóstico de problemas e compatibilidade de peças.
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// Identificador do cliente proprietário do carro.
        /// Chave estrangeira que estabelece o relacionamento com Cliente (N:1).
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Referência de navegação para o cliente proprietário.
        /// Facilita o acesso aos dados do cliente sem consulta adicional.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Coleção de peças associadas a este carro.
        /// Relacionamento 1:N - um carro pode ter múltiplas peças cadastradas.
        /// </summary>
        public ICollection<Peca> Pecas { get; set; } = new List<Peca>();

        /// <summary>
        /// Coleção de ordens de serviço para este carro.
        /// Relacionamento 1:N - um carro pode ter múltiplas ordens de serviço.
        /// </summary>
        public ICollection<OrdenServico> OrdensServico { get; set; } = new List<OrdenServico>();
    }
}
