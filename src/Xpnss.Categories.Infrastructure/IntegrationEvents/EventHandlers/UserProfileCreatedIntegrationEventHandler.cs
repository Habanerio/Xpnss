using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.UserProfiles;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Categories.Infrastructure.IntegrationEvents.EventHandlers;

public class UserProfileCreatedIntegrationEventHandler(
    ICategoriesRepository repository,
    ILogger<UserProfileCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<UserProfileCreatedIntegrationEvent>
{
    private readonly ICategoriesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<UserProfileCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task Handle(
        UserProfileCreatedIntegrationEvent @event,
        CancellationToken cancellationToken)
    {
        var userId = @event.UserId;

        var doesUserHaveAccountsResult = await repository.ListAsync(userId, cancellationToken);

        if (doesUserHaveAccountsResult.Value.Any())
            return;

        var categories = new Dictionary<Category, List<Category>>()
        {
            {
                Category.New(new UserId(userId), new CategoryName("Income"), "Income Categories", CategoryId.Empty, 1),
                new[]
                {
                    Category.New(new UserId(userId), new CategoryName("Salary"), "Salary Income", CategoryId.Empty, 1),
                    Category.New(new UserId(userId), new CategoryName("Bonus"), "Bonus Income", CategoryId.Empty, 2),
                    Category.New(new UserId(userId), new CategoryName("Interest Income"), "Interest Income", CategoryId.Empty, 3),
                    Category.New(new UserId(userId), new CategoryName("Misc Income"), "Misc Income", CategoryId.Empty, 4)
                }.ToList()
            },
            {
                Category.New(new UserId(userId), new CategoryName("Credit Cards / Loans"), "Credit Card or Loan Payments", CategoryId.Empty, 2),
                new[]
                {
                    Category.New(new UserId(userId), new CategoryName("Payments"), "Credit Card Payments", CategoryId.Empty, 1)
                }.ToList()
            },
            {
                Category.New(new UserId(userId), new CategoryName("Home"), "Home Expenses", CategoryId.Empty, 3),
                new[]
                {
                    Category.New(new UserId(userId), new CategoryName("Rent/Mortgage"), "Rent/Mortgage Payments", CategoryId.Empty, 1),
                    Category.New(new UserId(userId), new CategoryName("Home Insurance"), "Home Insurance Payments", CategoryId.Empty, 2),
                    Category.New(new UserId(userId), new CategoryName("Groceries"), "Groceries Expenses", CategoryId.Empty, 3),
                    Category.New(new UserId(userId), new CategoryName("Streaming Service"), "Streaming Services Payments", CategoryId.Empty, 4),
                    Category.New(new UserId(userId), new CategoryName("Internet"), "Internet Payments", CategoryId.Empty, 5),
                    Category.New(new UserId(userId), new CategoryName("Phone"), "Phone Payments", CategoryId.Empty, 6),
                    Category.New(new UserId(userId), new CategoryName("Utilities"), "Utility Payments", CategoryId.Empty, 7),
                    Category.New(new UserId(userId), new CategoryName("Misc"), "Misc Expenses", CategoryId.Empty, 8)
                }.ToList()
            },
            {
                Category.New(new UserId(userId), new CategoryName("Personal"), "Personal Expenses", CategoryId.Empty, 5),
                new[]
                {
                    Category.New(new UserId(userId), new CategoryName("Dining Out"), "Dining Out Expenses", CategoryId.Empty, 1),
                    Category.New(new UserId(userId), new CategoryName("Entertainment"), "Entertainment Expenses", CategoryId.Empty, 2),
                    Category.New(new UserId(userId), new CategoryName("Clothing"), "Clothing Expenses", CategoryId.Empty, 3),
                    Category.New(new UserId(userId), new CategoryName("Health/Wellness"), "Health Expenses", CategoryId.Empty, 4),
                    Category.New(new UserId(userId), new CategoryName("Vacation"), "Vacation Expenses", CategoryId.Empty, 5),
                    Category.New(new UserId(userId), new CategoryName("Donations / Charity"), "Donations or Charity Expenses", CategoryId.Empty, 6),
                    Category.New(new UserId(userId), new CategoryName("Misc"), "Misc Expenses", CategoryId.Empty, 7)
                }.ToList()
            }
        };

        foreach (var kvp in categories)
        {
            try
            {
                var category = kvp.Key;

                var rslt = await _repository.AddAsync(category, cancellationToken);

                if (rslt.IsSuccess)
                {
                    var newCategory = rslt.Value;

                    _logger.LogInformation("{EventId}: Category '{CategoryName}' was created for User '{UserId}'",
                        @event.Id, newCategory.Name, userId);

                    if (kvp.Value.Any())
                    {
                        var subCategoryOrder = 1;
                        foreach (var subCategory in kvp.Value)
                        {
                            newCategory.AddSubCategory(subCategory.Name, subCategory.Description, subCategoryOrder);

                            subCategoryOrder++;

                            var subCategoryResult = await _repository.UpdateAsync(userId, newCategory, cancellationToken);

                            if (subCategoryResult.IsSuccess)
                            {
                                _logger.LogInformation("{EventId}: Category '{CategoryName}/{SubCategoryName}' was created for User '{UserId}'",
                                    @event.Id, newCategory.Name, subCategory.Name, userId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "{EventId}: An error occurred while trying to create Category '{Name}' for User '{UserId}'",
                    @event.Id, kvp.Key.Name, userId);

                throw;
            }
        }
    }
}