using MediatR;

namespace Habanerio.Xpnss.Categories.Domain.Interfaces;

public interface ICategoriesCommand<out TResult> : IRequest<TResult> { }

public interface ICategoriesCommand : IRequest { }