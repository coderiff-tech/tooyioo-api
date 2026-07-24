// ReSharper disable ClassNeverInstantiated.Global

using Microsoft.AspNetCore.Mvc;

namespace Tooyioo.User.Features.RetrieveUsers.Contract;

public record RetrieveUsersRoute
{
    [FromQuery]
    public string? Id { get; init; }

    [FromQuery]
    public string? Alias { get; init; }
    
    [FromQuery]
    public string? Name { get; init; }
    
    [FromQuery]
    public string? LastName { get; init; }
    
    [FromQuery]
    public string? Email { get; init; }
    
    [FromQuery]
    public bool? IsEmailVerified { get; init; }

    [FromQuery]
    public int? PageNumber { get; init; }

    [FromQuery]
    public int? PageSize { get; init; }
    
    [FromQuery]
    public DateTime? CreatedAtFrom { get; init; }
    
    [FromQuery]
    public DateTime? CreatedAtTo { get; init; }
    
    [FromQuery]
    public DateTime? LastModifiedAtFrom { get; init; }
    
    [FromQuery]
    public DateTime? LastModifiedAtTo { get; init; }
}
