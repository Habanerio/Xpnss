using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Application.Merchants.Queries.GetMerchantsByIds;
using Habanerio.Xpnss.Domain.Merchants.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Application.Merchants.Queries.GetMerchant;

public class GetMerchantsByIdsHandler(IMerchantsRepository repository) : IRequestHandler<GetMerchantsByIdsQuery, Result<IEnumerable<MerchantDto>>>
{
    private readonly IMerchantsRepository _repository = repository ??
                                                        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<MerchantDto>>> Handle(GetMerchantsByIdsQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var merchantResult = await _repository.GetAsync(request.UserId, request.MerchantIds.Select(ObjectId.Parse), cancellationToken);

        if (merchantResult.IsFailed)
            return Result.Fail(merchantResult.Errors);

        if (merchantResult.ValueOrDefault is null)
            return Result.Ok(Enumerable.Empty<MerchantDto>());

        var dtos = Mapper.Map(merchantResult.Value);

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetMerchantsByIdsQuery>
    {
        public Validator()
        {
            RuleFor(x => x.MerchantIds).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}