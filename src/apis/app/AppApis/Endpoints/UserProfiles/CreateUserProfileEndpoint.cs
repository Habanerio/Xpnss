using System.Net;
using Carter;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.UserProfiles.Application.Commands;
using Habanerio.Xpnss.UserProfiles.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.UserProfiles;

public sealed class CreateUserProfileEndpoint : BaseEndpoint
{
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/users/create",
                    async (
                        [FromBody] CreateUserProfileCommand command,
                        [FromServices] IUserProfilesService service,
                        CancellationToken cancellationToken) =>
                    {
                        return await HandleAsync(command, service, cancellationToken);
                    })
                .Produces<UserProfileDto>((int)HttpStatusCode.OK)
                .Produces<string>((int)HttpStatusCode.BadRequest)
                .WithDisplayName("Create User Profile")
                .WithName("CreateUserProfile")
                .WithTags("UserProfiles")
                .WithOpenApi();
        }

        public static async Task<IResult> HandleAsync(
            CreateUserProfileCommand command,
            IUserProfilesService service,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(service);

            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequestWithErrors(validationResult.Errors);

            var result = await service.CommandAsync(command, cancellationToken);

            if (result.IsFailed)
                return BadRequestWithErrors(result.Errors);

            return Results.Ok(result.Value);
        }

        public sealed class Validator : AbstractValidator<CreateUserProfileCommand>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }
    }
}