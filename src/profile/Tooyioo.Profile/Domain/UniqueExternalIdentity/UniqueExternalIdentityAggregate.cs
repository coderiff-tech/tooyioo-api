// using Eventuous;
// using Funzo;
// using Tooyioo.Profile.Contracts;
//
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Domain.UniqueExternalIdentity;
//
// public sealed class UniqueExternalIdentityAggregate
//     : Aggregate<UniqueExternalIdentityState>
// {
//     public ClaimUniqueExternalIdentityResult Claim(ProfileId profileId)
//     {
//         if (State.ProfileId is not null)
//         {
//             return profileId == State.ProfileId 
//                 ? ClaimUniqueExternalIdentityResult.Ok() 
//                 : new UniqueExternalIdentityAlreadyClaimedError(State.ProfileId!);
//         }
//         
//         var uniqueExternalIdentityClaimed = new UniqueExternalIdDomainEvents.V1.Claimed(profileId);
//         Apply(uniqueExternalIdentityClaimed);
//         return ClaimUniqueExternalIdentityResult.Ok();
//     }
// }
//
// [Result<UniqueExternalIdentityAlreadyClaimedError>]
// public partial class ClaimUniqueExternalIdentityResult;
//
// public sealed record UniqueExternalIdentityAlreadyClaimedError(ProfileId ExternalProfileClaimedBy);