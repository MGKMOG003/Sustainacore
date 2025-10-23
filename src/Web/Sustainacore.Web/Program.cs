using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// AuthN/AuthZ (Firebase ID tokens)
var projectId = builder.Configuration["Firebase:ProjectId"];
var issuer = builder.Configuration["Firebase:TokenIssuer"] ?? (projectId is null ? null : $"https://securetoken.google.com/{projectId}");
if (!string.IsNullOrWhiteSpace(projectId) && !string.IsNullOrWhiteSpace(issuer))
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.Authority = issuer;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidateLifetime = true
            };
        });
}
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Standard pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT: AuthN then AuthZ
app.UseAuthentication();
app.UseAuthorization();

// Razor Pages + MVC routes
app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
