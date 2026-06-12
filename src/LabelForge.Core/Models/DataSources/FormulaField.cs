namespace LabelForge.Core.Models.DataSources;

public class FormulaField
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty;
    public string ResultType { get; set; } = "String";
    public string? Description { get; set; }
    public Guid TemplateId { get; set; }
}