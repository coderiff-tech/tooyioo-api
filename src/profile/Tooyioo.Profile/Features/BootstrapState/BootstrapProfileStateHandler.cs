// using Eventuous;
// using Funzo;
// using Slicent.Application.Commands;
// using Slicent.EventStore;
// using Tooyioo.Profile.Contracts;
// using Tooyioo.Profile.Domain;
// using Tooyioo.Profile.Domain.UniqueExternalIdentity;
// // ReSharper disable ConvertToPrimaryConstructor
// // ReSharper disable UnusedType.Global
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Features.BootstrapState;
//
// public sealed class BootstrapProfileStateHandler
//     : ICommandHandler<BootstrapProfileStateCommand, BootstrapProfileStateCommandResult>
// {
//     private readonly IEventReader _eventReader;
//     private readonly IEventWriter _eventWriter;
//
//     public BootstrapProfileStateHandler(
//         IEventReader eventReader,
//         IEventWriter eventWriter)
//     {
//         _eventReader = eventReader;
//         _eventWriter = eventWriter;
//     }
//
//     public async Task<BootstrapProfileStateCommandResult> Handle(
//         BootstrapProfileStateCommand command,
//         CancellationToken cancellationToken = default)
//     {
//         var profileId = command.ProfileId;
//         var externalId = command.ExternalId;
//         var externalIdentityProvider = command.ExternalIdentityProvider;
//
//         var uniqueExternalIdentity =
//             await _eventReader.LoadStateOrNew<UniqueExternalIdentityState, UniqueExternalIdentityId>(
//                 externalId,
//                 cancellationToken);
//
//         if (uniqueExternalIdentity.State.ProfileId is not null)
//         {
//             return new BootstrapProfileStateCommandOkResult(
//                 uniqueExternalIdentity.State.ProfileId,
//                 externalId,
//                 externalIdentityProvider);
//         }
//
//         var profile =
//             await _eventReader.LoadStateOrNew<ProfileState, ProfileId>(
//                 profileId,
//                 cancellationToken);
//
//         var profileEvents = GetProfileEvents(command);
//         var uniqueExternalIdentityEvents =
//             new object[] { new UniqueExternalIdDomainEvents.V1.Claimed(profileId) };
//
//         try
//         {
//             await _eventWriter.StoreStateChanges(
//                 new[]
//                 {
//                     profile.ToStateStreamChanges(profileEvents),
//                     uniqueExternalIdentity.ToStateStreamChanges(uniqueExternalIdentityEvents)
//                 },
//                 cancellationToken);
//
//             return new BootstrapProfileStateCommandOkResult(
//                 profileId,
//                 externalId,
//                 externalIdentityProvider);
//         }
//         catch (OptimisticConcurrencyException)
//         {
//             return new BootstrapProfileStateConcurrencyErrorResult();
//         }
//     }
//
//     private static object[] GetProfileEvents(BootstrapProfileStateCommand command)
//     {
//         var events = new List<object>
//         {
//             new ProfileDomainEvents.V1.Created(command.Name, command.LastName, command.Email)
//         };
//
//         if (!string.IsNullOrWhiteSpace(command.ExternalId))
//         {
//             events.Add(
//                 new ProfileDomainEvents.V1.ExternalIdAssociated(
//                     command.ExternalId,
//                     command.ExternalIdentityProvider));
//         }
//
//         if (command.IsEmailConfirmed)
//         {
//             events.Add(new ProfileDomainEvents.V1.EmailVerified());
//         }
//
//         return events.ToArray();
//     }
// }
//
// public sealed record BootstrapProfileStateCommand(
//     ProfileId ProfileId,
//     string Name,
//     string LastName,
//     string Email,
//     bool IsEmailConfirmed,
//     string ExternalId,
//     string ExternalIdentityProvider)
//     : ICommand<BootstrapProfileStateCommandResult>;
//
// [Result<BootstrapProfileStateCommandOkResult, BootstrapProfileStateConcurrencyErrorResult>]
// public partial class BootstrapProfileStateCommandResult;
//
// public sealed record BootstrapProfileStateCommandOkResult(
//     ProfileId ProfileId,
//     string ExternalId,
//     string ExternalIdProvider);
//
// public sealed record BootstrapProfileStateConcurrencyErrorResult;
