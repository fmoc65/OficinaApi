/// <summary>
/// Entidade de Associação OrdenServicoPeca.
/// Esta classe representa o relacionamento N:N entre OrdenServico e Peca.
/// Utilizamos uma tabela de junção explícita para permitir dados adicionais como quantidade e valor nesta associação.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas pela representação desta relação.
/// </summary>
namespace OficinaApi.Domain.Entities
{
    public class OrdenServicoPeca : Common.BaseEntity
    {
        /// <summary>
        /// Identificador da ordem de serviço.
        /// Chave estrangeira que estabelece o relacionamento com OrdenServico.
        /// </summary>
        public Guid IdOrdenServico { get; set; }

        /// <summary>
        /// Identificador da peça utilizada.
        /// Chave estrangeira que estabelece o relacionamento com Peca.
        /// </summary>
        public Guid IdPeca { get; set; }

        /// <summary>
        /// Quantidade de peças utilizadas nesta ordem de serviço.
        /// Importante para controle de consumo de estoque.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário da peça no momento da ordem de serviço.
        /// Mantemos este valor aqui para fins de auditoria histórica,
        /// pois o valor da peça pode mudar no futuro.
        /// </summary>
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Referência de navegação para a ordem de serviço.
        /// Facilita o acesso aos dados da ordem sem consulta adicional.
        /// </summary>
        public OrdenServico? OrdenServico { get; set; }

        /// <summary>
        /// Referência de navegação para a peça.
        /// Facilita o acesso aos dados da peça sem consulta adicional.
        /// </summary>
        public Peca? Peca { get; set; }
    }
}
