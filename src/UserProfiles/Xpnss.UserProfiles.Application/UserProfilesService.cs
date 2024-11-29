using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.UserProfiles.Application;

public class UserProfilesService(IMediator mediator, ILogger<UserProfilesService> logger) : IUserProfilesService
{
    private readonly ILogger<UserProfilesService> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    //TODO: Make them return Result<TResult> instead of TResult
    // See below
    public async Task<TResult> CommandAsync<TResult>(IUserProfilesCommand<TResult> command, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _mediator.Send(command, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error executing command {Command}", command.GetType().Name);

            return default;
        }
    }

    public Task CommandAsync(IUserProfilesCommand command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }

    public Task<TResult> QueryAsync<TResult>(IUserProfilesQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(query, cancellationToken);
    }
}

//public class UserProfilesService(IMediator mediator, ILogger<UserProfilesService> logger) :
//    IUserProfilesService
//{
//    private readonly ILogger<UserProfilesService> _logger = logger ??
//          throw new ArgumentNullException(nameof(logger));

//    private readonly IMediator _mediator = mediator ??
//                                           throw new AbandonedMutexException(nameof(mediator));

//    public async Task<Result<TResult>> CommandAsync<TResult>(
//        IUserProfilesCommand<TResult> command,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            return await _mediator.Send(command, cancellationToken);
//        }
//        catch (Exception e)
//        {
//            logger.LogError(e, "Error executing command {Command}", command.GetType().Name);

//            return default;
//        }
//    }

//    public Task CommandAsync(IUserProfilesCommand command, CancellationToken cancellationToken = default)
//    {
//        return _mediator.Send(command, cancellationToken);
//    }

//    public Task<Result<TResult>> QueryAsync<TResult>(IUserProfilesQuery<TResult> query, CancellationToken cancellationToken = default)
//    {
//        return _mediator.Send(query, cancellationToken);
//    }
//}