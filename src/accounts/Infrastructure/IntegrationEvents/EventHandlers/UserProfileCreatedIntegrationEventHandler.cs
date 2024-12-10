using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts;
using Habanerio.Xpnss.Accounts.Domain.Entities.Accounts.CashAccounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.IntegrationEvents.UserProfiles;
using Habanerio.Xpnss.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Infrastructure.IntegrationEvents.EventHandlers;

/// <summary>
/// Handles when the user's profile is created and creates the default accountBase(s) for them.
/// </summary>
public class UserProfileCreatedIntegrationEventHandler(
    IAccountsRepository repository,
    ILogger<UserProfileCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<UserProfileCreatedIntegrationEvent>
{
    private readonly IAccountsRepository _accountsRepository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<UserProfileCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task Handle(
        UserProfileCreatedIntegrationEvent @event,
        CancellationToken cancellationToken)
    {
        var userId = @event.UserId;

        var doesUserHaveAccountsResult = (await _accountsRepository.ListAsync(userId, cancellationToken));

        if (doesUserHaveAccountsResult.Value.Any())
            return;

        var accounts = new List<AbstractAccountBase>
        {
            CashAccount.New(
                new UserId(userId),
                new AccountName("Wallet"),
                "Wallet Account",
                "#E3FCD9")
        };

        foreach (var account in accounts)
        {
            try
            {
                await _accountsRepository.AddAsync(account, cancellationToken);

                _logger.LogInformation("{EventId}: Account '{Id}' was created for User '{UserId}'",
                    @event.Id, account.Id, userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{EventId}: An error occurred while trying to create Account " +
                    "'{Id}' for User '{UserId}'", @event.Id, account.Id, userId);

                throw;
            }
        }

    }
}