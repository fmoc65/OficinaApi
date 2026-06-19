using FluentValidation;

/// <summary>
/// Validator para CreateClienteCommand.
/// Utilizamos FluentValidation para centralizar todas as regras de validação de cliente.
/// Isso segue o princípio SOLID (Single Responsibility) - validação separada da lógica de negócio.
/// FluentValidation fornece uma sintaxe fluente e reutilizável para validações.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Validators
{
    public class CreateClienteValidator : AbstractValidator<CreateClienteCommand>
    {
        /// <summary>
        /// Construtor que define as regras de validação.
        /// Cada regra valida um campo específico do cliente.
        /// </summary>
        public CreateClienteValidator()
        {
            // Validação do Nome
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome é obrigatório") // Campo obrigatório - cada cliente deve ter um nome
                .MinimumLength(3)
                .WithMessage("Nome deve ter pelo menos 3 caracteres") // Evita nomes muito curtos/inválidos
                .MaximumLength(150)
                .WithMessage("Nome não pode exceder 150 caracteres"); // Limita tamanho do campo

            // Validação do Telefone
            RuleFor(x => x.Telefone)
                .NotEmpty()
                .WithMessage("Telefone é obrigatório") // Campo obrigatório para contato
                .Matches(@"^\(?[1-9]{2}\)?[9]?[6-9]\d{3}-?\d{4}$")
                .WithMessage("Telefone deve estar em um formato válido (ex: (11)98765-4321)"); // Valida formato de telefone brasileiro

            // Validação do Endereço
            RuleFor(x => x.Endereco)
                .NotEmpty()
                .WithMessage("Endereço é obrigatório") // Campo obrigatório para localização
                .MinimumLength(5)
                .WithMessage("Endereço deve ter pelo menos 5 caracteres") // Evita endereços muito incompletos
                .MaximumLength(255)
                .WithMessage("Endereço não pode exceder 255 caracteres"); // Limita tamanho do campo
        }
    }
}
