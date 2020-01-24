using MqTestConsole.Transport;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MqTestConsole
{
    class MessageReceiver
    {
        private ConcurrentBag<Task> _tasks;
        private readonly MqSettings _settings;
        private CancellationTokenSource _ct;

        public MessageReceiver(MqSettings settings)
        {
            _settings = settings;
            _tasks = new ConcurrentBag<Task>();
            _ct = new CancellationTokenSource();
        }

        public void Start(string queueName, Func<Message, Task> messageHandler)
        {
            
            var mqReceiver = new MqReceiver(_settings);
            var t = Task.Run(() => mqReceiver.GetMessages(queueName, messageHandler, _ct.Token), _ct.Token);
            
            _tasks.Add(t);
        }

        public void StopAll()
        {
            _ct.Cancel();
            Task.WhenAll(_tasks.ToArray()).ConfigureAwait(false);
            _ct.Dispose();

            _tasks = new ConcurrentBag<Task>();
            _ct = new CancellationTokenSource();
        }
    }
}
