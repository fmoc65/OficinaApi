/// <summary>
/// Entidade Ordem de Serviço do domínio.
/// Representa um serviço realizado pela oficina para um cliente em um carro específico.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas pela representação de OrdenServico.
/// </summary>
namespace OficinaApi.Domain.Entities
{
    public class OrdenServico : Common.BaseEntity
    {
        /// <summary>
        /// Identificador do carro para o qual o serviço é realizado.
        /// Chave estrangeira que estabelece o relacionamento com Carro (N:1).
        /// </summary>
        public Guid IdCarro { get; set; }

        /// <summary>
        /// Identificador do cliente proprietário do carro.
        /// Chave estrangeira que estabelece o relacionamento com Cliente (N:1).
        /// Denormalizado aqui para facilitar consultas e acesso direto.
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Descrição dos serviços a serem realizados.
        /// Campo obrigatório que detalha quais trabalhos devem ser executados.
        /// </summary>
        public string Servicos { get; set; } = string.Empty;

        /// <summary>
        /// Valor total da ordem de serviço.
        /// Calculado como somatório de mão de obra + peças utilizadas.
        /// </summary>
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Data em que a ordem de serviço foi criada.
        /// Herdada de BaseEntity, essencial para rastreamento.
        /// </summary>
        public DateTime DataOrdem { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Status da ordem de serviço (Aberta, Em Andamento, Concluída, etc).
        /// Importante para controle do workflow da ordem.
        /// </summary>
        public string Status { get; set; } = "Aberta";

        /// <summary>
        /// Referência de navegação para o carro.
        /// Facilita o acesso aos dados do carro sem consulta adicional.
        /// </summary>
        public Carro? Carro { get; set; }

        /// <summary>
        /// Referência de navegação para o cliente.
        /// Facilita o acesso aos dados do cliente sem consulta adicional.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Coleção de peças utilizadas nesta ordem de serviço.
        /// Relacionamento N:N através de OrdenServicoPeca.
        /// </summary>
        public ICollection<OrdenServicoPeca> PecasUtilizadas { get; set; } = new List<OrdenServicoPeca>();
    }
}
