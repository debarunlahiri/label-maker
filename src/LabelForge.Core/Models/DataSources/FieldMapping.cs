namespace LabelForge.Core.Models.DataSources;

public class FieldMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DataSourceId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string SourceField { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public string? Format { get; set; }
    public DataSource? DataSource { get; set; }
}