namespace LabelForge.Core.Models.Templates;

public class RFIDElement : LabelElement
{
    public string EpcValue { get; set; } = string.Empty;
    public string? UserMemoryValue { get; set; }
    public string? AccessPassword { get; set; }
    public string? KillPassword { get; set; }
    public string MemoryBank { get; set; } = "EPC";
    public string EncodingScheme { get; set; } = "ISO18000-6C";
    public bool ReadAfterWrite { get; set; } = true;
    public int RetryCount { get; set; } = 3;
    public bool VoidOnFailure { get; set; } = true;
    public string? PrinterRfidProfile { get; set; }

    public RFIDElement()
    {
        Type = Enums.ElementType.RFID;
        Width = 80;
        Height = 25;
    }
}