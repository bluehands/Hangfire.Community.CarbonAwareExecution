namespace Hangfire.CarbonAwareExecution;

public static class ComputingLocations
{
    public static ComputingLocation Germany { get; } = new("de");
}
public record ComputingLocation
{
    public string Name { get; }

    public ComputingLocation(string namedLocation)
    {
        Name = namedLocation.ToLowerInvariant();
    }
}