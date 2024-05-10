using System.Reflection;
using System.Text.RegularExpressions;
using TicketTests.Exceptions;

namespace TicketTests;

public static class TicketParser
{
    private static SortedList<int, List<(string pattern, PropertyInfo property)>>? _registeredProperties;
    
    private static void RegisterTicketProperties()
    {
        _registeredProperties = [];
        
        foreach (PropertyInfo property in typeof(Ticket).GetProperties())
        {
            if (property.PropertyType != typeof(string))
            {
                continue;
            }
            
            var printProperty = property.GetCustomAttribute<PrintPropertyAttribute>();
            if (printProperty is null)
            {
                continue;
            }

            if (!_registeredProperties.ContainsKey(printProperty.Line))
            {
                _registeredProperties.Add(printProperty.Line, new List<(string, PropertyInfo)>());
            }
            
            _registeredProperties[printProperty.Line].Add((printProperty.Pattern, property));
        }
    }

    public static Ticket Parse(string filePath)
    {
        if (_registeredProperties is null)
        {
            RegisterTicketProperties();
        }

        Ticket ticket = new();
        
        using var file = File.OpenText(filePath);

        int lineIndex = -1;
        while (true)
        {
            string? line = file.ReadLine();
            ++lineIndex;
            
            if (line is null)
            {
                break;
            }

            if (!_registeredProperties!
                    .TryGetValue(lineIndex + 1, out List<(string pattern, PropertyInfo property)>? printProperties))
            {
                continue;
            }

            foreach (var printProperty in printProperties)
            {
                Regex regex = new(printProperty.pattern);
                Match match = regex.Match(line);
            
                if (!match.Success)
                {
                    throw new MatchException(line, lineIndex + 1, printProperty.property.Name, printProperty.pattern);
                }

                if (!printProperty.property.CanWrite)
                {
                    throw new Exception($"Property {printProperty.property.Name} must have setter.");
                }
            
                printProperty.property.SetValue(ticket, match.Value);
            }
        }

        return ticket;
    }
}