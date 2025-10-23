using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sustainacore.Contracts;
using System.Net.Http.Json;

namespace Sustainacore.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "AdminOnly")]
public class AdminUsersController : ControllerBase
{
    private readonly FirestoreDb _db;
    private readonly string _projectId;
    private readonly HttpClient _http;

    public AdminUsersController(FirestoreDb db, IConfiguration cfg, IHttpClientFactory httpFactory)
    {
        _db = db;
        _projectId = cfg.GetSection("Firebase")["ProjectId"]!;
        _http = httpFactory.CreateClient();
    }

    [HttpGet]
    public async Task<ActionResult<IList<UserDto>>> GetAll()
    {
        var list = new List<UserDto>();
        var paged = FirebaseAuth.DefaultInstance.ListUsersAsync(null);
        await foreach (var u in paged)
        {
            u.CustomClaims.TryGetValue("role", out var roleObj);
            list.Add(new UserDto(
                u.Uid,
                u.Email ?? "",
                u.DisplayName ?? "",
                roleObj?.ToString(),
                u.EmailVerified
            ));
        }
        return Ok(list.OrderBy(u => u.Email));
    }

    [HttpPatch("{uid}/role")]
    public async Task<IActionResult> UpdateRole(string uid, [FromBody] UpdateRoleRequest req)
    {
        var allowed = new[] { "Admin", "ProjectManager", "Contractor", "Client" };
        if (!allowed.Contains(req.Role)) return BadRequest("Invalid role.");

        var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        var claims = user.CustomClaims?.ToDictionary(kv => kv.Key, kv => kv.Value)
                     ?? new Dictionary<string, object>();
        claims["role"] = req.Role;
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

        // Firestore profile
        var doc = _db.Collection("users").Document(uid);
        await doc.SetAsync(new
        {
            email = user.Email,
            displayName = user.DisplayName,
            role = req.Role,
            updatedAt = FieldValue.ServerTimestamp
        }, SetOptions.MergeAll);

        // Realtime DB mirror (simple PATCH)
        var rtdbUrl = $"https://{_projectId}-default-rtdb.firebaseio.com/users/{uid}.json";
        await _http.PatchAsJsonAsync(rtdbUrl, new
        {
            email = user.Email,
            displayName = user.DisplayName,
            role = req.Role,
            updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });

        return NoContent();
    }
}
