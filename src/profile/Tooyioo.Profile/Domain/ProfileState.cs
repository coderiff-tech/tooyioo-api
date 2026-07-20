// using Eventuous;
// using Tooyioo.Profile.Contracts;
//
// namespace Tooyioo.Profile.Domain;
//
// public sealed record ProfileState
//     : State<ProfileState, ProfileId>
// {
//     public string FirstName { get; private init; } = null!;
//     public string LastName { get; private init; } = null!;
//     public string Email { get; private init; } = null!;
//     public bool IsEmailVerified { get; private init; }
//     public string ExternalId { get; private init; } = null!;
//     public string ExternalProvider { get; private init; } = null!;
//     public string? Alias { get; private init; }
//     public string? PhoneNumber { get; private init; }
//     public bool IsProfileComplete { get; private init; }
//     
//     public ProfileState()
//     {
//         On<ProfileDomainEvents.V1.Created>(Created);
//         On<ProfileDomainEvents.V1.EmailVerified>(EmailVerified);
//         On<ProfileDomainEvents.V1.ExternalIdAssociated>(ExternalIdentityAssociated);
//         On<ProfileDomainEvents.V1.AliasSet>(AliasSet);
//         On<ProfileDomainEvents.V1.PhoneNumberSet>(PhoneNumberSet);
//         On<ProfileDomainEvents.V1.Completed>(Completed);
//     }
//
//     private static ProfileState Created(
//         ProfileState state, 
//         ProfileDomainEvents.V1.Created domainEvent)
//     {
//         return state with
//         {
//             FirstName = domainEvent.Name,
//             LastName = domainEvent.LastName,
//             Email = domainEvent.Email
//         };
//     }
//     
//     private static ProfileState EmailVerified(
//         ProfileState state, 
//         ProfileDomainEvents.V1.EmailVerified domainEvent)
//     {
//         return state with
//         {
//             IsEmailVerified = true
//         };
//     }
//     
//     private static ProfileState ExternalIdentityAssociated(
//         ProfileState state, 
//         ProfileDomainEvents.V1.ExternalIdAssociated domainEvent)
//     {
//         return state with
//         {
//             ExternalId = domainEvent.ExternalId,
//             ExternalProvider = domainEvent.ExternalIdProvider
//         };
//     }
//     
//     private static ProfileState AliasSet(
//         ProfileState state, 
//         ProfileDomainEvents.V1.AliasSet domainEvent)
//     {
//         return state with
//         {
//             Alias = domainEvent.Alias
//         };
//     }
//     
//     private static ProfileState PhoneNumberSet(
//         ProfileState state, 
//         ProfileDomainEvents.V1.PhoneNumberSet domainEvent)
//     {
//         return state with
//         {
//             PhoneNumber = domainEvent.PhoneNumber
//         };
//     }
//     
//     private static ProfileState Completed(
//         ProfileState state, 
//         ProfileDomainEvents.V1.Completed domainEvent)
//     {
//         return state with
//         {
//             IsProfileComplete = true
//         };
//     }
// }