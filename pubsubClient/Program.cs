using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Client
{

    class PubSubClient
    {
        private int clientId;
        Context context = null;
        Socket publisher;
        public PubSubClient(int clientId)
        {
            this.clientId = clientId;
            context = new Context(5);
            publisher = context.Socket(SocketType.PUB);
            publisher.Connect("tcp://127.0.0.1:5563");
        }

        public void SendMessage(int id, string message)
        {
            try
            {
                SendStatus result = publisher.Send(message, Encoding.Unicode);
                Console.WriteLine(result);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            //SimpleMessageReqRep();
            SimplePubMessage();

            Console.ReadLine();
        }


        private static void OneClient10000ParalellMessages()
        {
            int countRequests = 0;
            int countReplies = 0;
            Stopwatch countTime = new Stopwatch();
            PubSubClient client = new PubSubClient(1);
            countTime.Start();

            var result = Parallel.For(0, 10000, i =>
            {
                Interlocked.Increment(ref countRequests);
                client.SendMessage(i, string.Format("Message number : {0}", i));
                Interlocked.Increment(ref countReplies);
            });


            while (!result.IsCompleted) ;

            countTime.Stop();

            Console.WriteLine("It took {0} ms to send 10000 messages", countTime.Elapsed.TotalSeconds);

        }

        private static void SimplePubMessage()
        {
            PubSubClient client = new PubSubClient(1);

            bool run = true;
            int messageId;
            messageId = 0;
            while (run)
            {
                Console.Write("say:");
                string messageToSend = Console.ReadLine();
                if (messageToSend.ToLower().Equals("exit"))
                    run = false;

                client.SendMessage(messageId, messageToSend);
            }
        }
    }
}
