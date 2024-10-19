using MediatR;

namespace Habanerio.Xpnss.Modules.Categories.Interfaces;

public interface ICategoriesCommand<out TResult> : IRequest<TResult> { }

public interface ICategoriesCommand : IRequest { }