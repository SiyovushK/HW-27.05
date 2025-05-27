using Hangfire;
using Infrastructure.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class RecurringJobConfiguration
{
    // public static void RegisterRecurringJobs(this IServiceProvider services)
    // {
    //     var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();

    //     recurringJobManager.AddOrUpdate<SendPaymentEmailWithHangFire>(
    //         "Order details",
    //         service => service.Send(),
    //         "0 8 * * *"
    //     );
    // }
}