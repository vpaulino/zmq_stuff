using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMQ;
using System.Threading;

namespace Server
{

    class ReqReplServer
    {
        private int serverId;
        public ReqReplServer(int serverId)
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
            //  Prepare our context and socket
            Context context = new Context(10);
            Socket socket = context.Socket(SocketType.REP);
            socket.Bind("tcp://*:5555");
            Console.WriteLine("Connected to localhost:5555 !!");
              countMessages = 0;
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for messages...");
                    //  Wait for next request from client
                    zmq_buffer = socket.Recv();
                    string request = Encoding.ASCII.GetString(zmq_buffer);

                    Interlocked.Increment(ref countMessages);

                    NotifyMessageReceived(request); 
                    // log that we got one
                    //Console.WriteLine("Received request: [%s]", request);
                 
                    //  Send reply back to client
                    socket.Send(Encoding.Unicode.GetBytes("Message received ".ToCharArray()));

                }
                catch (ZMQ.Exception z)
                {
                    // report the exception
                    Console.WriteLine("ZMQ Exception occurred : {0}", z.Message);
                }

            }

           

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            ReqReplServer server = new ReqReplServer(0);
          
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
