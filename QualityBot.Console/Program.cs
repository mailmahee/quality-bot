
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using QualityBot.ServiceLibrary;


namespace QualityBot.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {

            var selfHost = new System.ServiceModel.ServiceHost(typeof(QualityBotService));

            selfHost.Open();

            System.Console.WriteLine("QualityBotService is up and running with the following endpoints:");
            foreach (ServiceEndpoint se in selfHost.Description.Endpoints)
            {
                Console.WriteLine(se.Address.ToString());
            }

            System.Console.ReadLine();

            selfHost.Close();

        }
    }
}
