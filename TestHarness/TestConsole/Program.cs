namespace TestConsole
{
    using System;

    using ServiceBus.Configuration;
    using ServiceBus.Desktop;
    using ServiceBus.Transport.Http.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Service bus host address: ");

            var hostAddress = Console.ReadLine();

            Console.WriteLine("Building Service Bus");

            var serviceBus =
                ServiceBusBuilder.Configure()
                    .WithHostAddress(new Uri(hostAddress))
                    .WithHttpTransport()
                    .AsDesktopApplication()
                    .Build();

            Console.WriteLine("Service Bus built and online.");

            Console.ReadLine();
        }
    }
}
