using LabelForge.Core.Models.Users;

namespace LabelForge.Core.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(User user, string password);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ValidateCredentialsAsync(string username, string password);
    Task<string> GenerateJwtTokenAsync(User user);
    Task AssignRoleAsync(Guid userId, Guid roleId);
    Task RemoveRoleAsync(Guid userId, Guid roleId);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
}