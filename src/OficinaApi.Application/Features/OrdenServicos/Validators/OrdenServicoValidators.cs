using FluentValidation;
using OficinaApi.Application.Features.OrdenServicos.Commands;

/// <summary>
/// Validators para Ordem de Serviço.
/// Validações mais complexas pois envolve múltiplas peças.
/// </summary>
namespace OficinaApi.Application.Features.OrdenServicos.Validators
{
    public class CreateOrdenServicoValidator : AbstractValidator<CreateOrdenServicoCommand>
    {
        public CreateOrdenServicoValidator()
        {
            // Validação do IdCarro
            RuleFor(x => x.IdCarro)
                .NotEmpty()
                .WithMessage("IdCarro é obrigatório"); // Ordem sempre associada a um carro

            // Validação do IdCliente
            RuleFor(x => x.IdCliente)
                .NotEmpty()
                .WithMessage("IdCliente é obrigatório"); // Ordem sempre associada a um cliente

            // Validação dos Serviços
            RuleFor(x => x.Servicos)
                .NotEmpty()
                .WithMessage("Descrição dos serviços é obrigatória") // Precisamos saber o que foi feito
                .MinimumLength(5)
                .WithMessage("Serviços deve ter pelo menos 5 caracteres")
                .MaximumLength(500)
                .WithMessage("Serviços não pode exceder 500 caracteres");

            // Validação das Peças
            RuleFor(x => x.Pecas)
                .NotNull()
                .WithMessage("Lista de peças é obrigatória")
                .NotEmpty()
                .WithMessage("Pelo menos uma peça deve ser adicionada"); // Uma ordem precisa ter peças

            // Validações individuais das peças
            RuleForEach(x => x.Pecas)
                .SetValidator(new PecaOrdenValidator());
        }
    }

    public class PecaOrdenValidator : AbstractValidator<PecaOrdenCommand>
    {
        public PecaOrdenValidator()
        {
            RuleFor(x => x.IdPeca)
                .NotEmpty()
                .WithMessage("IdPeca é obrigatório");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero");

            RuleFor(x => x.ValorUnitario)
                .GreaterThan(0)
                .WithMessage("ValorUnitario deve ser maior que zero");
        }
    }

    public class UpdateOrdenServicoValidator : AbstractValidator<UpdateOrdenServicoCommand>
    {
        public UpdateOrdenServicoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id da ordem é obrigatório");

            RuleFor(x => x.IdCarro)
                .NotEmpty()
                .WithMessage("IdCarro é obrigatório");

            RuleFor(x => x.IdCliente)
                .NotEmpty()
                .WithMessage("IdCliente é obrigatório");

            RuleFor(x => x.Servicos)
                .NotEmpty()
                .WithMessage("Descrição dos serviços é obrigatória")
                .MinimumLength(5)
                .WithMessage("Serviços deve ter pelo menos 5 caracteres")
                .MaximumLength(500)
                .WithMessage("Serviços não pode exceder 500 caracteres");

            // Validação de Status
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status é obrigatório")
                .Must(x => new[] { "Aberta", "Em Andamento", "Concluída", "Cancelada" }.Contains(x))
                .WithMessage("Status deve ser um dos valores válidos: Aberta, Em Andamento, Concluída, Cancelada");

            RuleFor(x => x.Pecas)
                .NotNull()
                .WithMessage("Lista de peças é obrigatória")
                .NotEmpty()
                .WithMessage("Pelo menos uma peça deve ser adicionada");

            RuleForEach(x => x.Pecas)
                .SetValidator(new PecaOrdenValidator());
        }
    }

    public class DeleteOrdenServicoValidator : AbstractValidator<DeleteOrdenServicoCommand>
    {
        public DeleteOrdenServicoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id da ordem é obrigatório para deleção");
        }
    }
}
