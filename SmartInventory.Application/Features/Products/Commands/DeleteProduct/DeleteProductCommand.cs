using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<HandlerResult<bool>>;
