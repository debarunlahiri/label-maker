namespace LabelForge.Core.Models.DataSources;

public class PrintTimeInput
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string InputType { get; set; } = "TextBox";
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string? ValidationRegex { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public string? AllowedValues { get; set; }
    public string? TargetObject { get; set; }
    public Guid TemplateId { get; set; }
}