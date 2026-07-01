using FluentValidation;

namespace SmartInventory.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative.");
    }
}
