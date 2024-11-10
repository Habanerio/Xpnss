using MediatR;

namespace Habanerio.Xpnss.Domain.Categories.Interfaces;

public interface ICategoriesQuery<out TResult> : IRequest<TResult> { }