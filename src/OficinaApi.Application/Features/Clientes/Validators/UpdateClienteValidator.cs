using FluentValidation;

/// <summary>
/// Validator para UpdateClienteCommand.
/// Segue o mesmo padrão do CreateClienteValidator.
/// Reutilizamos a mesma lógica de validação para manter consistência (princípio DRY).
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Validators
{
    public class UpdateClienteValidator : AbstractValidator<UpdateClienteCommand>
    {
        /// <summary>
        /// Construtor que define as regras de validação para atualização.
        /// </summary>
        public UpdateClienteValidator()
        {
            // Validação do Id
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do cliente é obrigatório"); // Precisamos saber qual cliente atualizar

            // Validação do Nome
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome é obrigatório")
                .MinimumLength(3)
                .WithMessage("Nome deve ter pelo menos 3 caracteres")
                .MaximumLength(150)
                .WithMessage("Nome não pode exceder 150 caracteres");

            // Validação do Telefone
            RuleFor(x => x.Telefone)
                .NotEmpty()
                .WithMessage("Telefone é obrigatório")
                .Matches(@"^\(?[1-9]{2}\)?[9]?[6-9]\d{3}-?\d{4}$")
                .WithMessage("Telefone deve estar em um formato válido");

            // Validação do Endereço
            RuleFor(x => x.Endereco)
                .NotEmpty()
                .WithMessage("Endereço é obrigatório")
                .MinimumLength(5)
                .WithMessage("Endereço deve ter pelo menos 5 caracteres")
                .MaximumLength(255)
                .WithMessage("Endereço não pode exceder 255 caracteres");
        }
    }
}
