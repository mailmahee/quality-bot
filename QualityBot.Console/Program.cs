using QualityBot.ServiceLibrary;
using System;
using System.ServiceModel.Description;

namespace QualityBot.ServiceHost
{
    static class Program
    {
        static void Main(string[] args)
        {
            var selfHost = new System.ServiceModel.ServiceHost(typeof(QualityBotService));

            selfHost.Open();

            Console.WriteLine("QualityBotService is up and running with the following endpoints:");
            foreach (ServiceEndpoint se in selfHost.Description.Endpoints)
            {
                Console.WriteLine(se.Address.ToString());
            }

            Console.ReadLine();

            selfHost.Close();
        }
    }
}