using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SmartInventory.Application;
using SmartInventory.Infrastructure;
using SmartInventory.Infrastructure.Persistance.Seeding;
using SmartInventory.Presentation.Authentication;
using SmartInventory.Presentation.Extensions;
using SmartInventory.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints();

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer)
    || string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException("JWT issuer and audience must be configured.");
}

if (Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32)
{
    throw new InvalidOperationException("JWT key must be at least 32 bytes. Configure Jwt:Key or Jwt__Key.");
}

if (jwtOptions.AccessTokenMinutes <= 0 || jwtOptions.RefreshTokenDays <= 0)
{
    throw new InvalidOperationException("JWT token lifetimes must be greater than zero.");
}

builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<PasswordService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Host is "localhost" or "127.0.0.1";
            });
        }
        else
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:3000",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:5174",
                "http://127.0.0.1:3000");
        }

        policy.AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
