using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Presentation.Authentication;

public sealed class JwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);
    }

    public AccessToken CreateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: _signingCredentials);

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public string CreateRefreshToken() =>
        WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

    public DateTime GetRefreshTokenExpiry() =>
        DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

    public static string HashRefreshToken(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}

public sealed record AccessToken(string Value, DateTime ExpiresAt);
