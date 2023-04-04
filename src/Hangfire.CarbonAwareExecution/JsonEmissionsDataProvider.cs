using System.Collections.Concurrent;
using System.Net.Http.Headers;
using CarbonAware.DataSources.Memory;
using CarbonAware.Model;

namespace Hangfire.CarbonAwareExecution;

public class JsonEmissionsDataProvider : IEmissionsDataProvider
{
    private readonly HttpClient m_HttpClient;
    private readonly ConcurrentDictionary<ComputingLocation, EmissionsForecastDataCache> m_EmissionsForecastData = new();

    public JsonEmissionsDataProvider()
    {
        m_HttpClient=new HttpClient();
    }
    public JsonEmissionsDataProvider(HttpClient httpClient)
    {
        m_HttpClient = httpClient;
    }

    public async Task<List<EmissionsData>> GetForecastData(Location location)
    {
        var computingLocation = new ComputingLocation(location.Name ?? "de");
        var emissionsForecastDataCache = GetEmissionsForecastDataCache(computingLocation);
        return await emissionsForecastDataCache.GetForecastData();
    }

    public async Task<DataBoundary> GetDataBoundary(ComputingLocation computingLocation)
    {
        var emissionsForecastDataCache = GetEmissionsForecastDataCache(computingLocation);
        return await emissionsForecastDataCache.GetDataBoundary();
    }

    private EmissionsForecastDataCache GetEmissionsForecastDataCache(ComputingLocation computingLocation)
    {
        if (m_EmissionsForecastData.ContainsKey(computingLocation))
        {
            return m_EmissionsForecastData[computingLocation];
        }
        var cache = new EmissionsForecastDataCache(m_HttpClient, computingLocation);
        m_EmissionsForecastData[computingLocation] = cache;
        return cache;
    }


}
internal class EmissionsForecastDataCache
{
    private readonly HttpClient m_HttpClient;
    private List<EmissionsData> m_EmissionsData = new();
    private DateTimeOffset m_LastRequestTime = DateTimeOffset.MinValue;
    private string? m_ETag;
    public ComputingLocation Location { get; }

    public EmissionsForecastDataCache(HttpClient httpClient, ComputingLocation location)
    {
        Location = location;
        m_HttpClient = httpClient;
    }
    public async Task<List<EmissionsData>> GetForecastData()
    {
        await DownloadDataToCache();
        return m_EmissionsData;
    }
    public async Task<DataBoundary> GetDataBoundary()
    {
        await DownloadDataToCache();
        if (m_EmissionsData.Count == 0)
        {
            return new DataBoundary(DateTimeOffset.MaxValue, DateTimeOffset.MinValue);
        }
        return new DataBoundary(m_EmissionsData[0].Time, m_EmissionsData[^1].Time);
    }
    private async Task DownloadDataToCache()
    {
        if (DateTimeOffset.Now < m_LastRequestTime.AddMinutes(5))
        {
            return;
        }
        m_LastRequestTime = DateTimeOffset.Now;
        var locationName = Location.Name;
        var uri = new Uri($"https://carbonawarecomputing.blob.core.windows.net/forecasts/{locationName}.json");
        m_HttpClient.DefaultRequestHeaders.IfNoneMatch.Clear();
        if (string.IsNullOrEmpty(m_ETag))
        {
            m_ETag = "\"*\"";
        }

        if (!m_ETag.StartsWith("\""))
        {
            m_ETag = "\"" + m_ETag + "\"";
        }
        m_HttpClient.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(m_ETag));
        var response = await m_HttpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var eTagHeader = response.Headers.FirstOrDefault(h => h.Key.Equals("ETag", StringComparison.InvariantCultureIgnoreCase));
        m_ETag = eTagHeader.Value.FirstOrDefault();

        var json = await response.Content.ReadAsStringAsync();
        var jsonFile = System.Text.Json.JsonSerializer.Deserialize<EmissionsForecastJsonFile>(json)!;
        m_EmissionsData = jsonFile.Emissions.Select(e => new EmissionsData()
        {
            Duration = e.Duration,
            Rating = e.Rating,
            Location = locationName,
            Time = e.Time
        }).ToList();
    }
}
internal class EmissionsForecastJsonFile
{
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.Now;
    public List<EmissionsDataRaw> Emissions { get; set; } = new();
}
internal record EmissionsDataRaw
{
    public DateTimeOffset Time { get; set; }
    public double Rating { get; set; }
    public TimeSpan Duration { get; set; }
}