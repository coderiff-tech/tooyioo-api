// using Eventuous;
//
// namespace Tooyioo.Profile.Contracts;
//
// public static class ProfileDomainEvents
// {
//     public static class V1
//     {
//         private const string Prefix = "V1.Profile";
//         
//         [EventType(Prefix + "Created")]
//         public record Created(string Name, string LastName, string Email);
//         
//         [EventType(Prefix + "ExternalIdAssociated")]
//         public record ExternalIdAssociated(string ExternalId, string ExternalIdProvider);
//         
//         [EventType(Prefix + "EmailVerified")]
//         public record EmailVerified;
//         
//         [EventType(Prefix + "AliasSet")]
//         public record AliasSet(string Alias);
//         
//         [EventType(Prefix + "PhoneNumberSet")]
//         public record PhoneNumberSet(string? PhoneNumber);
//         
//         [EventType(Prefix + "Completed")]
//         public record Completed;
//     }
// }