namespace LabelForge.Core.Models.Templates;

public class DateTimeElement : LabelElement
{
    public string Format { get; set; } = "dd-MM-yyyy";
    public DateTimeValueType ValueType { get; set; } = DateTimeValueType.CurrentDate;
    public string? CustomFormat { get; set; }
    public bool UsePrinterDate { get; set; }
    public bool UseServerDate { get; set; }
    public int OffsetDays { get; set; }
    public string? OffsetDirection { get; set; } = "Add";

    public DateTimeElement()
    {
        Type = Enums.ElementType.DateTime;
        Width = 120;
        Height = 25;
    }
}

public enum DateTimeValueType
{
    CurrentDate,
    CurrentTime,
    CurrentDateTime,
    CustomFormat,
    OffsetDate,
    ExpiryDate
}