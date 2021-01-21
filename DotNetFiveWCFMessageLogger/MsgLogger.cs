using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DotNetFiveWCFMessageLogger
{
    public sealed class MsgLogger
    {
        private static string curDir = System.Environment.CurrentDirectory;
        private static string tStamp = DateTime.Now.Ticks.ToString();
        private static string logFile = "";
        private static MsgLogger instance = null;

        public static MsgLogger GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MsgLogger();
                }
                return instance;
            }
        }

        private MsgLogger()
        {
            logFile = $"{curDir}\\{tStamp}.log";
            Debug.WriteLine(logFile);

            WriteMsg("Log Created");
        }

        public void WriteMsg(string message)
        {
            using (StreamWriter w = File.AppendText(logFile))
            {
                w.WriteLine(message);
                w.WriteLine(System.Environment.NewLine);
            }
        }
    }
    public class EndpointLoggingBehavior : IEndpointBehavior
    {
        public EndpointLoggingBehavior(string serviceName)
        {
            _serviceName = serviceName;
        }

        private readonly string _serviceName;

        public void AddBindingParameters(ServiceEndpoint endpoint,
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new MessageLoggingInspector(_serviceName));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
    public class MessageLoggingInspector : IClientMessageInspector
    {
        private readonly string _serviceName;

        public MessageLoggingInspector(string serviceName)
        {
            _serviceName = serviceName;
        }
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            MsgLogger.GetInstance.WriteMsg("AfterReceiveReply");

            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            Message msg = buffer.CreateMessage();
            StringBuilder sb = new StringBuilder();
            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb))
            {
                msg.WriteMessage(xw);
                xw.Close();
            }
            MsgLogger.GetInstance.WriteMsg(sb.ToString());
            //Console.WriteLine("Message Received:\n{0}", sb.ToString());

        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            MsgLogger.GetInstance.WriteMsg("BeforeSendRequest");
            // copying message to buffer to avoid accidental corruption
            var buffer = request.CreateBufferedCopy(int.MaxValue);
            request = buffer.CreateMessage();
            // creating copy
            var copy = buffer.CreateMessage();
            //getting full output message
            //var fullOutputMessage = copy.ToString();
            MsgLogger.GetInstance.WriteMsg(copy.ToString());
            return null;
        }
    }
}
