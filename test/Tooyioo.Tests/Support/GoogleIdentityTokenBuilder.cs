using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Tooyioo.Common;
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.Tests.Support;

public sealed class GoogleIdentityTokenBuilder
{
    public const string Issuer = "https://accounts.google.com";

    private readonly byte[] _signingKey;
    private string _subject = "google-sub-123";
    private string _name = "Joe";
    private string _lastName = "Bloggs";
    private string _email = "joe.bloggs@example.com";
    private bool _isEmailVerified = true;

    public GoogleIdentityTokenBuilder(byte[] signingKey)
    {
        _signingKey = signingKey;
    }

    public GoogleIdentityTokenBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public GoogleIdentityTokenBuilder WithPersonalDetails(
        string name,
        string lastName,
        string email,
        bool isEmailVerified)
    {
        _name = name;
        _lastName = lastName;
        _email = email;
        _isEmailVerified = isEmailVerified;
        return this;
    }

    public string Build()
    {
        var securityKey = new SymmetricSecurityKey(_signingKey);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "https://accounts.google.com",
            audience: "tooyioo-tests",
            claims:
            [
                new Claim(Claims.Sub, _subject),
                new Claim(Claims.GivenName, _name),
                new Claim(Claims.FamilyName, _lastName),
                new Claim(Claims.Email, _email),
                new Claim(Claims.EmailVerified, _isEmailVerified.ToString().ToLowerInvariant())
            ],
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static byte[] CreateSigningKey() => "this-is-a-sample-key-to-use-when-signing-test-jwt"u8.ToArray();
}
