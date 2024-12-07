using MediatR;

namespace Habanerio.Xpnss.UserProfiles.Domain.Interfaces;

public interface IUserProfilesQuery<out TResult> : IRequest<TResult>;