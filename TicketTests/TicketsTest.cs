using System.Text.RegularExpressions;

namespace TicketTests;

[TestFixture]
public class TicketsTest
{
    [Test, TestCaseSource(typeof(TicketsSource), nameof(TicketsSource.TicketFiles), new object?[]{ null })]
    public void TestTicketsSource(string ticketFile)
    {
        Assert.DoesNotThrow(() =>
        {
            Ticket ticket = TicketParser.Parse(ticketFile);
            
            Assert.DoesNotThrow(() => { var _ = ticket.DepartureDate; }, "Departure date has wrong format.");
            
            AssertCorrectStationName(ticket.From);
            AssertCorrectStationName(ticket.To);
            
            Assert.That(ticket.SystemNumber, Does.EndWith(ticket.Number), "System number must end with ticket number.");
            Assert.That(ticket.SystemNumber[3], Is.EqualTo('1'), "System number must have fourth digit equals to 1.");

            Assert.That(ticket.LongTerm, Is.AnyOf("Разовый", "Многоразовый"), "Wrong ticket's long term.");
            Assert.That(ticket.Fullness, Is.AnyOf("Полный", "Частичный"), "Wrong ticket's fullness.");

            Assert.That(ticket.FareCost,
                ticket.Fullness == "Полный" 
                    ? Is.EqualTo(ticket.BaseCost) 
                    : Is.LessThan(ticket.BaseCost));
            
            Assert.That(ticket.TotalCost / ticket.FareCost,
                ticket.LongTerm == "Разовый"
                ? Is.EqualTo(1.0f)
                : Is.GreaterThan(1.0f));
        });
    }

    private void AssertCorrectStationName(string stationName)
    {
        Assert.That(
            stationName, Does.Not.Match(@"\s\.|\.\S"), 
            "Dot must be placed after word and have a space after. Example: \"Sentence. Sentence\"");
        Assert.That(
            stationName, Does.Not.Match(@"[\D-[\s]]\d+|\d+[\D-[\s]]"),
            "Number must be isolated with spaces. Example: \"Sentence. 74 Sentence\"");
        Assert.That(
            stationName, Does.Not.Match(@"[\W\d]-{1}|-{1}[\W\d]"),
            "Hyphen must be sandwiched between letters. Example: \"Word-Word\"");
    }

    private class TicketsSource
    {
        public static IEnumerable<string> TicketFiles(string? folder = null)
        {
            if (folder is not null)
            {
                return Directory.EnumerateFiles(folder, "*.txt");
            }

            string workingDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo? currentirectory = new DirectoryInfo(workingDirectory);
            while (!currentirectory.EnumerateDirectories("promit").Any())
            {
                currentirectory = currentirectory.Parent;
                if (currentirectory is null)
                {
                    throw new Exception($"\"promit\" folder not found in parents of working directory {workingDirectory}");
                }
            }
            
            return Directory.GetFiles($"{currentirectory.FullName}{Path.DirectorySeparatorChar}promit", "*.txt");
        }
    }
}