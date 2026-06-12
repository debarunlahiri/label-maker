using LabelForge.Core.Models.Automation;
using LabelForge.Core.Models.DataSources;
using LabelForge.Core.Models.Printing;
using LabelForge.Core.Models.System;
using LabelForge.Core.Models.Templates;
using LabelForge.Core.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace LabelForge.Database;

public class LabelForgeDbContext : DbContext
{
    public LabelForgeDbContext(DbContextOptions<LabelForgeDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    public DbSet<LabelTemplate> Templates => Set<LabelTemplate>();
    public DbSet<TemplateVersion> TemplateVersions => Set<TemplateVersion>();
    public DbSet<LabelElement> Elements => Set<LabelElement>();

    public DbSet<PrinterInfo> Printers => Set<PrinterInfo>();
    public DbSet<PrintJob> PrintJobs => Set<PrintJob>();
    public DbSet<PrintJobLog> PrintJobLogs => Set<PrintJobLog>();

    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<DataSourceConnection> DataSourceConnections => Set<DataSourceConnection>();
    public DbSet<FieldMapping> FieldMappings => Set<FieldMapping>();
    public DbSet<FormulaField> FormulaFields => Set<FormulaField>();
    public DbSet<PrintTimeInput> PrintTimeInputs => Set<PrintTimeInput>();

    public DbSet<GlobalVariable> GlobalVariables => Set<GlobalVariable>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<IntegrationTrigger> IntegrationTriggers => Set<IntegrationTrigger>();
    public DbSet<IntegrationLog> IntegrationLogs => Set<IntegrationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Module).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Role).WithMany(r => r.UserRoles).HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.HasOne(e => e.Role).WithMany(r => r.RolePermissions).HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.Permission).WithMany(p => p.RolePermissions).HasForeignKey(e => e.PermissionId);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.KeyHash);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<LabelTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnType("text");
            entity.HasMany(e => e.Elements).WithOne().HasForeignKey("TemplateId").OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Versions).WithOne(v => v.Template).HasForeignKey(v => v.TemplateId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TemplateVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TemplateId, e.VersionNumber }).IsUnique();
            entity.Property(e => e.TemplateJson).HasColumnType("text").IsRequired();
            entity.Property(e => e.ChangeComment).HasColumnType("text");
        });

        modelBuilder.Entity<LabelElement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<PrinterInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PrinterType).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(100);
            entity.Property(e => e.DriverName).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Department).HasMaxLength(200);
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.PayloadJson).HasColumnType("text");
            entity.Property(e => e.ErrorMessage).HasColumnType("text");
            entity.HasMany(e => e.Logs).WithOne(l => l.PrintJob).HasForeignKey(l => l.PrintJobId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PrintJobLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Message).HasColumnType("text");
        });

        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ConnectionString).HasColumnType("text");
            entity.HasMany(e => e.FieldMappings).WithOne(f => f.DataSource).HasForeignKey(f => f.DataSourceId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DataSourceConnection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ConnectionString).HasColumnType("text");
        });

        modelBuilder.Entity<FieldMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ObjectName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FieldName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SourceField).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<FormulaField>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Expression).HasColumnType("text").IsRequired();
        });

        modelBuilder.Entity<PrintTimeInput>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Label).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<GlobalVariable>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Value).HasColumnType("text");
        });

        modelBuilder.Entity<SystemSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Value).HasColumnType("text");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Action);
            entity.Property(e => e.Action).HasMaxLength(200).IsRequired();
            entity.Property(e => e.OldValue).HasColumnType("text");
            entity.Property(e => e.NewValue).HasColumnType("text");
            entity.Property(e => e.ErrorMessage).HasColumnType("text");
        });

        modelBuilder.Entity<Integration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Configuration).HasColumnType("text");
            entity.Property(e => e.FieldMapping).HasColumnType("text");
            entity.HasMany(e => e.Triggers).WithOne(t => t.Integration).HasForeignKey(t => t.IntegrationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Logs).WithOne(l => l.Integration).HasForeignKey(l => l.IntegrationId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IntegrationTrigger>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TriggerType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Configuration).HasColumnType("text");
        });

        modelBuilder.Entity<IntegrationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InputData).HasColumnType("text");
            entity.Property(e => e.MappedData).HasColumnType("text");
            entity.Property(e => e.OutputResult).HasColumnType("text");
            entity.Property(e => e.ErrorMessage).HasColumnType("text");
        });
    }
}