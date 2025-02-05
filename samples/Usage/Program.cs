using System.Reflection.Metadata.Ecma335;
using CarbonAware.Model;
using CarbonAwareComputing;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage.SQLite;

namespace Usage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string password="";
            string userName="";
            builder.Services.AddHangfireCarbonAwareExecution(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                //.UseCarbonAwareDataProvider(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany)
                .UseCarbonAwareDataProvider(new MockDataProvider(), ComputingLocations.Germany)
                //.UseCarbonAwareExecution(
                //    () => new CarbonAwareExecutionOptions(
                //        new CarbonAwareDataProviderWattTime(userName, password), 
                //        ComputingLocations.Germany))
                .UseInMemoryStorage()
                //.UseSQLiteStorage()
                //.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                //{
                //    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                //    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                //    QueuePollInterval = TimeSpan.Zero,
                //    UseRecommendedIsolationLevel = true,
                //    DisableGlobalLocks = true
                //})
            );

            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHangfireDashboard();
            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
                e.MapSwagger();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapHangfireDashboard();

            app.Run();
        }
    }

    public class MockDataProvider : CarbonAwareDataProvider
    {
        public override Task<ExecutionTime> CalculateBestExecutionTime(ComputingLocation location, DateTimeOffset earliestExecutionTime,
            DateTimeOffset latestExecutionTime, TimeSpan estimatedJobDuration) =>
            Task.FromResult(ExecutionTime.BestExecutionTime(earliestExecutionTime.Add(TimeSpan.FromMinutes(1.5)), TimeSpan.FromMinutes(30), 1));

        public override Task<GridCarbonIntensity> GetCarbonIntensity(ComputingLocation location, DateTimeOffset now) => Task.FromResult(GridCarbonIntensity.EmissionData(location.Name, now, 123));
    }
}