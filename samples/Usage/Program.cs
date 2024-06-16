using System.Reflection.Metadata.Ecma335;
using CarbonAware.Model;
using CarbonAwareComputing;
using Hangfire;
using Hangfire.SqlServer;

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
                .UseCarbonAwareDataProvider(new CarbonAwareDataProviderOpenData(), ComputingLocations.UnitedKingdomLondon)
                //.UseCarbonAwareExecution(
                //    () => new CarbonAwareExecutionOptions(
                //        new CarbonAwareDataProviderWattTime(userName, password), 
                //        ComputingLocations.Germany))
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