using MediatR;

namespace Habanerio.Xpnss.Categories.Domain.Interfaces;

public interface ICategoriesQuery<out TResult> : IRequest<TResult> { }