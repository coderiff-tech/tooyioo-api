// // ReSharper disable ClassNeverInstantiated.Global
//
// using Microsoft.AspNetCore.Mvc;
//
// namespace Tooyioo.Profile.Features.RetrieveAll.Contracts;
//
// public record RetrieveAllProfileRoute
// {
//     [FromQuery]
//     public string? Alias { get; init; }
//     
//     [FromQuery]
//     public required bool? IsComplete { get; init; }
//
//     [FromQuery]
//     public int? PageNumber { get; init; }
//
//     [FromQuery]
//     public int? PageSize { get; init; }
//     
//     [FromQuery]
//     public DateTime? CreatedAtFrom { get; init; }
//     
//     [FromQuery]
//     public DateTime? CreatedAtTo { get; init; }
//     
//     [FromQuery]
//     public DateTime? LastModifiedAtFrom { get; init; }
//     
//     [FromQuery]
//     public DateTime? LastModifiedAtTo { get; init; }
//     
//     [FromQuery]
//     public DateTime? CompletedAtFrom { get; init; }
//     
//     [FromQuery]
//     public DateTime? CompletedAtTo { get; init; }
// }
