using System.ComponentModel.DataAnnotations;

namespace Tooyioo.Api.Infrastructure.Slicent;

public sealed class MongoDbOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required]
    public string MongoDb { get; init; } = string.Empty;
}
