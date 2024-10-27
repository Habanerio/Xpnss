using FluentResults;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Transactions;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;

namespace Habanerio.Xpnss.Apis.App.AppApis.Managers;

public interface IAccountTransactionManager
{
    Task<Result<TransactionDto>> AddTransactionAsync(CreateTransactionEndpoint.CreateTransactionRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Adds a Transaction to an Account and updates the Account.
/// </summary>
public class AccountTransactionManager : IAccountTransactionManager
{
    private readonly IAccountsService _accountsService;
    private readonly ITransactionsService _transactionsService;

    public AccountTransactionManager(IAccountsService accountsService, ITransactionsService transactionsService)
    {
        _accountsService = accountsService ??
                           throw new ArgumentNullException(nameof(accountsService));
        _transactionsService = transactionsService ??
                               throw new ArgumentNullException(nameof(transactionsService));
    }

    /// <summary>
    /// Adds a Transaction and updates the Account's Balance
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<TransactionDto>> AddTransactionAsync(CreateTransactionEndpoint.CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        AccountDto? account = null;

        if (!string.IsNullOrWhiteSpace(request.AccountId))
        {
            var accountQuery = new GetAccount.Query(request.UserId, request.AccountId);
            var accountResult = await _accountsService.ExecuteAsync(accountQuery, cancellationToken);

            if (accountResult.IsFailed)
                return Result.Fail(accountResult.Errors);

            account = accountResult.ValueOrDefault;

            if (account == null)
                return Result.Fail($"No account found for '{request.AccountId}");
        }

        var transactionCommand = new CreateTransaction.Command(
            request.UserId,
            request.AccountId,
            request.Items.Select(i => new TransactionItemDto
            {
                Amount = i.Amount,
                CategoryId = i.CategoryId,
                Description = i.Description
            }),
            request.TransactionDate,
            request.TransactionType,
            request.Description,
            request.TransactionMerchant is not null
                ? new MerchantDto
                {
                    Id = request.TransactionMerchant.Id,
                    Name = request.TransactionMerchant.Name,
                    Location = request.TransactionMerchant.Location
                }
                : null);

        var transactionResult = await _transactionsService.ExecuteAsync(transactionCommand, cancellationToken);

        if (transactionResult.IsFailed)
            return Result.Fail(transactionResult.Errors);

        var transactionDto = transactionResult.Value;

        if (transactionDto is null)
            return Result.Fail("Transaction was not created");

        if (account != null)
        {
            var isCredit = TransactionTypes.IsCredit(request.TransactionType);

            var addTransactionAmountCommand = new AddTransactionAmount.Command(
                request.UserId,
                request.AccountId,
                transactionResult.Value.Amount,
                IsCredit: isCredit,
                request.TransactionDate);

            var addTransactionAmountResult = await _accountsService.ExecuteAsync(addTransactionAmountCommand, cancellationToken);

            if (addTransactionAmountResult.IsFailed)
                return Result.Fail(addTransactionAmountResult.Errors);
        }

        return Result.Ok(transactionDto);
    }
}