using Microsoft.Maui.Graphics;

namespace LabelMaker.Models;

public class RFIDElement : LabelElement
{
    private string _epcValue = "";
    private string _userMemoryValue = "";
    private string _accessPassword = "";
    private string _killPassword = "";
    private string _memoryBank = "EPC";
    private string _encodingScheme = "ISO18000-6C";
    private bool _readAfterWrite = true;
    private int _retryCount = 3;
    private bool _voidOnFailure = true;
    private string _printerRfidProfile = "";
    private string _fontFamily = "Arial";
    private double _fontSize = 10;
    private Color _textColor = Colors.Black;

    public RFIDElement()
    {
        Type = ElementType.RFID;
        Name = "RFID";
        Width = 80;
        Height = 25;
    }

    public string EpcValue
    {
        get => _epcValue;
        set { _epcValue = value; OnPropertyChanged(); }
    }

    public string UserMemoryValue
    {
        get => _userMemoryValue;
        set { _userMemoryValue = value; OnPropertyChanged(); }
    }

    public string AccessPassword
    {
        get => _accessPassword;
        set { _accessPassword = value; OnPropertyChanged(); }
    }

    public string KillPassword
    {
        get => _killPassword;
        set { _killPassword = value; OnPropertyChanged(); }
    }

    public string MemoryBank
    {
        get => _memoryBank;
        set { _memoryBank = value; OnPropertyChanged(); }
    }

    public string EncodingScheme
    {
        get => _encodingScheme;
        set { _encodingScheme = value; OnPropertyChanged(); }
    }

    public bool ReadAfterWrite
    {
        get => _readAfterWrite;
        set { _readAfterWrite = value; OnPropertyChanged(); }
    }

    public int RetryCount
    {
        get => _retryCount;
        set { _retryCount = value; OnPropertyChanged(); }
    }

    public bool VoidOnFailure
    {
        get => _voidOnFailure;
        set { _voidOnFailure = value; OnPropertyChanged(); }
    }

    public string PrinterRfidProfile
    {
        get => _printerRfidProfile;
        set { _printerRfidProfile = value; OnPropertyChanged(); }
    }

    public string FontFamily
    {
        get => _fontFamily;
        set { _fontFamily = value; OnPropertyChanged(); }
    }

    public double FontSize
    {
        get => _fontSize;
        set { _fontSize = value; OnPropertyChanged(); }
    }

    public Color TextColor
    {
        get => _textColor;
        set { _textColor = value; OnPropertyChanged(); }
    }
}