// using Eventuous;
// using Funzo;
// using Tooyioo.Profile.Contracts;
//
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Domain.UniqueAlias;
//
// public sealed class UniqueAliasAggregate
//     : Aggregate<UniqueAliasState>
// {
//     public ClaimUniqueAliasResult Claim(ProfileId profileId)
//     {
//         if (State.IdentityId is not null)
//         {
//             return profileId == State.IdentityId 
//                 ? ClaimUniqueAliasResult.Ok() 
//                 : new UniqueAliasAlreadyClaimedError(State.IdentityId!);
//         }
//         
//         var uniqueExternalIdentityClaimed = new UniqueAliasDomainEvents.V1.Claimed(profileId);
//         Apply(uniqueExternalIdentityClaimed);
//         return ClaimUniqueAliasResult.Ok();
//     }
// }
//
// [Result<UniqueAliasAlreadyClaimedError>]
// public partial class ClaimUniqueAliasResult;
//
// public sealed record UniqueAliasAlreadyClaimedError(ProfileId ProfileAliasClaimedBy);