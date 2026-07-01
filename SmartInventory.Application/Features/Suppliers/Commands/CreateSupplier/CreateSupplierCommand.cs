using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address) : IRequest<HandlerResult<SupplierDto>>;
