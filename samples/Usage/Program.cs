using CarbonAwareComputing;
using Hangfire;
using Hangfire.Community.CarbonAwareExecution;
using Hangfire.SqlServer;
using Hangfire.Storage.SQLite;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            builder.Services
                //.AddSingleton<CarbonAwareDataProvider, CarbonAwareDataProviderOpenData>()
                .AddLogging(l => l
                    .AddConsole()
                )
                .AddHangfire((serviceProvider, configuration) => configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()

                        //.UseCarbonAwareExecution(ComputingLocations.Germany, serviceProvider) //this requires CarbonAwareDataProvider to be registered in di container
                        .UseCarbonAwareExecution(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany, serviceProvider.GetService<ILogger<ShiftJobCarbonAwareFilter>>())
                        
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

            builder.Services.AddHangfireServer(o => o.Queues = ["default", "other"]);

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHangfireDashboard();
            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            
            app.MapSwagger();
            app.MapControllers();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapHangfireDashboard();

            app.Run();
        }
    }
}