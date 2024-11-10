using FluentResults;
using FluentValidation.Results;

namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints;

public abstract class BaseEndpoint
{
    public static IResult BadRequestWithErrors(string error)
    {
        return Results.BadRequest(new List<string> { error }.AsEnumerable());
    }

    /// <summary>
    /// Helper method for when the Api Endpoint is expected to return
    /// a BadRequest that has one or more FluentValidation errors.
    /// Just normalizes for consistency.
    /// </summary>
    /// <param name="errors">One or more FluentValidation errors</param>
    /// <returns>A BadRequest with the errors separated by a NewLine</returns>
    public static IResult BadRequestWithErrors(List<ValidationFailure> errors)
    {
        return Results.BadRequest(errors.Select(e => e.ErrorMessage));
        //return Results.BadRequest(string.Join(Environment.NewLine,
        //    errors.Select(e => e.ErrorMessage)));
    }

    /// <summary>
    /// Helper method for when the Api Endpoint is expected to return
    /// a BadRequest that has one or more FluentResults errors.
    /// Just normalizes for consistency.
    /// </summary>
    /// <param name="errors">One or more FluentResults errors</param>
    /// <returns>A BadRequest with the errors separated by a NewLine</returns>
    public static IResult BadRequestWithErrors(List<IError> errors)
    {
        return Results.BadRequest(errors.Select(e => e.Message));
        //return Results.BadRequest(string.Join(Environment.NewLine,
        //    errors.Select(e => e.Message)));
    }
}