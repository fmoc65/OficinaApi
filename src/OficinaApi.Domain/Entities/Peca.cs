/// <summary>
/// Entidade Peça do domínio.
/// Representa uma peça de reposição cadastrada para um carro.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas pela representação de Peça.
/// </summary>
namespace OficinaApi.Domain.Entities
{
    public class Peca : Common.BaseEntity
    {
        /// <summary>
        /// Identificador único da peça no catálogo.
        /// Campo obrigatório para controle de inventário.
        /// </summary>
        public string IdPeca { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do carro ao qual a peça pertence.
        /// Chave estrangeira que estabelece o relacionamento com Carro (N:1).
        /// </summary>
        public Guid IdCarro { get; set; }

        /// <summary>
        /// Quantidade de peças em estoque.
        /// Essencial para controle de inventário e reposição.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário da peça.
        /// Necessário para cálculo de custos em ordens de serviço.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Referência de navegação para o carro.
        /// Facilita o acesso aos dados do carro sem consulta adicional.
        /// </summary>
        public Carro? Carro { get; set; }

        /// <summary>
        /// Coleção de ordens de serviço que utilizam esta peça.
        /// Relacionamento N:N através de OrdenServicoPeca.
        /// </summary>
        public ICollection<OrdenServicoPeca> OrdensServicoPecas { get; set; } = new List<OrdenServicoPeca>();
    }
}
