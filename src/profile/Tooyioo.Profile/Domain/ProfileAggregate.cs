// using Eventuous;
// using Tooyioo.Profile.Contracts;
//
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.Domain;
//
// public sealed class ProfileAggregate
//     : Aggregate<ProfileState>
// {
//     public void Bootstrap(
//         string name, 
//         string lastName, 
//         string email, 
//         bool isEmailVerified, 
//         string externalId,
//         string externalProvider)
//     {
//         EnsureDoesntExist();
//         
//         var identityCreated = new ProfileDomainEvents.V1.Created(name, lastName, email);
//         Apply(identityCreated);
//
//         if (!string.IsNullOrWhiteSpace(externalId))
//         {
//             var identityExternalIdentityAssociated = 
//                 new ProfileDomainEvents.V1.ExternalIdAssociated(externalId, externalProvider);
//             Apply(identityExternalIdentityAssociated);
//         }
//
//         if (!isEmailVerified)
//         {
//             return;
//         }
//         
//         var identityEmailVerified = new ProfileDomainEvents.V1.EmailVerified();
//         Apply(identityEmailVerified);
//     }
//
//     public void CompleteProfile(
//         string alias)
//     {
//         EnsureExists();
//
//         if (State.IsProfileComplete)
//         {
//             return;
//         }
//         
//         var identityAliasAssociated = new ProfileDomainEvents.V1.AliasSet(alias);
//         Apply(identityAliasAssociated);
//         
//         var identityProfileCompleted = new ProfileDomainEvents.V1.Completed();
//         Apply(identityProfileCompleted);
//     }
//     
//     public void SetProfilePhoneNumber(
//         string? phoneNumber)
//     {
//         EnsureExists();
//
//         if (State.PhoneNumber == phoneNumber)
//         {
//             return;
//         }
//         
//         var identityAliasAssociated = new ProfileDomainEvents.V1.PhoneNumberSet(phoneNumber);
//         Apply(identityAliasAssociated);
//     }
// }