using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sustainacore.Application.Interfaces;
using Sustainacore.Application.Services;
using Sustainacore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// strongly-typed Firebase settings
var fb = builder.Configuration.GetSection("Firebase");
var projectId = fb["ProjectId"];
var apiKey = fb["ApiKey"];
var tokenIssuer = fb["TokenIssuer"]; // https://securetoken.google.com/<projectId>

JwtSecurityTokenHandler.DefaultMapInboundClaims = false; // don't remap to MS names

// Razor Pages with area security
builder.Services
    .AddRazorPages(options =>
    {
        options.Conventions.AuthorizeAreaFolder("Admin", "/", "AdminOnly");
        options.Conventions.AuthorizeAreaFolder("ProjectManager", "/", "PMOnly");
        options.Conventions.AuthorizeAreaFolder("Contractor", "/", "ContractorOnly");
        options.Conventions.AuthorizeAreaFolder("Client", "/", "ClientOnly");

        // Allow anonymous for /Account (login/register)
        options.Conventions.AllowAnonymousToFolder("/Account");
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = tokenIssuer;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = tokenIssuer,
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        // Allow tokens from cookie (web) OR Authorization header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (string.IsNullOrEmpty(ctx.Token) &&
                    ctx.Request.Cookies.TryGetValue("id_token", out var cookieToken))
                    ctx.Token = cookieToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    opts.AddPolicy("PMOnly", p => p.RequireRole("ProjectManager"));
    opts.AddPolicy("ContractorOnly", p => p.RequireRole("Contractor"));
    opts.AddPolicy("ClientOnly", p => p.RequireRole("Client"));
});

builder.Services.AddHttpClient("FirebaseAuth", c =>
{
    c.BaseAddress = new Uri(fb["AuthBase"]!); // https://identitytoolkit.googleapis.com/v1
});

builder.Services.AddSingleton(new FirebaseConfig(
    ProjectId: projectId!,
    ApiKey: apiKey!,
    AuthBase: fb["AuthBase"]!,
    TokenIssuer: tokenIssuer!
));

// Register domain/application services
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
builder.Services.AddScoped<ProjectService>();

builder.Services.AddScoped<FirebaseAuthService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// (Optional) Admin API to set role via Admin SDK later if you want C# route instead of Node seeder
// app.MapPost("/api/admin/assign-role", [Authorize(Policy="AdminOnly")] ...);

app.Run();

public record FirebaseConfig(string ProjectId, string ApiKey, string AuthBase, string TokenIssuer);
