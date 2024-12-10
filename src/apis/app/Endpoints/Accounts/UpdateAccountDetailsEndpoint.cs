using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Commands.UpdateAccountDetails;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Accounts;

/// <summary>
/// Api endpoint for updating Account details.
/// </summary>
public class UpdateAccountDetailsEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/users/{userId}/accounts/{accountId}",
                    async (
                        [FromRoute] string userId,
                        [FromRoute] string accountId,
                        [FromBody] UpdateAccountDetailsApiRequest request,
                        [FromServices] IAccountsService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(userId, accountId, request, service, cancellationToken);
                    })
                .Produces<AccountDto>((int)HttpStatusCode.OK)
                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
                .Produces((int)HttpStatusCode.NotFound)
                .WithDisplayName("Update Account Details")
                .WithName("UpdateAccountDetails")
                .WithTags("Accounts")
                .WithOpenApi();
        }
    }

    public static async Task<IResult> HandleAsync(
        string userId,
        string accountId,
        UpdateAccountDetailsApiRequest request,
        IAccountsService service,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(service);

        if (string.IsNullOrWhiteSpace(userId))
            return BadRequestWithErrors("User Id is required");

        if (string.IsNullOrWhiteSpace(accountId))
            return BadRequestWithErrors("Account Id is required");

        var validationResult = await new Validator().ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequestWithErrors(validationResult.Errors);

        var command = new UpdateAccountDetailsCommand(userId, request);

        var result = await service.CommandAsync(command, cancellationToken);

        if (result.IsFailed)
            return BadRequestWithErrors(result.Errors);

        return Results.Ok(result.Value);
    }

    public sealed class Validator : AbstractValidator<UpdateAccountDetailsApiRequest>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}