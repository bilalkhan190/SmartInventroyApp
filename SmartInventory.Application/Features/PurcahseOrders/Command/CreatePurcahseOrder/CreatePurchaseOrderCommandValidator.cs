using FluentValidation;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;

public sealed class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(command => command.SupplierId)
            .NotEmpty().WithMessage("Supplier is required.");

        RuleFor(command => command.Products)
            .NotEmpty().WithMessage("At least one product is required.");

        RuleFor(command => command.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
            .When(command => !string.IsNullOrWhiteSpace(command.Notes));

        RuleForEach(command => command.Products).ChildRules(product =>
        {
            product.RuleFor(item => item.ProductId)
                .NotEmpty().WithMessage("Product is required.");

            product.RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            product.RuleFor(item => item.UnitAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Unit amount cannot be negative.");
        });

        RuleFor(command => command.Products)
            .Must(products => products.Select(product => product.ProductId).Distinct().Count() == products.Count)
            .WithMessage("Duplicate products are not allowed in the same purchase order.")
            .When(command => command.Products.Count > 0);
    }
}
