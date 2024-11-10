using MediatR;

namespace Habanerio.Xpnss.Domain.Categories.Interfaces;

public interface ICategoriesCommand<out TResult> : IRequest<TResult> { }

public interface ICategoriesCommand : IRequest { }