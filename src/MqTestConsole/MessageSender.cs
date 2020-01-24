using System;
using MqTestConsole.Transport;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MqTestConsole
{
    public class MessageSender
    {
        private ConcurrentBag<Task> _tasks;
        private readonly MqSettings _settings;
        private CancellationTokenSource _ct;

        public MessageSender(MqSettings settings)
        {
            _settings = settings;
            _tasks = new ConcurrentBag<Task>();
            _ct = new CancellationTokenSource();
        }
        public void Start(string queueName)
        {

            var mqSender = new MqSender(_settings);
            var t = Task.Run(() => PutRandomMessages(mqSender, queueName, _ct.Token), _ct.Token);

            _tasks.Add(t);
        }

        private void PutRandomMessages(MqSender mqSender, string queueName, CancellationToken token)
        {
            var generator = new MessageGenerator();
            while (!token.IsCancellationRequested)
            {
                mqSender.PutMessage(queueName, generator.RandomString(1024), Guid.NewGuid().ToString("N").ToUpper());

                Thread.Sleep(100);
            }
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
