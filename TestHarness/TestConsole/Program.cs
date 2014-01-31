namespace TestConsole
{
    using System;

    using ServiceBus.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Service bus host address: ");

            var hostAddress = Console.ReadLine();

            Console.WriteLine("Building Service Bus");
            
            Console.WriteLine("Service Bus built and online.");

            Console.ReadLine();
        }
    }
}
