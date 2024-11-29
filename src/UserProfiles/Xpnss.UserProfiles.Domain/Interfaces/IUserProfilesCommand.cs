using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Domain.Interfaces;

public interface IUserProfilesCommand<out TResult> : IRequest<TResult> { }

public interface IUserProfilesCommand : IRequest { }