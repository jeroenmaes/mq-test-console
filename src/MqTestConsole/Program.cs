using System;
using System.Threading;
using System.Threading.Tasks;
using MqTestConsole.Transport;

namespace MqTestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            var settings = new MqSettings
            {
                Channel = "DEMO.DEV.SVRCONN",
                Connection = "localhost(1414)",
                QueueManager = "QM1"
            };
            
            var receiver = new MessageReceiver(settings);
            var sender = new MessageSender(settings);
            
            var receiveStarted = false;
            var sendStarted = false;
            var keyInfo = new ConsoleKeyInfo();
            while (keyInfo.KeyChar != 'e' && keyInfo.KeyChar != 'E')
            {
                Console.WriteLine("Press <e> to Exit, <r> to stop/start receiving messages, <s> to stop/start sending messages");
                keyInfo = Console.ReadKey();
                if (keyInfo.KeyChar == 'r')
                {
                    if (receiveStarted)
                    {
                        Console.WriteLine("Stopping all message receivers...");
                        receiver.StopAll();
                    }
                    else
                    {
                        Console.WriteLine("Starting all message receivers...");
                        StartMessagePumps(receiver);   
                    }

                    receiveStarted = !receiveStarted;
                }
                else if (keyInfo.KeyChar == 's')
                {
                    if (sendStarted)
                    {
                        Console.WriteLine("Stopping all message senders...");
                        sender.StopAll();
                    }
                    else
                    {
                        Console.WriteLine("Starting all message senders...");
                        StartMessageSenders(sender);
                    }

                    sendStarted = !sendStarted;
                }
            }
        }

        private static void StartMessageSenders(MessageSender sender)
        {
            sender.Start("DEV.QUEUE.1");
            sender.Start("DEV.QUEUE.2");
            sender.Start("DEV.QUEUE.3");
            sender.Start("DEV.QUEUE.4");
            sender.Start("DEV.QUEUE.5");
        }

        private static void StartMessagePumps(MessageReceiver receiver)
        {
            receiver.Start("DEV.QUEUE.1", ProcessMessage);
            receiver.Start("DEV.QUEUE.2", ProcessMessage);
            receiver.Start("DEV.QUEUE.3", ProcessMessage);
            receiver.Start("DEV.QUEUE.4", ProcessMessage);
            receiver.Start("DEV.QUEUE.5", ProcessMessage);
        }

        private static Task ProcessMessage(Message message)
        {
            Logger.LogMessage($"ProcessMessage:: {message.MessageId} - {message.Body}");

            //Simulate processing
            Thread.Sleep(100);

            return Task.CompletedTask;
        }
    }
}
