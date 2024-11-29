using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.UserProfiles;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Infrastructure.IntegrationEvents.EventHandlers;

public class UserProfileCreatedIntegrationEventHandler(
    IAccountsRepository accountsRepository,
    ILogger<TransactionCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<UserProfileCreatedIntegrationEvent>
{
    private readonly IAccountsRepository _accountsRepository = accountsRepository ??
        throw new ArgumentNullException(nameof(accountsRepository));

    private readonly ILogger<TransactionCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public Task Handle(
        UserProfileCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}