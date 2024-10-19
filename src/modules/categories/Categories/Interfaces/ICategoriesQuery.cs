using MediatR;

namespace Habanerio.Xpnss.Modules.Categories.Interfaces;

public interface ICategoriesQuery<out TResult> : IRequest<TResult> { }