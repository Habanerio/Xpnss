using FluentResults;

namespace Habanerio.Xpnss.UserProfiles.Domain.Interfaces;

public interface IUserProfilesService
{
    Task<TResult> CommandAsync<TResult>(
        IUserProfilesCommand<TResult> command,
        CancellationToken cancellationToken = default);

    Task CommandAsync(
        IUserProfilesCommand command,
        CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TResult>(
        IUserProfilesQuery<TResult> query,
        CancellationToken cancellationToken = default);
}

//TODO: Make them return Result<TResult> instead of TResult
//public interface IUserProfilesService
//{
//    Task<Result<TResult>> CommandAsync<TResult>(
//        IUserProfilesCommand<TResult> command,
//        CancellationToken cancellationToken = default);

//    Task CommandAsync(
//        IUserProfilesCommand command,
//        CancellationToken cancellationToken = default);

//    Task<Result<TResult>> QueryAsync<TResult>(
//        IUserProfilesQuery<TResult> query,
//        CancellationToken cancellationToken = default);
//}