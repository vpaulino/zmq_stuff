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

    class ReqReplClient 
    {
        private int clientId;
        Context context = null;
        public ReqReplClient(int clientId) 
        {
            this.clientId = clientId;
            context = new Context(5);
        }

        public string SendMessage(int id, string message) 
        {
            string reply;
            
            using (Socket requester = context.Socket(SocketType.REQ))
            {
                requester.Connect("tcp://localhost:5555");

                requester.Send(message, Encoding.Unicode);
                    
                reply =  requester.Recv(Encoding.Unicode);

            }
            
            return reply;
        }
    
    }

    class Program
    {

        static void Main(string[] args)
        {
            //SimpleMessageReqRep();
            OneClient10000ParalellMessages();

            Console.ReadLine();
        }


        private static void OneClient10000ParalellMessages() 
        {
            int countRequests = 0;
            int countReplies = 0;
            Stopwatch countTime = new Stopwatch();
            ReqReplClient client = new ReqReplClient(1);
            countTime.Start();

            var result = Parallel.For(0, 10000, i =>
            {
                Interlocked.Increment(ref countRequests);
                if(!String.IsNullOrEmpty(client.SendMessage(i, string.Format("Message number : {0}", i))))
                    Interlocked.Increment(ref countReplies);
            });


            while (!result.IsCompleted) ;

            countTime.Stop();

            Console.WriteLine("It took {0} ms to send 10000 messages", countTime.Elapsed.TotalSeconds);

        }

        private static void SimpleMessageReqRep()
        {
            ReqReplClient client = new ReqReplClient(1);

            bool run = true;
            int messageId;
            messageId = 0;
            while (run)
            {
                Console.Write("say:");
                string messageToSend = Console.ReadLine();
                if (messageToSend.ToLower().Equals("exit"))
                    run = false;

                var response = client.SendMessage(messageId, messageToSend);
                Console.Write("server says: {0}", response);

            }
        }
    }
}
