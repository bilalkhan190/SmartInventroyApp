using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Suppliers.Queries.GetSuppliersList;

public sealed record GetSuppliersListQuery : IRequest<HandlerResult<IReadOnlyList<SupplierDto>>>;
