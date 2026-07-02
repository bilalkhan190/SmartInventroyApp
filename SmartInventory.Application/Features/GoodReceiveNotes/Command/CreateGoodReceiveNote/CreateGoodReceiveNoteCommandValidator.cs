using FluentValidation;

namespace SmartInventory.Application.Features.GoodReceiveNotes.Command.CreateGoodReceiveNote;

public sealed class CreateGoodReceiveNoteCommandValidator : AbstractValidator<CreateGoodReceiveNoteCommand>
{
    public CreateGoodReceiveNoteCommandValidator()
    {
        RuleFor(command => command.PurchaseOrderId)
            .NotEmpty().WithMessage("Purchase order is required.");

        RuleFor(command => command.Items)
            .NotEmpty().WithMessage("At least one line item is required.");

        RuleForEach(command => command.Items).ChildRules(item =>
        {
            item.RuleFor(line => line.ProductId)
                .NotEmpty().WithMessage("Product is required.");

            item.RuleFor(line => line.ReceivedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Received quantity cannot be negative.");

            item.RuleFor(line => line.AcceptedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Accepted quantity cannot be negative.");

            item.RuleFor(line => line.AcceptedQuantity)
                .LessThanOrEqualTo(line => line.ReceivedQuantity)
                .WithMessage("Accepted quantity cannot exceed received quantity.");

            item.RuleFor(line => line.BatchNo)
                .MaximumLength(100).WithMessage("Batch number must not exceed 100 characters.")
                .When(line => !string.IsNullOrWhiteSpace(line.BatchNo));

            item.RuleFor(line => line.Remarks)
                .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.")
                .When(line => !string.IsNullOrWhiteSpace(line.Remarks));
        });

        RuleFor(command => command.Items)
            .Must(items => items.Select(item => item.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate products are not allowed in the same GRN.")
            .When(command => command.Items.Count > 0);
    }
}
