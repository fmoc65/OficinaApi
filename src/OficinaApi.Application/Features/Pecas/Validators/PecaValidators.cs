using FluentValidation;
using OficinaApi.Application.Features.Pecas.Commands;

/// <summary>
/// Validators para Peça.
/// Centraliza todas as regras de validação.
/// </summary>
namespace OficinaApi.Application.Features.Pecas.Validators
{
    public class CreatePecaValidator : AbstractValidator<CreatePecaCommand>
    {
        public CreatePecaValidator()
        {
            // Validação do IdPeca
            RuleFor(x => x.IdPeca)
                .NotEmpty()
                .WithMessage("IdPeca é obrigatório") // Identificador único da peça no catálogo
                .MinimumLength(1)
                .WithMessage("IdPeca não pode ser vazio")
                .MaximumLength(50)
                .WithMessage("IdPeca não pode exceder 50 caracteres");

            // Validação do IdCarro
            RuleFor(x => x.IdCarro)
                .NotEmpty()
                .WithMessage("IdCarro é obrigatório"); // Peça sempre pertence a um carro

            // Validação da Quantidade
            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero"); // Não faz sentido estoque zero

            // Validação do Valor
            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("Valor deve ser maior que zero"); // Peça sempre tem custo positivo
        }
    }

    public class UpdatePecaValidator : AbstractValidator<UpdatePecaCommand>
    {
        public UpdatePecaValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id da peça é obrigatório");

            RuleFor(x => x.IdPeca)
                .NotEmpty()
                .WithMessage("IdPeca é obrigatório")
                .MaximumLength(50)
                .WithMessage("IdPeca não pode exceder 50 caracteres");

            RuleFor(x => x.IdCarro)
                .NotEmpty()
                .WithMessage("IdCarro é obrigatório");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("Valor deve ser maior que zero");
        }
    }

    public class DeletePecaValidator : AbstractValidator<DeletePecaCommand>
    {
        public DeletePecaValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id da peça é obrigatório para deleção");
        }
    }
}
