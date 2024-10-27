using System.Net;
using Carter;
using FluentResults;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;
using Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using Habanerio.Xpnss.Modules.Categories.CQRS.Commands;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Setup;

public class SetupEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/setup/{userId}",
                    async (
                        [FromRoute] string userId,
                        [FromServices] IAccountsService accountsService,
                        [FromServices] ICategoriesService categoriesService,
                        [FromServices] ITransactionsService transactionsService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, accountsService, categoriesService, transactionsService, cancellationToken);
                    }
                )
                .Produces((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Setup")
                .WithName("Setup")
                .WithTags("Setup")
                .WithOpenApi();
        }

        private async Task<IResult> HandleAsync(
            string userId,
            IAccountsService accountsService,
            ICategoriesService categoriesService,
            ITransactionsService transactionsService,
            CancellationToken cancellationToken)
        {
            var accountsResult = await AddAccountsAsync(userId, accountsService);

            if (accountsResult.IsFailed)
                return Results.BadRequest(accountsResult.Errors[0].Message);

            var categoriesResult = await AddCategoriesAsync(userId, categoriesService);

            if (categoriesResult.IsFailed)
                return Results.BadRequest(categoriesResult.Errors[0].Message);

            return Results.Ok();
        }

        private async Task<Result> AddAccountsAsync(string userId, IAccountsService accountsService)
        {
            var existingAccountsQuery = new GetAccounts.Query(userId);

            var existingAccountsResult = await accountsService.ExecuteAsync(existingAccountsQuery);

            if (existingAccountsResult.Value.Any())
                return Result.Ok();

            var accountDtos = new List<AccountDto>
            {
                new CashAccountDto
                {
                    UserId = userId,
                    Name = "Cash",
                    Description = "Primary Cash Account",
                    Balance = 0,
                    DisplayColor = "#ff00ff"
                },
                new CheckingAccountDto
                {
                    UserId = userId,
                    Name = "Checking",
                    Description = "Primary Checking Account",
                    Balance = 0,
                    OverDraftAmount = 500m,
                    DisplayColor = "#ff0000"
                },
                new SavingsAccountDto
                {
                    UserId = userId,
                    Name = "Savings",
                    Description = "Primary Savings Account",
                    Balance = 0,
                    InterestRate = 2.00m,
                    DisplayColor = "#00ff00"
                },
                new CreditCardAccountDto
                {
                    UserId = userId,
                    Name = "Credit Card",
                    Description = "Primary Credit Card Account",
                    Balance = 0,
                    CreditLimit = 5000m,
                    InterestRate = 19.99m,
                    DisplayColor = "#0000ff"
                }
            };

            foreach (var accountDto in accountDtos)
            {
                CreateAccount.Command command;

                switch (accountDto.AccountType)
                {
                    case "Cash":
                        var cashAccount = accountDto as CashAccountDto;
                        command = new CreateAccount.Command(cashAccount.UserId,
                            cashAccount.AccountType,
                            cashAccount.Name,
                            cashAccount.Description,
                            cashAccount.Balance,
                            0,
                            0,
                            0,
                            cashAccount.DisplayColor);

                        var cashResult = await accountsService.ExecuteAsync(command);

                        if (cashResult.IsFailed)
                            return Result.Fail(cashResult.Errors[0].Message);
                        break;
                    case "Checking":
                        var checkingAccount = accountDto as CheckingAccountDto;
                        command = new CreateAccount.Command(checkingAccount.UserId,
                            checkingAccount.AccountType,
                            checkingAccount.Name,
                            checkingAccount.Description,
                            checkingAccount.Balance,
                            0,
                            0,
                            checkingAccount.OverDraftAmount,
                            checkingAccount.DisplayColor);

                        var checkingResult = await accountsService.ExecuteAsync(command);

                        if (checkingResult.IsFailed)
                            return Result.Fail(checkingResult.Errors[0].Message);
                        break;

                    case "Savings":
                        var savingsAccount = accountDto as SavingsAccountDto;
                        command = new CreateAccount.Command(savingsAccount.UserId,
                            savingsAccount.AccountType,
                            savingsAccount.Name,
                            savingsAccount.Description,
                            savingsAccount.Balance, 0,
                            savingsAccount.InterestRate, 0,
                            savingsAccount.DisplayColor);

                        var savingsResult = await accountsService.ExecuteAsync(command);

                        if (savingsResult.IsFailed)
                            return Result.Fail(savingsResult.Errors[0].Message);
                        break;
                    case "CreditCard":
                        var creditCardAccount = accountDto as CreditCardAccountDto;
                        command = new CreateAccount.Command(creditCardAccount.UserId,
                            creditCardAccount.AccountType,
                            creditCardAccount.Name,
                            creditCardAccount.Description,
                            creditCardAccount.Balance,
                            creditCardAccount.CreditLimit,
                            creditCardAccount.InterestRate,
                            0,
                            creditCardAccount.DisplayColor);

                        var creditCardResult = await accountsService.ExecuteAsync(command);

                        if (creditCardResult.IsFailed)
                            return Result.Fail(creditCardResult.Errors[0].Message);
                        break;
                    case "LineOfCredit":
                        var lineOfCreditAccount = accountDto as LineOfCreditAccountDto;
                        command = new CreateAccount.Command(lineOfCreditAccount.UserId,
                            lineOfCreditAccount.AccountType,
                            lineOfCreditAccount.Name,
                            lineOfCreditAccount.Description,
                            lineOfCreditAccount.Balance,
                            lineOfCreditAccount.CreditLimit,
                            lineOfCreditAccount.InterestRate,
                            0,
                            lineOfCreditAccount.DisplayColor);

                        var lineOfCreditResult = await accountsService.ExecuteAsync(command);

                        if (lineOfCreditResult.IsFailed)
                            return Result.Fail(lineOfCreditResult.Errors[0].Message);
                        break;
                    default:
                        return Result.Fail("Invalid Account Type");
                }
            }

            return Result.Ok();
        }

        private async Task<Result> AddCategoriesAsync(string userId, ICategoriesService categoriesService)
        {
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto
                {
                    UserId = userId,
                    Name = "Home",
                    Description = "Home Expenses",
                    SubCategories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Name = "Rent/Mortgage",
                            Description = "Rent or Mortgage Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Home Insurance",
                            Description = "Home Insurance Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Streaming Service",
                            Description = "Streaming Services Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Internet",
                            Description = "Internet Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Utilities",
                            Description = "Utility Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Misc",
                            Description = "Misc Expenses",
                        },
                    }
                },
                new CategoryDto
                {
                    UserId = userId,
                    Name = "Auto",
                    Description = "Auto Expenses",
                    SubCategories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Name = "Car Payment",
                            Description = "Car Payment",
                        },
                        new CategoryDto
                        {
                            Name = "Car Insurance",
                            Description = "Car Insurance",
                        },
                        new CategoryDto
                        {
                            Name = "Gas",
                            Description = "Gas Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Maintenance",
                            Description = "Car Maintenance",
                        },
                        new CategoryDto
                        {
                            Name = "Misc",
                            Description = "Misc Expenses",
                        },
                    }
                },
                new CategoryDto
                {
                    UserId = userId,
                    Name="Personal",
                    Description="Personal Expenses",
                    SubCategories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Name = "Groceries",
                            Description = "Grocery Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Dining Out",
                            Description = "Dining Out Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Entertainment",
                            Description = "Entertainment Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Clothing",
                            Description = "Clothing Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Vacation",
                            Description = "Vacation Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Misc",
                            Description = "Misc Expenses",
                        },
                    }
                }
            };

            foreach (var categoryDto in categoryDtos)
            {
                var command = new CreateCategory.Command(
                    categoryDto.UserId,
                    categoryDto.Name,
                    categoryDto.Description);

                var result = await categoriesService.ExecuteAsync(command);

                if (result.IsFailed)
                    return Result.Fail(result.Errors[0].Message);

                if (categoryDto.SubCategories.Any())
                {
                    var subCategoryCommand = new AddSubCategories.Command(userId,
                        result.Value.Id,
                        categoryDto.SubCategories);

                    var subCategoryResult = await categoriesService.ExecuteAsync(subCategoryCommand);
                }
            }

            return Result.Ok();
        }
    }
}