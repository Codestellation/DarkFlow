using System;
using System.ServiceModel;
using Codestellation.DarkFlow.Bootstrap;
using Codestellation.DarkFlow.Config;

namespace BookingService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var executor = Create
                .Executor
                .MaxConcurrency(1)
                .WithQueuedExecutors(new QueuedExecutorSettings {Name = "default", MaxConcurrency = 1})
                .RouteTasks(x => x.ByNamespace("*").To("default"))
                .Build();

            var register = new BookRegister();
            var service = new BookingService(executor, register);
            var baseAddress = new Uri("net.tcp://localhost:9090");

            var serviceHost = new ServiceHost(service, baseAddress);
            
            serviceHost.Open();

            Console.WriteLine("Press any key to stop");
            Console.ReadKey();

            serviceHost.Close();
        }
    }
}