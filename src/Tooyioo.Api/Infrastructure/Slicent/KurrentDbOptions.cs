using System.ComponentModel.DataAnnotations;

namespace Tooyioo.Api.Infrastructure.Slicent;

public sealed class KurrentDbOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required]
    public string KurrentDb { get; init; } = string.Empty;
}
