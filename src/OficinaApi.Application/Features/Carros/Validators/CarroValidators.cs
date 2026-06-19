using FluentValidation;
using OficinaApi.Application.Features.Carros.Commands;

/// <summary>
/// Validators para commands de Carro.
/// Utilizamos FluentValidation para centralizar validações.
/// Aplicamos SOLID (Single Responsibility) - validação separada da lógica.
/// </summary>
namespace OficinaApi.Application.Features.Carros.Validators
{
    public class CreateCarroValidator : AbstractValidator<CreateCarroCommand>
    {
        /// <summary>
        /// Define regras de validação para criação de carro.
        /// </summary>
        public CreateCarroValidator()
        {
            // Validação do Modelo
            RuleFor(x => x.Modelo)
                .NotEmpty()
                .WithMessage("Modelo é obrigatório") // Todo carro precisa ter modelo definido
                .MinimumLength(2)
                .WithMessage("Modelo deve ter pelo menos 2 caracteres") // Evita valores inválidos
                .MaximumLength(100)
                .WithMessage("Modelo não pode exceder 100 caracteres"); // Limita tamanho

            // Validação do Ano
            RuleFor(x => x.Ano)
                .GreaterThan(1900)
                .WithMessage("Ano deve ser maior que 1900") // Veículos modernos começam por volta de 1900
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
                .WithMessage($"Ano não pode ser superior a {DateTime.UtcNow.Year + 1}"); // Não pode ser futuro muito distante

            // Validação do IdCliente
            RuleFor(x => x.IdCliente)
                .NotEmpty()
                .WithMessage("IdCliente é obrigatório"); // Carro sempre pertence a um cliente
        }
    }

    public class UpdateCarroValidator : AbstractValidator<UpdateCarroCommand>
    {
        /// <summary>
        /// Define regras de validação para atualização de carro.
        /// </summary>
        public UpdateCarroValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do carro é obrigatório");

            RuleFor(x => x.Modelo)
                .NotEmpty()
                .WithMessage("Modelo é obrigatório")
                .MinimumLength(2)
                .WithMessage("Modelo deve ter pelo menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("Modelo não pode exceder 100 caracteres");

            RuleFor(x => x.Ano)
                .GreaterThan(1900)
                .WithMessage("Ano deve ser maior que 1900")
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
                .WithMessage($"Ano não pode ser superior a {DateTime.UtcNow.Year + 1}");

            RuleFor(x => x.IdCliente)
                .NotEmpty()
                .WithMessage("IdCliente é obrigatório");
        }
    }

    public class DeleteCarroValidator : AbstractValidator<DeleteCarroCommand>
    {
        /// <summary>
        /// Define regras de validação para deleção de carro.
        /// </summary>
        public DeleteCarroValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do carro é obrigatório para deleção");
        }
    }
}
