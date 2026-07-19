// using Eventuous;
// using Funzo;
// using Slicent.Application.Commands;
// using Slicent.EventStore;
// using Tooyioo.Profile.Domain;
// using Tooyioo.Profile.Domain.UniqueExternalIdentity;
// // ReSharper disable ConvertToPrimaryConstructor
// // ReSharper disable UnusedType.Global
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Features.Bootstrap;
//
// public sealed class BootstrapProfileHandler
//     : ICommandHandler<BootstrapProfileCommand, BootstrapProfileCommandResult>
// {
//     private readonly IEventReader _eventReader;
//     private readonly IEventWriter _eventWriter;
//
//     public BootstrapProfileHandler(
//         IEventReader eventReader,
//         IEventWriter eventWriter)
//     {
//         _eventReader = eventReader;
//         _eventWriter = eventWriter;
//     }
//
//     public async Task<BootstrapProfileCommandResult> Handle(
//         BootstrapProfileCommand command, 
//         CancellationToken cancellationToken = default)
//     {
//         var profileId = command.ProfileId;
//         var externalId = command.ExternalId;
//         var externalIdentityProvider = command.ExternalIdentityProvider;
//         
//         var uniqueExternalIdentityAggregate =
//             await _eventReader.LoadAggregateOrNew<UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
//                 externalId, cancellationToken);
//
//         var claimUniqueExternalIdentityResult = uniqueExternalIdentityAggregate.Claim(profileId);
//         var hasUniqueExternalIdentityError = claimUniqueExternalIdentityResult.IsErr(out var externalIdentityAlreadyClaimedError);
//         if (hasUniqueExternalIdentityError)
//         {
//             // Idempotency guaranteed if external identity is already claimed
//             return new BootstrapProfileCommandOkResult(
//                 externalIdentityAlreadyClaimedError!.ExternalProfileClaimedBy, 
//                 externalId, 
//                 externalIdentityProvider);
//         }
//         
//         var profileAggregate =
//             await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
//                 profileId, cancellationToken);
//         
//         profileAggregate.Bootstrap(
//             command.Name, 
//             command.LastName, 
//             command.Email, 
//             command.IsEmailConfirmed, 
//             externalId, 
//             externalIdentityProvider);
//
//         try
//         {
//             _ = await _eventWriter.Store<
//                 ProfileAggregate, ProfileState, ProfileId,
//                 UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
//                 profileAggregate, uniqueExternalIdentityAggregate, cancellationToken);
//             
//             return new BootstrapProfileCommandOkResult(
//                 profileId,
//                 command.ExternalId,
//                 command.ExternalIdentityProvider);
//         }
//         catch (OptimisticConcurrencyException)
//         {
//             return new BootstrapProfileConcurrencyErrorResult();
//         }
//     }
// }
//
// public sealed record BootstrapProfileCommand(
//     ProfileId ProfileId,
//     string Name, 
//     string LastName, 
//     string Email, 
//     bool IsEmailConfirmed,
//     string ExternalId,
//     string ExternalIdentityProvider)
//     : ICommand<BootstrapProfileCommandResult>;
//
// [Result<BootstrapProfileCommandOkResult, BootstrapProfileConcurrencyErrorResult>]
// public partial class BootstrapProfileCommandResult;
//
// public sealed record BootstrapProfileCommandOkResult(ProfileId ProfileId, string ExternalId, string ExternalIdProvider);
//
// public sealed record BootstrapProfileConcurrencyErrorResult;