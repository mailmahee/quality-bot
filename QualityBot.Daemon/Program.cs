namespace QualityBotDaemon
{
    using System;

    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Worker starting...");
            var worker = Kernel.Get<Worker>();
            worker.Poll();
        }
    }
}