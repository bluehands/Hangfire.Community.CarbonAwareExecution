using CarbonAwareComputing.Hangfire;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Hangfire;

public static class HangfireServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireCarbonAwareExecution(
        [NotNull] this IServiceCollection services,
        [NotNull] Action<IGlobalConfiguration> configuration)
    {
        return AddHangfireCarbonAwareExecution(services, (provider, config) => configuration(config));
    }

    public static IServiceCollection AddHangfireCarbonAwareExecution(
        [NotNull] this IServiceCollection services,
        [NotNull] Action<IServiceProvider, IGlobalConfiguration> configuration)
    {
        services.TryAddSingleton<IBackgroundJobPerformer>(x => new CustomBackgroundJobPerformer(
            new BackgroundJobPerformer(
                x.GetRequiredService<IJobFilterProvider>(),
                x.GetRequiredService<JobActivator>(),
                TaskScheduler.Default)));


        services.TryAddSingleton<IBackgroundJobStateChanger>(x => new CustomBackgroundJobStateChanger(
            new BackgroundJobStateChanger(x.GetRequiredService<IJobFilterProvider>())));

        services.TryAddSingleton<IBackgroundJobFactory>(x =>
            new CustomBackgroundJobFactory(
                new BackgroundJobFactory(x.GetRequiredService<IJobFilterProvider>()),
                x.GetRequiredService<IBackgroundJobClient>
            )
        );
        return services.AddHangfire(configuration);
    }
}

