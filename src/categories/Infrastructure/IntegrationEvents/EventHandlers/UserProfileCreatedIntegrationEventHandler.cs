using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Shared.IntegrationEvents.UserProfiles;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Categories.Infrastructure.IntegrationEvents.EventHandlers;

public class UserProfileCreatedIntegrationEventHandler(
    ICategoriesRepository repository,
    //IClientSessionHandle mongoSession,
    ILogger<UserProfileCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<UserProfileCreatedIntegrationEvent>
{
    private readonly ILogger<UserProfileCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    //TODO: Would like to use IClientSessionHandle,
    //      but get error: 'Standalone servers do not support transactions.'

    ////    private readonly IClientSessionHandle _mongoSession = mongoSession ??
    ////    throw new ArgumentNullException(nameof(mongoSession));

    private readonly ICategoriesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task Handle(
        UserProfileCreatedIntegrationEvent @event,
        CancellationToken cancellationToken)
    {
        var userId = @event.UserId;

        var doesUserHaveAccountsResult = await repository.ListAsync(userId, cancellationToken);

        if (doesUserHaveAccountsResult.Value.Any())
            return;

        var categories = new List<Category>();

        var hiddenCategory = Category.New(
            new UserId(userId),
            new CategoryName("Hidden By System"),
            CategoryGroupEnums.CategoryKeys.NA,
            "For transactions such as Transfers, where you don't want it to be calculated in the totals",
            99999);

        categories.Add(hiddenCategory);

        //var incomeCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Income"),
        //    CategoryGroupEnums.CategoryKeys.REVENUE,
        //    "Income Categories", 1);

        //incomeCategory.AddSubCategory(
        //    new CategoryName("Pay"),
        //    "Work Pay", 1);

        //incomeCategory.AddSubCategory(
        //    new CategoryName("Misc Income"),
        //    "Misc Income", 2);

        //categories.Add(incomeCategory);

        //var homeCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Expenses"),
        //    CategoryGroupEnums.CategoryKeys.EXPENSES,
        //    "",
        //    3);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Mortgage/Rent"),
        //    "Mortgage/Rent",
        //    1);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Home Insurance"),
        //    "Home Insurance Payments",
        //    2);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Groceries"),
        //    "Groceries Expenses",
        //    3);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Streaming Services"),
        //    "Streaming Services Payments",
        //    4);

        //homeCategory.AddSubCategory
        //    (new CategoryName("Internet"),
        //        "Internet Payments",
        //        5);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Phone"),
        //    "Phone Payments",
        //    6);

        //homeCategory.AddSubCategory
        //    (new CategoryName("Utilities"),
        //        "Utility Payments",
        //        7);

        //homeCategory.AddSubCategory(
        //    new CategoryName("Misc"),
        //    "Misc Expenses",
        //    8);

        //categories.Add(homeCategory);



        //var carCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Car"),
        //    CategoryGroupEnums.CategoryKeys.EXPENSES,
        //"Car",
        //    4);

        //carCategory.AddSubCategory(
        //    new CategoryName("Payments"),
        //    "Car Payment",
        //    1);

        //carCategory.AddSubCategory(
        //    new CategoryName("Insurance"),
        //    "Car Insurance",
        //    2);

        //carCategory.AddSubCategory(
        //    new CategoryName("Gas"),
        //    "Gas Expenses",
        //    3);

        //carCategory.AddSubCategory(
        //    new CategoryName("Maintenance"),
        //    "Car Maintenance Expenses",
        //    4);

        //carCategory.AddSubCategory(
        //    new CategoryName("Misc"),
        //    "Misc Expenses",
        //    6);

        //categories.Add(carCategory);

        //var personalCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Personal"),
        //    CategoryGroupEnums.CategoryKeys.EXPENSES,
        //    "Personal Expenses",
        //    5);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Dining Out"),
        //    "Dining Out Expenses",
        //    1);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Entertainment"),
        //    "Theatres, Concerts, ...",
        //    2);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Clothing"),
        //    "Clothing Expenses",
        //    3);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Health/Wellness"),
        //    "Health Expenses",
        //    4);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Vacation"),
        //    "Vacation Expenses",
        //    5);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Donations"),
        //    "Donations or Charity Expenses",
        //    6);

        //personalCategory.AddSubCategory(
        //    new CategoryName("Misc"),
        //    "Misc Expenses",
        //    7);

        //categories.Add(personalCategory);


        //var uncategorizedIncomeCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Uncategorized Income"),
        //    CategoryGroupEnums.CategoryKeys.REVENUE,
        //    "Uncategorized Expenses",
        //    6);

        //categories.Add(uncategorizedIncomeCategory);

        //var uncategorizedExpenseCategory = Category.New(
        //    new UserId(userId),
        //    new CategoryName("Uncategorized Expenses"),
        //    CategoryGroupEnums.CategoryKeys.EXPENSES,
        //    "Uncategorized Expenses",
        //    7);

        //categories.Add(uncategorizedExpenseCategory);



        foreach (var category in categories)
        {
            //using (_mongoSession)
            //{
            //      _mongoSession.StartTransaction();

            try
            {
                var rslt = await _repository.AddAsync(category, cancellationToken);

                if (rslt.IsSuccess)
                {
                    var newCategory = rslt.Value;

                    if (category.SubCategories.Any() && newCategory.SubCategories.Count == 0)
                    {
                        //await _mongoSession.AbortTransactionAsync(cancellationToken);

                        throw new InvalidOperationException(
                            $"SubCategories were not created for Category '{newCategory.Name}'");
                    }

                    //await _mongoSession.CommitTransactionAsync(cancellationToken);
                    _logger.LogInformation("{EventId}:" +
                                           " Category '{NewCategoryName}' was created for User '{UserId}'",
                                @event.Id, newCategory.Name, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{EventId}: " +
                    "An error occurred while trying to create Category '{CategoryName}' for User '{UserId}'",
                    @event.Id, category.Name, userId);

                throw;
            }
            //}

        }
    }
}