// Domain/Entities/BloodType.cs
using Health.Domain.Common;
using Health.Domain.Entities;

public class BloodType : BaseEntity
{
    public int BloodTypeId { get; set; }
    public string Name { get; set; } = null!; // e.g., "A+", "O-"
    public string? Description { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}
