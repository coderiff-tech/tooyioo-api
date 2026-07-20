// ReSharper disable UseUtf8StringLiteral
namespace Tooyioo.Tests.Support.Extensions;

public static class GuidExtensions
{
    public static Guid ToGuid(this int value) => new(value, 0, 0, [0, 0, 0, 0, 0, 0, 0, 0]);
}
