// using Slicent.Application.Queries;
// // ReSharper disable ConvertToPrimaryConstructor
// // ReSharper disable ClassNeverInstantiated.Global
//
// namespace Tooyioo.Profile.ReadModel;
//
// public sealed record ProfileDocument
//     : Document
// {
//     public ProfileDocument(string Id) 
//         : base(Id)
//     {
//     }
//
//     public required string ExternalId { get; init; }
//     public required string ExternalIdProvider { get; init; }
//     public required string Name { get; init; }
//     public required string LastName { get; init; }
//     public required string Email { get; init; }
//     public required string PhoneNumber { get; init; }
//     public required bool IsEmailVerified { get; init; }
//     public required string Alias { get; init; }
//     public required bool IsComplete { get; init; }
//     public required DateTime? CompletedAt { get; init; }
// }