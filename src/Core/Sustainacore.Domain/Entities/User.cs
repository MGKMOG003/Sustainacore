using System;

namespace Sustainacore.Domain.Entities
{
    /// <summary>
    /// Represents an application user.  This entity is
    /// separate from any external authentication provider (e.g., Firebase).
    /// </summary>
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = "Client";
    }
}

