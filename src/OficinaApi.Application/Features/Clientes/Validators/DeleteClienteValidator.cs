using FluentValidation;

/// <summary>
/// Validator para DeleteClienteCommand.
/// Validações simples mas importantes para operações de deleção.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Validators
{
    public class DeleteClienteValidator : AbstractValidator<DeleteClienteCommand>
    {
        /// <summary>
        /// Construtor que define as regras de validação.
        /// </summary>
        public DeleteClienteValidator()
        {
            // Validação do Id
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do cliente é obrigatório para deleção"); // Precisamos saber qual cliente deletar
        }
    }
}
