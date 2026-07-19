// using System.Security.Claims;
// using Microsoft.AspNetCore.Http;
// using Slicent.Application.Authorization;
// using Tooyioo.Profile.Features.Retrieve.Contracts;
//
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Features.Retrieve;
//
// public sealed class RetrieveProfileAuthorizer
//     : IRouteAuthorizer<RetrieveProfileRoute>
// {
//     public Task<bool> Authorize(
//         ClaimsPrincipal claimsPrincipal, 
//         RetrieveProfileRoute route, 
//         HttpContext http, 
//         CancellationToken ct)
//     {
//         var subOption = claimsPrincipal.GetSubClaim();
//         if (!subOption.IsSome(out var sub))
//         {
//             return Task.FromResult(false);
//         }
//         var isSameProfile = route.ProfileId.ToString() == sub;
//         return Task.FromResult(isSameProfile);
//     }
// }