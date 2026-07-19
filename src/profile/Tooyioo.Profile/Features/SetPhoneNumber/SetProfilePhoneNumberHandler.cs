// using Eventuous;
// using Funzo;
// using Slicent.Application.Commands;
// using Slicent.EventStore;
// using Tooyioo.Profile.Domain;
// // ReSharper disable ConvertToPrimaryConstructor
// // ReSharper disable UnusedType.Global
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Features.SetPhoneNumber;
//
// public sealed class SetProfilePhoneNumberHandler
//     : ICommandHandler<SetProfilePhoneNumberCommand, SetProfilePhoneNumberCommandResult>
// {
//     private readonly IEventReader _eventReader;
//     private readonly IEventWriter _eventWriter;
//
//     public SetProfilePhoneNumberHandler(
//         IEventReader eventReader,
//         IEventWriter eventWriter)
//     {
//         _eventReader = eventReader;
//         _eventWriter = eventWriter;
//     }
//     
//     public async Task<SetProfilePhoneNumberCommandResult> Handle(
//         SetProfilePhoneNumberCommand command, 
//         CancellationToken cancellationToken = default)
//     {
//         var profileId = command.ProfileId;
//         var profileAggregate =
//             await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
//                 profileId, cancellationToken);
//         profileAggregate.SetProfilePhoneNumber(command.PhoneNumber);
//
//         try
//         {
//             await _eventWriter.StoreAggregate<ProfileAggregate, ProfileState, ProfileId>(
//                 profileAggregate, cancellationToken: cancellationToken);
//             
//             return new SetProfilePhoneNumberCommandOkResult(profileId);
//         }
//         catch (OptimisticConcurrencyException)
//         {
//             return new SetProfilePhoneNumberConcurrencyErrorResult();
//         }
//     }
// }
//
// public sealed record SetProfilePhoneNumberCommand(ProfileId ProfileId, string? PhoneNumber)
//     : ICommand<SetProfilePhoneNumberCommandResult>;
//
// [Result<SetProfilePhoneNumberCommandOkResult, SetProfilePhoneNumberConcurrencyErrorResult>]
// public partial class SetProfilePhoneNumberCommandResult;
//
// public sealed record SetProfilePhoneNumberCommandOkResult(ProfileId Id);
//
// public sealed record SetProfilePhoneNumberConcurrencyErrorResult;
