/// <summary>
/// Classe base para todas as entidades do domínio.
/// Utilizamos uma classe base para aplicar o princípio DRY (Don't Repeat Yourself),
/// centralizando propriedades comuns a todas as entidades como Id, DataCriacao e DataAtualizacao.
/// Isso facilita manutenção e evolução do código, além de garantir consistência.
/// </summary>
namespace OficinaApi.Domain.Common
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador único da entidade.
        /// Utilizamos como chave primária para garantir a unicidade de cada registro no banco de dados.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data de criação do registro.
        /// Necessária para auditoria e rastreamento de quando o registro foi criado no sistema.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização do registro.
        /// Essencial para auditoria e para identificar quando houve mudanças no registro.
        /// </summary>
        public DateTime? DataAtualizacao { get; set; }
    }
}
