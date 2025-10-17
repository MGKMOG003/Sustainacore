using System;
using System.Collections.Generic;

namespace Sustainacore.Domain.Entities
{
    /// <summary>
    /// Represents a project in the system.  A project has a unique identifier,
    /// a name, optional description, dates and a list of user IDs assigned to it.
    /// </summary>
    public class Project
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public List<string> UserIds { get; set; } = new();

        public Project() { }

        public Project(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
