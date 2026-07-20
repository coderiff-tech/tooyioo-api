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
    private string? _issuer = Issuer;
    private string _subject = "google-sub-123";
    private bool _includeSubject = true;
    private string _name = "Joe";
    private string _lastName = "Bloggs";
    private string _email = "joe.bloggs@example.com";
    private bool _isEmailVerified = true;
    private DateTime _notBefore = DateTime.UtcNow.AddMinutes(-1);
    private DateTime _expires = DateTime.UtcNow.AddMinutes(10);

    public GoogleIdentityTokenBuilder(byte[] signingKey)
    {
        _signingKey = signingKey;
    }

    public GoogleIdentityTokenBuilder WithSubject(string subject)
    {
        _subject = subject;
        _includeSubject = true;
        return this;
    }

    public GoogleIdentityTokenBuilder WithoutSubject()
    {
        _includeSubject = false;
        return this;
    }

    public GoogleIdentityTokenBuilder WithIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public GoogleIdentityTokenBuilder WithoutIssuer()
    {
        _issuer = null;
        return this;
    }

    public GoogleIdentityTokenBuilder Expired()
    {
        _notBefore = DateTime.UtcNow.AddMinutes(-20);
        _expires = DateTime.UtcNow.AddMinutes(-10);
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
        var claims = new List<Claim>
        {
            new(Claims.GivenName, _name),
            new(Claims.FamilyName, _lastName),
            new(Claims.Email, _email),
            new(Claims.EmailVerified, _isEmailVerified.ToString().ToLowerInvariant())
        };

        if (_includeSubject)
        {
            claims.Add(new Claim(Claims.Sub, _subject));
        }
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: "tooyioo-tests",
            claims: claims,
            notBefore: _notBefore,
            expires: _expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static byte[] CreateSigningKey() => "this-is-a-sample-key-to-use-when-signing-test-jwt"u8.ToArray();
}
