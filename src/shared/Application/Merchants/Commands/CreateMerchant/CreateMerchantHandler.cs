using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Domain.Merchants;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Merchants.Commands.CreateMerchant;

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