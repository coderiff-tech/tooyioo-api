// using System.Security.Claims;
// using Microsoft.AspNetCore.Http;
// using Slicent.Application.Authorization;
// using Tooyioo.Profile.Features.RetrieveAll.Contracts;
//
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Features.RetrieveAll;
//
// public sealed class RetrieveAllProfileAuthorizer
//     : IRouteAuthorizer<RetrieveAllProfileRoute>
// {
//     public Task<bool> Authorize(
//         ClaimsPrincipal claimsPrincipal, 
//         RetrieveAllProfileRoute route, 
//         HttpContext http, 
//         CancellationToken ct)
//     {
//         return Task.FromResult(true);
//     }
// }