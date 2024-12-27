//using System.Net;
//using Carter;
//using Habanerio.Xpnss.Categories.Domain.Interfaces;
//using Habanerio.Xpnss.Shared.DTOs.Categories;
//using Habanerio.Xpnss.Totals.Domain.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;

//public sealed class GetCategoriesTotalsByYearEndpoint : BaseEndpoint
//{
//    public sealed class Endpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapGet("/api/v1/users/{userId}/categories/totals/year/{year:int}",
//                    async (
//                        [FromRoute] string userId,
//                        [FromRoute] int year,
//                        [FromServices] ICategoriesService categoriesService,
//                        [FromServices] IMonthlyTotalsService monthlyTotalsService,
//                        CancellationToken cancellationToken = default) =>
//                    {
//                        return await HandleAsync(
//                            userId, 
//                            year,
//                            categoriesService, 
//                            monthlyTotalsService, 
//                            cancellationToken);
//                    })
//                .Produces<CategoryDto>((int)HttpStatusCode.OK)
//                .Produces<IEnumerable<string>>((int)HttpStatusCode.BadRequest)
//                .WithDisplayName("Get Categories Totals by Year")
//                .WithName("GetCategoriesTotalsByYear")
//                .WithTags("Categories")
//                .WithOpenApi();
//        }

//        public static async Task<IResult> HandleAsync(
//            string userId,
//            int year,
//            ICategoriesService categoriesService,
//            IMonthlyTotalsService monthlyTotalsService,
//            CancellationToken cancellationToken = default)
//        {
//            ArgumentNullException.ThrowIfNull(categoriesService);
//            ArgumentNullException.ThrowIfNull(monthlyTotalsService);

//            if (string.IsNullOrWhiteSpace(userId))
//                return BadRequestWithErrors($"{nameof(userId)} cannot be null or empty");


//        }
//    }
//}
