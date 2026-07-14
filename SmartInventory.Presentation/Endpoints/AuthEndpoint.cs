using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;
using SmartInventory.Presentation.Authentication;

namespace SmartInventory.Presentation.Endpoints;

public sealed class AuthEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .AllowAnonymous();

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Create an account and sign in");

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Sign in with email and password");

        group.MapPost("/signin", Login)
            .WithName("SignIn")
            .WithSummary("Alias for login");

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .WithSummary("Rotate a refresh token and issue a new access token");

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithSummary("Revoke a refresh token");
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        IApplicationDbContext dbContext,
        PasswordService passwordService,
        JwtTokenService tokenService,
        CancellationToken cancellationToken)
    {
        var validationErrors = ValidateRegistration(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var email = request.Email.Trim();
        var normalizedEmail = NormalizeEmail(email);
        var emailExists = await dbContext.Users.AnyAsync(
            user => user.NormalizedEmail == normalizedEmail && user.DeletedAt == null,
            cancellationToken);

        if (emailExists)
        {
            return Results.Conflict(new { error = "An account with this email already exists." });
        }

        var user = new User
        {
            Email = email,
            NormalizedEmail = normalizedEmail,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = passwordService.Hash(request.Password)
        };

        dbContext.Users.Add(user);
        var refreshToken = AddRefreshToken(dbContext, user, tokenService);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created("/api/auth/login", CreateTokenResponse(user, refreshToken, tokenService));
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IApplicationDbContext dbContext,
        PasswordService passwordService,
        JwtTokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrEmpty(request.Password)
            || request.Password.Length > 128)
        {
            return InvalidCredentials();
        }

        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await dbContext.Users.SingleOrDefaultAsync(
            candidate => candidate.NormalizedEmail == normalizedEmail && candidate.DeletedAt == null,
            cancellationToken);

        if (user is null || !user.IsActive || !passwordService.Verify(request.Password, user.PasswordHash))
        {
            return InvalidCredentials();
        }

        var refreshToken = AddRefreshToken(dbContext, user, tokenService);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(CreateTokenResponse(user, refreshToken, tokenService));
    }

    private static async Task<IResult> Refresh(
        RefreshTokenRequest request,
        IApplicationDbContext dbContext,
        JwtTokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return InvalidRefreshToken();
        }

        var tokenHash = JwtTokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive
            || !storedToken.User.IsActive || storedToken.User.DeletedAt is not null)
        {
            return InvalidRefreshToken();
        }

        var newRawToken = tokenService.CreateRefreshToken();
        var newTokenHash = JwtTokenService.HashRefreshToken(newRawToken);
        storedToken.Revoke(newTokenHash);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = newTokenHash,
            ExpiresAt = tokenService.GetRefreshTokenExpiry()
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(CreateTokenResponse(storedToken.User, newRawToken, tokenService));
    }

    private static async Task<IResult> Logout(
        RefreshTokenRequest request,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var tokenHash = JwtTokenService.HashRefreshToken(request.RefreshToken);
            var storedToken = await dbContext.RefreshTokens.SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

            if (storedToken?.IsActive == true)
            {
                storedToken.Revoke();
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return Results.NoContent();
    }

    private static string AddRefreshToken(
        IApplicationDbContext dbContext,
        User user,
        JwtTokenService tokenService)
    {
        var rawToken = tokenService.CreateRefreshToken();
        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = JwtTokenService.HashRefreshToken(rawToken),
            ExpiresAt = tokenService.GetRefreshTokenExpiry()
        });

        return rawToken;
    }

    private static TokenResponse CreateTokenResponse(
        User user,
        string refreshToken,
        JwtTokenService tokenService)
    {
        var accessToken = tokenService.CreateAccessToken(user);
        return new TokenResponse(
            accessToken.Value,
            refreshToken,
            accessToken.ExpiresAt,
            "Bearer");
    }

    private static Dictionary<string, string[]> ValidateRegistration(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors[nameof(request.DisplayName)] = ["Display name is required."];
        }
        else if (request.DisplayName.Trim().Length > 200)
        {
            errors[nameof(request.DisplayName)] = ["Display name cannot exceed 200 characters."];
        }

        if (string.IsNullOrWhiteSpace(request.Email)
            || request.Email.Trim().Length > 256
            || !new EmailAddressAttribute().IsValid(request.Email.Trim()))
        {
            errors[nameof(request.Email)] = ["A valid email address is required."];
        }

        if (string.IsNullOrEmpty(request.Password)
            || request.Password.Length is < 8 or > 128)
        {
            errors[nameof(request.Password)] = ["Password must contain between 8 and 128 characters."];
        }

        return errors;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    private static IResult InvalidCredentials() =>
        Results.Unauthorized();

    private static IResult InvalidRefreshToken() =>
        Results.Json(new { error = "Refresh token is invalid or expired." }, statusCode: StatusCodes.Status401Unauthorized);
}

public sealed record RegisterRequest(string DisplayName, string Email, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string TokenType);
