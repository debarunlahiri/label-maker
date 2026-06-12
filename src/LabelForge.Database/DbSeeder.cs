using LabelForge.Core.Enums;
using LabelForge.Core.Models.Automation;

namespace LabelForge.Database;

public static class DbSeeder
{
    public static async Task SeedAsync(LabelForgeDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedPermissionsAsync(context);
        await SeedSystemSettingsAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(LabelForgeDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = Enum.GetNames<UserRole>().Select(name => new Core.Models.Users.Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = $"{name} role"
            }).ToList();
            context.Roles.AddRange(roles);
        }
    }

    private static async Task SeedPermissionsAsync(LabelForgeDbContext context)
    {
        if (!context.Permissions.Any())
        {
            var permissions = new[]
            {
                "CanCreateTemplate", "CanEditTemplate", "CanDeleteTemplate", "CanApproveTemplate",
                "CanPrintTemplate", "CanReprintLabel", "CanConfigurePrinter", "CanConfigureDataSource",
                "CanCreateAutomationTrigger", "CanViewAuditLogs", "CanManageUsers", "CanManageGlobalVariables",
                "CanAccessRestApi"
            };
            context.Permissions.AddRange(permissions.Select(p => new Core.Models.Users.Permission
            {
                Id = Guid.NewGuid(),
                Name = p,
                Module = GetModuleForPermission(p)
            }));
        }
    }

    private static async Task SeedSystemSettingsAsync(LabelForgeDbContext context)
    {
        if (!context.SystemSettings.Any())
        {
            var defaults = new Dictionary<string, string>
            {
                ["AppName"] = "LabelForge Studio",
                ["Version"] = "1.0.0",
                ["DefaultPrinterSyncInterval"] = "30",
                ["InactivePrinterSyncInterval"] = "300",
                ["MaxConcurrentApiRequests"] = "100",
                ["MaxBatchRecords"] = "10000",
                ["PasswordMinLength"] = "8",
                ["PasswordRequireUppercase"] = "true",
                ["PasswordRequireLowercase"] = "true",
                ["PasswordRequireNumber"] = "true",
                ["MaxLoginAttempts"] = "5"
            };
            context.SystemSettings.AddRange(defaults.Select(kv => new Core.Models.System.SystemSettings
            {
                Id = Guid.NewGuid(),
                Key = kv.Key,
                Value = kv.Value
            }));
        }
    }

    private static string GetModuleForPermission(string permission)
    {
        if (permission.Contains("Template")) return "Templates";
        if (permission.Contains("Printer")) return "Printers";
        if (permission.Contains("DataSource")) return "DataSources";
        if (permission.Contains("Automation") || permission.Contains("Trigger")) return "Automation";
        if (permission.Contains("Audit")) return "Audit";
        if (permission.Contains("User")) return "Users";
        if (permission.Contains("Variable")) return "System";
        if (permission.Contains("Api")) return "Api";
        if (permission.Contains("Print") || permission.Contains("Reprint")) return "Printing";
        return "General";
    }
}