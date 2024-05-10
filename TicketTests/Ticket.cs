using System.Globalization;
using System.Text.RegularExpressions;

namespace TicketTests;

public class Ticket
{
    // <d d . m m . y y y y>
    [PrintProperty(3, @"\d \d \. \d \d \. \d \d \d \d")]
    public string DeraptureDateString { get; init; }
    private DateOnly? _deraptureDate;
    public DateOnly DepartureDate
    {
        get
        {
            _deraptureDate ??= DateOnly.ParseExact(
                DeraptureDateString.Replace(" ", ""),
                "dd.MM.yyyy", DateTimeFormatInfo.InvariantInfo);

            return (DateOnly)_deraptureDate;
        }
    }
    
    // <от FR-OM. 75 FR >
    [PrintProperty(4, @"^от\s+\w(?:\w|\.| |-)+\w\s+$")]
    public string FromString { get; init; }
    private string? _from;
    public string From
    {
        get
        {
            _from ??= new Regex(@"\s\w(?:\w|\.| |-)+\w").Match(FromString).Value[1..];
            return _from;
        }
    }
    
    // <до TO-TO. 75 TO >
    [PrintProperty(5, @"^до\s+\w(?:\w|\.| |-)+\w\s+$")]
    public string ToString { get; init; }
    private string? _to;
    public string To
    {
        get
        {
            _to ??= new Regex(@"\s\w(?:\w|\.| |-)+\w").Match(ToString).Value[1..];
            return _to;
        }
    }
    
    [PrintProperty(6, @"^Билет N: \d{5}")]
    public string NumberString { get; init; }
    private string? _number;
    public string Number
    {
        get
        {
            _number ??= NumberString[^5..];
            return _number;
        }
    }
    
    [PrintProperty(6, @"Сист\.N: \d{13}$")]
    public string SystemNumberString { get; init; }
    private string? _systemNumber;
    public string SystemNumber
    {
        get
        {
            _systemNumber ??= SystemNumberString[^13..];
            return _systemNumber;
        }
    }
    
    [PrintProperty(7, @"Перевозка \w+ \w+ -> П\s+\d+")]
    public string ShippingString { get; init; }
    
    private string? _longTerm;
    public string LongTerm
    {
        get
        {
            _longTerm ??= new Regex(@"\w+")
                .Match(ShippingString).NextMatch()
                .Value;
            return _longTerm;
        }
    }
    
    private string? _fullness;
    public string Fullness
    {
        get
        {
            _fullness ??= new Regex(@"\w+")
                .Match(ShippingString).NextMatch().NextMatch()
                .Value;
            return _fullness;
        }
    }
    
    private int? _referenceCost;
    public int BaseCost
    {
        get
        {
            _referenceCost ??= int.Parse(new Regex(@"\d+").Match(ShippingString).Value);
            return (int)_referenceCost;
        }
    }
    
    [PrintProperty(8, @"^Стоимость по тарифу:\s+=\d+[\.,]\d{2}")]
    public string FareCostString { get; init; }
    private float? _fareCost;
    public float FareCost
    {
        get
        {
            if (_fareCost is null)
            {
                int equalSignIndex = FareCostString.LastIndexOf('=');
                string fareCost = FareCostString[(equalSignIndex + 1)..].Replace(",", ".");
                _fareCost = float.Parse(fareCost);
            }
            return (float)_fareCost;
        }
    }
    
    [PrintProperty(9, @"ИТОГ: \d+[\.,]\d{2}")]
    public string TotalCostString { get; init; }
    private float? _totalCost;
    public float TotalCost
    {
        get
        {
            _totalCost ??= float.Parse(TotalCostString[6..].Replace(",", "."));
            return (float)_totalCost;
        }
    }
}