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
            builder.Services
                //.AddHangfireCarbonAwareExecution(configuration => configuration
                .AddHangfire(configuration => configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseCarbonAwareExecution(new CarbonAwareDataProviderOpenData(), ComputingLocations.Germany, Console.WriteLine)
                        //.UseCarbonAwareExecution(
                        //    () => new CarbonAwareExecutionOptions(
                        //        new CarbonAwareDataProviderWattTime(userName, password), 
                        //        ComputingLocations.Germany))
                        //.UseInMemoryStorage()
                        .UseSQLiteStorage()
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