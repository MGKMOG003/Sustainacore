using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sustainacore.Api.Config;

var builder = WebApplication.CreateBuilder(args);

// Firebase config
var fb = builder.Configuration.GetSection("Firebase");
var projectId = fb["ProjectId"] ?? throw new InvalidOperationException("Missing Firebase:ProjectId");
var issuer    = fb["TokenIssuer"] ?? $"https://securetoken.google.com/{projectId}";
var svcPath   = fb["ServiceAccountPath"] ?? throw new InvalidOperationException("Missing Firebase:ServiceAccountPath");
var authBase  = fb["AuthBase"] ?? "https://identitytoolkit.googleapis.com/v1";

builder.Services.AddSingleton(new FirebaseConfig(projectId, issuer, svcPath, authBase));

// Firebase Admin SDK
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(svcPath),
    ProjectId = projectId
});

// Firestore
builder.Services.AddSingleton(_ => FirestoreDb.Create(projectId));

// JWT Bearer (Firebase ID tokens)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",      p => p.RequireClaim("role", "Admin"));
    options.AddPolicy("PMOnly",         p => p.RequireClaim("role", "ProjectManager"));
    options.AddPolicy("ContractorOnly", p => p.RequireClaim("role", "Contractor"));
    options.AddPolicy("ClientOnly",     p => p.RequireClaim("role", "Client"));
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

