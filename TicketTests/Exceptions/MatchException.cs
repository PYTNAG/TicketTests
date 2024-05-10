namespace TicketTests.Exceptions;

public class MatchException : Exception
{
    public string Line { get; }
    public int LineNumber { get; }
    public string PropertyName { get; }
    public string Pattern { get; }
    
    public MatchException(string line, int lineNumber, string propertyName, string pattern)
        : base($"Line {line} ({lineNumber + 1}) doesn't match {propertyName} with pattern {pattern}.")
    {
        Line = line;
        LineNumber = lineNumber;
        PropertyName = propertyName;
        Pattern = pattern;
    }
}