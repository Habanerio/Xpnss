using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Merchants.Application.Mappers;
using Habanerio.Xpnss.Merchants.Domain;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Merchants.Application.Commands.CreateMerchant;

public class CreateMerchantHandler(IMerchantsRepository repository) : IRequestHandler<CreateMerchantCommand, Result<MerchantDto>>
{
    private readonly IMerchantsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<MerchantDto>> Handle(CreateMerchantCommand command, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var merchant = Merchant.New(new UserId(command.UserId), new MerchantName(command.Name), command.Location);

        var result = await _repository.AddAsync(merchant, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors[0].Message ?? "Could not save the Merchant");

        var merchantDto = Mapper.Map(merchant);

        if (merchantDto is null)
            return Result.Fail("Failed to map MerchantDocument to MerchantDto");

        return Result.Ok(merchantDto);
    }

    public class Validator : AbstractValidator<CreateMerchantCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}