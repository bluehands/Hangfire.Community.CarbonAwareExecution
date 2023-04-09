using CarbonAware.Model;
using CarbonAwareComputing.ExecutionForecast;
using Hangfire;
using Hangfire.CarbonAwareExecution;
using Hangfire.SqlServer;

namespace Usage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseCarbonAwareExecution(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany)
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHangfireDashboard();

            app.Run();
        }
    }
}