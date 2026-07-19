// using Microsoft.AspNetCore.Http;
// using Slicent.Application.Commands;
// using Tooyioo.Profile.Domain;
// using Tooyioo.Profile.Features.SetPhoneNumber.Contracts;
//
// // ReSharper disable UnusedType.Global
//
// namespace Tooyioo.Profile.Features.SetPhoneNumber;
//
// public static class SetProfilePhoneNumberMappers
// {
//     public sealed class CommandMapper
//         : ICommandMapper<SetProfilePhoneNumberRoute, SetProfilePhoneNumberRequest, HttpContext, 
//             SetProfilePhoneNumberCommand>
//     {
//         public SetProfilePhoneNumberCommand Map(
//             SetProfilePhoneNumberRoute urlParams,
//             SetProfilePhoneNumberRequest request, 
//             HttpContext context)
//             => new(new ProfileId(urlParams.ProfileId.ToString()), request.PhoneNumber?.Trim());
//     }
//
//     public sealed class CommandHttpResponseMapper
//         : ICommandHttpResponseMapper<SetProfilePhoneNumberCommandResult, HttpContext, SetProfilePhoneNumberResponse>
//     {
//         public IResult Map(
//             SetProfilePhoneNumberCommandResult commandResult,
//             HttpContext context,
//             CommandHttpResponseGenerator<SetProfilePhoneNumberResponse> commandHttpResponseGenerator)
//             => commandResult.Match<IResult>(
//                 ok => commandHttpResponseGenerator.Ok(new SetProfilePhoneNumberResponse { Id = ok.Id }),
//                 _ => commandHttpResponseGenerator.Conflict(
//                     context,
//                     "profile_concurrency_conflict",
//                     "Could not set the profile's phone number because of a concurrency conflict. Please retry.")
//                 );
//     }
// }
