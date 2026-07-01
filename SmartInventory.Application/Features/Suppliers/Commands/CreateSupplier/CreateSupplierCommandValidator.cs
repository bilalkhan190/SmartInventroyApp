using FluentValidation;

namespace SmartInventory.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(x => x.ContactName)
            .MaximumLength(200).WithMessage("Contact name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is not valid.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Email)
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
    }
}
