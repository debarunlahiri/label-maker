using LabelForge.Core.Enums;

namespace LabelForge.Core.Models.DataSources;

public class DataSource
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DataSourceType Type { get; set; }
    public string? Description { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string? Query { get; set; }
    public string? FilePath { get; set; }
    public string? ApiUrl { get; set; }
    public string? ApiHeaders { get; set; }
    public string? ApiMethod { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public List<FieldMapping> FieldMappings { get; set; } = [];
}