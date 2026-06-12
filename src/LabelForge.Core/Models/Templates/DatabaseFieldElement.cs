namespace LabelForge.Core.Models.Templates;

public class DatabaseFieldElement : LabelElement
{
    public string DataSourceId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string FontFamily { get; set; } = "Arial";
    public double FontSize { get; set; } = 12;
    public bool Bold { get; set; }
    public string TextColor { get; set; } = "#000000";
    public string? Format { get; set; }

    public DatabaseFieldElement()
    {
        Type = Enums.ElementType.DatabaseField;
        Width = 120;
        Height = 25;
    }
}