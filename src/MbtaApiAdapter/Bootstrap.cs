namespace MbtaApiAdapter
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Reflection;
    using System.Threading;

    public class Bootstrap
    {
        RootConfiguration _configuration;

        public Bootstrap()
        {

        }

        public void Run()
        {
            var executableName = Assembly.GetEntryAssembly()?.GetName().Name;
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile($"{executableName}.json", false, true);

            _configuration = new RootConfiguration();

            configurationBuilder.Build().Bind(_configuration);

            var publisher = new Publisher(_configuration);

            CancellationTokenSource source = new CancellationTokenSource();

            var scheduleWorker = new ScheduleWorker(_configuration, source.Token, publisher);

            scheduleWorker.DoWork();

            Console.WriteLine("Press Ctr+X to exit.");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.X && key.Modifiers == ConsoleModifiers.Control)
                {
                    source.Cancel();
                    scheduleWorker.Dispose();
                    break;
                }
            }
        }
    }
}
