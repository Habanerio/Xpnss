using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Merchants.Application.Mappers;
using Habanerio.Xpnss.Merchants.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Merchants.Application.Queries.GetMerchant;

public class GetMerchantHandler(IMerchantsRepository repository) : IRequestHandler<GetMerchantQuery, Result<MerchantDto>>
{
    private readonly IMerchantsRepository _repository = repository ??
                                                        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<MerchantDto>> Handle(GetMerchantQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var merchantResult = await _repository.GetAsync(request.UserId, request.MerchantId, cancellationToken);

        if (merchantResult.IsFailed)
            return Result.Fail(merchantResult.Errors);

        var dto = Mapper.Map(merchantResult.Value);

        if (dto is null)
            return Result.Fail("Failed to map MerchantDocument to MerchantDto");

        return Result.Ok(dto);
    }

    public class Validator : AbstractValidator<GetMerchantQuery>
    {
        public Validator()
        {
            RuleFor(x => x.MerchantId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}