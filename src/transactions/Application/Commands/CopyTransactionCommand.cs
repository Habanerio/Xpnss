using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CopyTransactionCommand(
    string UserId,
    string TransactionId,
    CopyTransactionRequest[] Requests) :
    ITransactionsCommand<IEnumerable<Result<TransactionDto>>>;

public sealed class CopyTransactionCommandHandler(ITransactionsRepository repository, IMediator mediator) :
    IRequestHandler<CopyTransactionCommand, IEnumerable<Result<TransactionDto>>>
{
    public async Task<IEnumerable<Result<TransactionDto>>> Handle(
        CopyTransactionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(mediator);

        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return new List<Result<TransactionDto>>
                { Result.Fail(validationResult.Errors[0].ErrorMessage) };

        var actualTransactionResult = await repository.GetAsync(
            command.UserId,
            command.TransactionId,
            cancellationToken);

        if (actualTransactionResult.IsFailed || actualTransactionResult.ValueOrDefault is null)
            return new List<Result<TransactionDto>> {
                Result.Fail(actualTransactionResult.Errors?[0].Message ?? "Transaction not found")
        };

        var actualTransaction = actualTransactionResult.ValueOrDefault;

        var tasks = new List<Task<Result<TransactionDto>>>();

        var newTransactionDates = command.Requests
            .Select(r => r.TransactionDate).ToList();

        foreach (var newTransactionDate in newTransactionDates)
        {
            var refTransactionId = actualTransaction.Id.Value;

            if (actualTransaction is CreditTransaction creditTransaction)
            {
                if (actualTransaction.TransactionType.Equals(TransactionEnums.TransactionKeys.DEPOSIT))
                {
                    var createDepositRequest = new CreateDepositTransactionApiRequest(
                        command.UserId,
                        creditTransaction.AccountId.Value,
                        creditTransaction.TotalAmount.Value,
                        creditTransaction.Description,
                        new PayerPayeeApiRequest() { Id = creditTransaction.PayerPayeeId.Value },
                        newTransactionDate,
                        creditTransaction.Tags?.ToList() ?? [],
                        creditTransaction.ExtTransactionId,
                        refTransactionId);

                    var newCommand = new CreateTransactionCommand(command.UserId, createDepositRequest);

                    tasks.Add(mediator.Send(newCommand, cancellationToken));
                }
            }
            else if (actualTransaction is DebitTransaction debitTransaction)
            {
                if (actualTransaction is PurchaseTransaction purchaseTransaction)
                {
                    var createPurchaseRequest = new CreatePurchaseTransactionApiRequest(
                        command.UserId,
                        accountId: purchaseTransaction.AccountId,
                        new PayerPayeeApiRequest() { Id = purchaseTransaction.PayerPayeeId.Value },
                        purchaseTransaction.Description,
                        newTransactionDate,
                        purchaseTransaction.Items.Select(i => new TransactionApiRequestItem
                        {
                            Amount = i.Amount,
                            CategoryId = i.CategoryId,
                            SubCategoryId = i.SubCategoryId,
                            Description = i.Description,
                        }).ToList(),
                        purchaseTransaction.Tags?.ToList() ?? [],
                        purchaseTransaction.ExtTransactionId,
                        refTransactionId);

                    var newCommand = new CreateTransactionCommand(command.UserId, createPurchaseRequest);

                    tasks.Add(mediator.Send(newCommand, cancellationToken));
                }

                else if (actualTransaction.TransactionType.Equals(TransactionEnums.TransactionKeys.WITHDRAWAL))
                {
                    var createWithdrawalRequest = new CreateWithdrawalTransactionApiRequest(
                        command.UserId,
                        accountId: debitTransaction.AccountId,
                        amount: debitTransaction.TotalAmount,
                        cashAccountId: debitTransaction.PayerPayeeId,
                        debitTransaction.Description,
                        newTransactionDate,
                        debitTransaction.Tags?.ToList() ?? [],
                        debitTransaction.ExtTransactionId,
                        refTransactionId);

                    var newCommand = new CreateTransactionCommand(command.UserId, createWithdrawalRequest);

                    tasks.Add(mediator.Send(newCommand, cancellationToken));
                }

                else if (actualTransaction.TransactionType.Equals(TransactionEnums.TransactionKeys.PURCHASE))
                {
                    throw new NotImplementedException("Copying a Debit Transaction of type Purchase is not yet supported");
                }

                else
                {
                    return new List<Result<TransactionDto>>{ Result.Fail($"{nameof(CopyTransactionCommandHandler)}: " +
                        $"Invalid Transaction Type. '{actualTransaction.GetType().Name}' " +
                        $"is not (yet) a support type")};
                }
            }
        }

        if (tasks.Any())
        {
            var newTransactionResults = await Task.WhenAll(tasks);

            return newTransactionResults.AsEnumerable();
        }

        return new List<Result<TransactionDto>>
            { Result.Fail("No transactions were copied") };
    }
    public class Validator : AbstractValidator<CopyTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Requests).NotEmpty();
        }
    }
}