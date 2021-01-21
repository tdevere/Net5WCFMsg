using ServiceReference1;
using System;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Create a client
            CalculatorClient client = new CalculatorClient();
            DotNetFiveWCFMessageLogger.MsgLogger.GetInstance.WriteMsg("client started");
            client.Endpoint.EndpointBehaviors.Add(new DotNetFiveWCFMessageLogger.EndpointLoggingBehavior("CalculatorService"));

            try
            {
                var openResult = client.Add(1, 1);

                Console.Write(openResult);

                client.Close();
            }
            catch (CommunicationException e)
            {
                client.Abort();
            }
            catch (TimeoutException e)
            {
                client.Abort();
            }
            catch (Exception e)
            {
                client.Abort();
            }
        }
    }
}
