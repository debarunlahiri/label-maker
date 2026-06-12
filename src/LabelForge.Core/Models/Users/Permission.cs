namespace LabelForge.Core.Models.Users;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public List<RolePermission> RolePermissions { get; set; } = [];
}