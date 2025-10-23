namespace Sustainacore.Api.Config;

public sealed record FirebaseConfig(
    string ProjectId,
    string TokenIssuer,
    string ServiceAccountPath,
    string AuthBase
);
