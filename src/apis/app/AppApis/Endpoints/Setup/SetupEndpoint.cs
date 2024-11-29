using System.Net;
using Carter;
using FluentResults;
using Habanerio.Xpnss.Accounts.Application.Commands.CreateAccount;
using Habanerio.Xpnss.Accounts.Application.Queries.GetAccounts;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Categories.Application.Commands;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.UserProfiles.Application.Commands;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Setup;

public class SetupEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/setup",
                    async (
                        [FromBody] CreateUserProfileRequest request,
                        [FromServices] IAccountsService accountsService,
                        [FromServices] ICategoriesService categoriesService,
                        [FromServices] IUserProfilesService userProfileService,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(request, accountsService, categoriesService, userProfileService, cancellationToken);
                    }
                )
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Setup")
                .WithName("Setup")
                .WithTags("Setup")
                .WithOpenApi();
        }

        private async Task<IResult> HandleAsync(
            CreateUserProfileRequest request,
            IAccountsService accountsService,
            ICategoriesService categoriesService,
            IUserProfilesService userProfileService,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequestWithErrors($"Email is required ({nameof(request.Email)})");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return BadRequestWithErrors($"First Name is required ({nameof(request.FirstName)})");

            var userProfileResult = await AddUserProfileAsync(request, userProfileService, cancellationToken);

            if (userProfileResult.IsFailed || userProfileResult.ValueOrDefault is null)
                return BadRequestWithErrors(userProfileResult.Errors[0].Message ?? $"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            if (string.IsNullOrWhiteSpace(userProfileResult.Value.Id))
                return BadRequestWithErrors($"Could not create a UserProfile for {request.FirstName} ({request.Email})");

            var userProfileDto = userProfileResult.Value;
            var userId = userProfileDto.Id;

            var accountsResult = await AddAccountsAsync(userId, accountsService, cancellationToken);

            if (accountsResult.IsFailed)
                return BadRequestWithErrors(accountsResult.Errors[0].Message);

            var categoriesResult = await AddCategoriesAsync(userId, categoriesService, cancellationToken);

            if (categoriesResult.IsFailed)
                return BadRequestWithErrors(categoriesResult.Errors[0].Message);

            return Results.Ok(userProfileDto);
        }

        /// <summary>
        /// Creates Accounts for the new User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accountsService"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Result> AddAccountsAsync(string userId, IAccountsService accountsService, CancellationToken cancellationToken = default)
        {
            //TODO: Add to a IntegrationEvent
            var existingAccountsQuery = new GetAccountsQuery(userId);

            var existingAccountsResult = await accountsService.QueryAsync(existingAccountsQuery, cancellationToken);

            if (existingAccountsResult.Value.Any())
                return Result.Ok();

            var accountDtos = new List<AccountDto>
            {
                new()
                {
                    UserId = userId,
                    Name = $"Wallet",
                    AccountType = AccountTypes.Keys.Cash.ToString(),
                    Description = "Wallet Account",
                    Balance = 0,
                    DisplayColor = "#E3FCD9"
                },
                new()
                {
                    UserId = userId,
                    Name = $"Checking",
                    AccountType = AccountTypes.Keys.Checking.ToString(),
                    Description = "Checking Account",
                    Balance = 0,
                    DisplayColor = "#ea7d33"
                },
                new()
                {
                    UserId = userId,
                    Name = $"Savings",
                    AccountType = AccountTypes.Keys.Savings.ToString(),
                    Description = "Savings Account",
                    Balance = 0,
                    DisplayColor = "#eeb822"
                },
                new()
                {
                    UserId = userId,
                    Name = $"Credit Card 1",
                    AccountType = AccountTypes.Keys.CreditCard.ToString(),
                    Description = "Credit Card 1 Account",
                    Balance = 0,
                    DisplayColor = "#05aced"
                }
            };

            foreach (var accountDto in accountDtos)
            {
                var createAccountCommand = new CreateAccountCommand(
                    accountDto.UserId,
                    accountDto.AccountType,
                    accountDto.Name,
                    accountDto.Description,
                    DisplayColor: accountDto.DisplayColor);

                _ = await accountsService.CommandAsync(createAccountCommand, cancellationToken);
            }

            return Result.Ok();
        }

        /// <summary>
        /// Creates Categories for the new User
        /// </summary>
        /// <returns></returns>
        private static async Task<Result> AddCategoriesAsync(string userId, ICategoriesService categoriesService, CancellationToken cancellationToken = default)
        {
            //TODO: Add to a IntegrationEvent
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto()
                {
                    UserId = userId,
                    Name = "Income",
                    Description = "Income Categories",
                    SubCategories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Name = "Salary",
                            Description = "Salary Income",
                        },
                        new CategoryDto
                        {
                            Name = "Bonus",
                            Description = "Bonus Income",
                        },
                        new CategoryDto
                        {
                            Name = "Interest",
                            Description = "Interest Income",
                        },
                        new CategoryDto
                        {
                            Name = "Misc",
                            Description = "Misc Income",
                        },
                    }
                },
                new CategoryDto
                {
                    UserId = userId,
                    Name = "Credit Cards / Loans",
                    Description = "Credit Card or Loan Payments",
                    SubCategories = new List<CategoryDto>
                    {
                        new CategoryDto
                        {
                            Name = "Credit Card 1 Payment",
                            Description = "Credit Card 1 Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Credit Card 2 Payment",
                            Description = "Credit Card 2 Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Line of Credit Payment",
                            Description = "Line of Credit Payments",
                        },
                        new CategoryDto
                        {
                            Name = "Loan Payment",
                            Description = "Loan Payments",
                        },
                    }
                },
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
                            Name = "Groceries",
                            Description = "Property Taxes",
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
                            Name = "Phone",
                            Description = "Phone Payments",
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
                            Name = "Health/Wellness",
                            Description = "Health Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Vacation",
                            Description = "Vacation Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Donations / Charity",
                            Description = "Donations or Charity Expenses",
                        },
                        new CategoryDto
                        {
                            Name = "Misc",
                            Description = "Misc Expenses",
                        },
                    }
                }
            };

            var sortOrder = 1;

            foreach (var categoryDto in categoryDtos)
            {
                var command = new CreateCategoryCommand(
                    categoryDto.UserId,
                    categoryDto.Name,
                    null,
                    categoryDto.Description,
                    sortOrder);

                var result = await categoriesService.CommandAsync(command, cancellationToken);

                if (result.IsFailed)
                    return Result.Fail(result.Errors[0].Message);

                if (categoryDto.SubCategories.Any())
                {
                    var subSortOrder = 1;

                    foreach (var subCategory in categoryDto.SubCategories)
                    {
                        var subCategoryCommand = new CreateCategoryCommand(
                            userId,
                            subCategory.Name,
                            result.Value.Id,
                            subCategory.Description,
                            subSortOrder);

                        var subResult = await categoriesService.CommandAsync(subCategoryCommand, cancellationToken);

                        if (subResult.IsFailed)
                            return Result.Fail(subResult.Errors[0].Message);

                        subSortOrder++;
                    }
                }

                sortOrder++;
            }

            return Result.Ok();
        }

        /// <summary>
        /// Creates a new UserProfile
        /// </summary>
        /// <returns></returns>
        private static async Task<Result<UserProfileDto>> AddUserProfileAsync(CreateUserProfileRequest request, IUserProfilesService userProfilesService, CancellationToken cancellationToken = default)
        {
            //TODO: Add to a IntegrationEvent
            var createUserProfileCommand = new CreateUserProfileCommand(request.Email, request.FirstName, request.LastName, request.ExtUserId);

            var result = await userProfilesService.CommandAsync(createUserProfileCommand, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors[0].Message);

            return Result.Ok(result.Value);
        }
    }
}