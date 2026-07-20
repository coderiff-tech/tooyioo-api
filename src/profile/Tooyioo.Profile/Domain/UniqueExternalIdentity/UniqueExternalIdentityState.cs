// using Eventuous;
// using Tooyioo.Profile.Contracts;
//
// namespace Tooyioo.Profile.Domain.UniqueExternalIdentity;
//
// public sealed record UniqueExternalIdentityState
//     : State<UniqueExternalIdentityState, UniqueExternalIdentityId>
// {
//     public ProfileId? ProfileId { get; private init; }
//     
//     public UniqueExternalIdentityState()
//     {
//         On<UniqueExternalIdDomainEvents.V1.Claimed>(UniqueExternalIdentityClaimed);
//     }
//
//     private static UniqueExternalIdentityState UniqueExternalIdentityClaimed(
//         UniqueExternalIdentityState state, 
//         UniqueExternalIdDomainEvents.V1.Claimed domainEvent)
//     {
//         return state with
//         {
//             ProfileId = domainEvent.ProfileId
//         };
//     }
// }