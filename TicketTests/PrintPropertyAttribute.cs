namespace TicketTests;

[AttributeUsage(AttributeTargets.Property)]
public class PrintPropertyAttribute(int line, string pattern) : Attribute
{
    public int Line { get; } = line;
    public string Pattern { get; } = pattern;
}