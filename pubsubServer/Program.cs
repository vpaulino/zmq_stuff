using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;
namespace pubsubServer
{

    class PubSubServer
    {
        private int serverId;
        public PubSubServer(int serverId)
        {
            this.serverId = serverId;

        }

        public Action<String> MessageReceived;

        private void NotifyMessageReceived(string messageReceived)
        {
            if (MessageReceived != null)
            {
                MessageReceived(messageReceived);
            }
        }

        public int Init(ref int countMessages)
        {
            byte[] zmq_buffer = new byte[1024];

            Console.WriteLine("Connecting to localhost:5555....");
            
            Context context = new Context(1);
            using (Socket subscriber = context.Socket(SocketType.SUB)) 
            { 
                subscriber.Bind("tcp://*:5555");

                Console.WriteLine("Connected to localhost:5555 !!");
                countMessages = 0;

                subscriber.Subscribe("", Encoding.Unicode);

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Waiting for messages...");
                        //  Wait for next request from client
                        zmq_buffer = subscriber.Recv();
                        string messageReceived  = Encoding.ASCII.GetString(zmq_buffer);
                        Interlocked.Increment(ref countMessages);

                        NotifyMessageReceived(messageReceived);
                      
                    }
                    catch (ZMQ.Exception z)
                    {
                       
                        Console.WriteLine("ZMQ Exception occurred : {0}", z.Message);
                    }

                }
            }



        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            PubSubServer server = new PubSubServer(0);

            server.MessageReceived = messageReceived =>
            {
                Console.WriteLine("Received request: {0}", messageReceived);
            };

            int countMessages = 0;
            server.Init(ref countMessages);
            Console.ReadLine();

            Console.WriteLine("NumberOfMessages: {0}", countMessages);
        }
    }
}
